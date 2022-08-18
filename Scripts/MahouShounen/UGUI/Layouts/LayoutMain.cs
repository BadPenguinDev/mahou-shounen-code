using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;


public enum LayoutMainMode { Schedule, WeekendDay, WeekendNight }

[System.Serializable] 
public class OnLayoutMainModeChangedEvent : UnityEvent<LayoutMainMode> { }

public class LayoutMain : LayoutBase
{
    public LayoutMainMode mode;
    
    [Header("Main Menu Date")]
    public Text textMonth;
    public Text textWeek;
    public Text textDay;

    [Header("Main Menu Profile")]
    public Text textCurrency;
    public Text textPlayerName;
    public Text textStressValue;
    public Scrollbar scrollBarStress;

    [Header("Main Button Info")]
    public Button buttonStat;
    public Button buttonSocial;
    public Button buttonSkill;
    public Button buttonDressRoom;
    public Button buttonAchievement;

    [Space()]
    public Button buttonStore;
    public Button buttonInventory;
    public Button buttonSettings;

    [Header("Main Button Interaction")]
    public Button buttonSchedule;
    public Button buttonMap;
    public Button buttonDreamFlow;

    [Header("Main Portrait")]
    public Image imagePortraitPlayer;
    public Image imageClothingPlayer;

    [Header("Main Events")]
    public OnLayoutMainModeChangedEvent onLayoutMainModeChanged;

    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        buttonStat.onClick.AddListener(OnButtonStatClicked);
        buttonSocial.onClick.AddListener(OnButtonSocialClicked);
        buttonSkill.onClick.AddListener(OnButtonSkillClicked);
        buttonDressRoom.onClick.AddListener(OnButtonDressRoomClicked);
        buttonAchievement.onClick.AddListener(OnButtonAchievementClicked);

        buttonStore.onClick.AddListener(OnButtonStoreClicked);
        buttonInventory.onClick.AddListener(OnButtonInventoryClicked);
        buttonSettings.onClick.AddListener(OnButtonSettingsClicked);

        buttonSchedule.onClick.AddListener(OnButtonSchedulelClicked);
        buttonMap.onClick.AddListener(OnButtonMapClicked);
        buttonDreamFlow.onClick.AddListener(OnButtonDreamFlowClicked);
        
        // Name
        textPlayerName.text = MSGameInstance.Get().playerName;

        // Stress
        MSGameInstance.Get().onStressChanged.AddListener(UpdateStress);
        UpdateStress(MSGameInstance.Get().playerStress);

        // Currency
        MSGameInstance.Get().onCurrencyChanged.AddListener(UpdateCurrency);
        UpdateCurrency(MSGameInstance.Get().currency);
        
        // Events
        onLayoutMainModeChanged = new OnLayoutMainModeChangedEvent();
        onLayoutMainModeChanged.AddListener(MSGameInstance.Get().OnLayoutModeChanged);
    }

    public override void OpenLayout(string layoutName = "On")
    {
        base.OpenLayout(layoutName);

        // Set Date
        var date = MSGameInstance.Get().currentDate;
        var monthKey = "Cont" + date.month.ToString();

        textMonth.GetComponent<LocalizeStringEvent>().SetEntry(monthKey);
        textMonth.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");

        textWeek.GetComponent<LocalizeStringEvent>().SetEntry(date.week.ToString());
        textWeek.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");

        textDay.GetComponent<LocalizeStringEvent>().SetEntry(date.day.ToString());
        textDay.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");
        
        switch (mode)
        {
            case LayoutMainMode.Schedule:
            case LayoutMainMode.WeekendDay:
                var casualwearIndex = (int)MSGameInstance.Get().casualwear;
                imageClothingPlayer.sprite = MSDataManager.Get().playerCharacterData.casualWearSprites[casualwearIndex];
                
                break;
            case LayoutMainMode.WeekendNight:
                var costumeIndex = (int)MSGameInstance.Get().costume;
                imageClothingPlayer.sprite = MSDataManager.Get().playerCharacterData.costumeSprites[costumeIndex];
                
                break;
        }
    }

    public void SetLayoutMainMode(LayoutMainMode mode)
    {
        this.mode = mode;

        switch (mode)
        {
            case LayoutMainMode.Schedule:
            {
                UGUIManager.Get().SetBackgroundDay();

                buttonSchedule.gameObject.SetActive(true);
                buttonMap.gameObject.SetActive(false);
                buttonDreamFlow.gameObject.SetActive(false);

                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                imageClothingPlayer.sprite = MSDataManager.Get().playerCharacterData.casualWearSprites[clothingIndex];
                break;
            }
            case LayoutMainMode.WeekendDay:
                UGUIManager.Get().SetBackgroundDay();

                buttonSchedule.gameObject.SetActive(false);
                buttonMap.gameObject.SetActive(true);
                buttonDreamFlow.gameObject.SetActive(false);
                break;
            case LayoutMainMode.WeekendNight:
            {
                UGUIManager.Get().SetBackgroundNight();

                buttonSchedule.gameObject.SetActive(false);
                buttonMap.gameObject.SetActive(false);
                buttonDreamFlow.gameObject.SetActive(true);

                var clothingIndex = (int)MSGameInstance.Get().costume;
                imageClothingPlayer.sprite = MSDataManager.Get().playerCharacterData.costumeSprites[clothingIndex];
                
                var layoutDreampost = manager.GetLayout(LayoutType.Dreampost) as LayoutDreampost;
                layoutDreampost.LoadDreamposts();
                break;
            }
        }
        
        manager.GetComponent<MSGameController>().PlayAudioByLayoutMainMode(mode);
        onLayoutMainModeChanged.Invoke(mode);
    }

    private void UpdateStress(int value)
    {
        textStressValue.text = value.ToString();
        scrollBarStress.size = value / (float)MSCommon.MaxStressGauge;
        
        // Text Preset
        var scrollBarSize = scrollBarStress.size;
        UGUIManager.Get().SetScrollBarTextPreset(textStressValue, scrollBarSize);
    }
    private void UpdateCurrency(int value)
    {
        textCurrency.text = value.ToString();
    }
    
    private void OnButtonStatClicked()
    {
        manager.OpenLayout(LayoutType.Stat);
        CloseLayout();
    }

    private void OnButtonSocialClicked()
    {
        manager.OpenLayout(LayoutType.Social);
        CloseLayout();
    }

    private void OnButtonSkillClicked()
    {
        manager.OpenLayout(LayoutType.Skill);
        CloseLayout();
    }
    
    private void OnButtonDressRoomClicked()
    {
        manager.OpenLayout(LayoutType.DressRoom);
        CloseLayout();
    }
    
    private void OnButtonAchievementClicked()
    {
        
    }
    
    private void OnButtonStoreClicked()
    {
        
    }

    private void OnButtonInventoryClicked()
    {
        manager.OpenLayout(LayoutType.Inventory);
        CloseLayout();
    }
    
    private void OnButtonSettingsClicked()
    {
        UGUIManager.Get().OpenPopUp(PopUpType.Settings);
    }

    private void OnButtonSchedulelClicked()
    {
        manager.OpenLayout(LayoutType.Schedule);
        CloseLayout();
    }

    private void OnButtonMapClicked()
    {
        manager.OpenLayout(LayoutType.WorldMap);
        CloseLayout();
    }

    private void OnButtonDreamFlowClicked()
    {
        manager.OpenLayout(LayoutType.Dreampost);
        CloseLayout();
    }
}