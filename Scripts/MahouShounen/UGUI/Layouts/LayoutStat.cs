using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class LayoutStat : LayoutBase
{
    public GameObject buttonReturn;
    
    [Header("Stat Profile")]
    public Text textPlayerName;
    public Text textBirthday;
    public Text textStressValue;
    public Scrollbar scrollBarStress;
    public Text textCurrency;
    
    [Header("Stat List")]
    public List<ElementStatInfo> elementStatInfos;
    
    [Header("Stat Schedule List")]
    public Toggle tabClass;
    public Toggle tabPartTime;
    public Toggle tabRest;
    
    public Transform contentSchedule;
    public GameObject prefabElementStatSchedule;
    
    List<ElementStatSchedule> elementStatSchedules;
    
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
        buttonReturn.GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);
        
        SetPlayerSettings();
        
        #region Stat Elements
        foreach (var element in elementStatInfos)
        {
            element.SetParentComponent(this);
        }
        #endregion
        
        #region Stat Schedules
        tabClass.   onValueChanged.AddListener(delegate { SetScheduleType(ScheduleType.Class);    });
        tabPartTime.onValueChanged.AddListener(delegate { SetScheduleType(ScheduleType.PartTime); });
        tabRest.    onValueChanged.AddListener(delegate { SetScheduleType(ScheduleType.Others);   });

        var prefabRectTransform = prefabElementStatSchedule.GetComponent<RectTransform>();
        var elementSize = prefabRectTransform.sizeDelta;
        
        elementStatSchedules = new List<ElementStatSchedule>();
        for (var i = 0; i < MSDataManager.Get().GetMaxScheduleCountByType(); i++)
        {
            var prefabObject = Object.Instantiate(prefabElementStatSchedule, contentSchedule);

            var element = prefabObject.GetComponent<ElementStatSchedule>();
            element.SetParentComponent(this);

            var rectTransform = prefabObject.GetComponent<RectTransform>();
            
            var posX = (i / 2) * (elementSize.x + 3f);
            var posY = (i % 2) * (elementSize.y + 3f);
            rectTransform.anchoredPosition = new Vector2(posX, -posY);
            
            elementStatSchedules.Add(element);
        }
        #endregion
    }

    public override void OpenLayout(string layoutName = "On")
    {
        base.OpenLayout(layoutName);
        SetScheduleType(ScheduleType.Class);
    }

    private void SetPlayerSettings()
    {
        // Name
        textPlayerName.text = MSGameInstance.Get().playerName;
        
        // Birthday
        var   dayKey = MSGameInstance.Get().playerBirthday.  day.ToString();
        var  weekKey = MSGameInstance.Get().playerBirthday. week.ToString();
        var monthKey = MSGameInstance.Get().playerBirthday.month.ToString();

        // if (LocalizationSystem.Get().IsUsingContractionInFullDate())
        // {
        //     monthKey = "Cont" + monthKey;
        //       dayKey = "Cont" +   dayKey;
        // }

        var birthdayString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", dayKey) + ", "
                           + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", weekKey) + ", "
                           + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", monthKey);

        textBirthday.text = birthdayString;
        
        // Stress
        MSGameInstance.Get().onStressChanged.AddListener(UpdateStress);
        UpdateStress(MSGameInstance.Get().playerStress);
        
        // Currency
        MSGameInstance.Get().onCurrencyChanged.AddListener(UpdateCurrency);
        UpdateCurrency(MSGameInstance.Get().currency);
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
    
    public void SetScheduleType(ScheduleType scheduleType)
    {
        foreach (var element in elementStatSchedules)
        {
            element.gameObject.SetActive(false);
        }
        
        var scheduleDatas = MSDataManager.Get().GetScheduleDatasByType(scheduleType);
        for (var i = 0; i < scheduleDatas.Count; i++)
        {
            elementStatSchedules[i].SetScheduleData(scheduleDatas[i]);
            elementStatSchedules[i].gameObject.SetActive(true);
        }
        
        // Content Schedule Size
        var prefabRectTransform = prefabElementStatSchedule.GetComponent<RectTransform>();
        var elementSize = prefabRectTransform.sizeDelta;

        var contentScheduleTransform = contentSchedule.GetComponent<RectTransform>();
        contentScheduleTransform.sizeDelta = new Vector2((elementSize.x + 3f) * ((scheduleDatas.Count / 2) + 1) - 3f, 
                                                         (elementSize.y * 2f) + 3f);
    }

    private void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
}
