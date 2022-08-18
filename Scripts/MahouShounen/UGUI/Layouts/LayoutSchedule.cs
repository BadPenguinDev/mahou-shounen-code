using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.Events;


public class LayoutSchedule : LayoutBase
{
    public ScheduleData targetScheduleData;
    public Button buttonReturn;

    [Header("Schedule Grid List")]
    public Transform gridListToggleSchedule;
    public GameObject prefabElementScheduleSelectToggle;
    public List<ElementScheduleSelectToggle> elementScheduleSelectToggles;

    [Header("Schedule Type Toggle")]
    public Toggle tabClass;
    public Toggle tabPartTime;
    public Toggle tabRest;

    [Header("Schedule Details")]
    public Text textScheduleDescription;
    public Transform verticalListScheduleStat;
    public GameObject panelScheduleStatPrefab;
    public List<ElementScheduleStat> scheduleStatPanels;
    public ElementScheduleStress scheduleStressPanel;

    [Header("Schedule Selected")]
    public List<ElementScheduleSelected> scheduleSelectedPanels;
    public Button buttonSavePreset;
    public Button buttonLoadPreset;
    public OutlineColored textSavePresetOutline;

    [Header("Schedule Start Button")]
    public Button buttonPlaySchedule;
    public OutlineColored textPlayScheduleOutline;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        
        // Element Schedule Type Toggle
        tabClass.   onValueChanged.AddListener(SetScheduleTypeClass   );
        tabPartTime.onValueChanged.AddListener(SetScheduleTypePartTime);
        tabRest.    onValueChanged.AddListener(SetScheduleTypeOthers  );

        // Element Schedule Select Toggle
        for (var i = 0; i < MSDataManager.Get().GetMaxScheduleCountByType(); i++)
        {
            var prefabObject = Object.Instantiate(prefabElementScheduleSelectToggle, gridListToggleSchedule);

            var element = prefabObject.GetComponent<ElementScheduleSelectToggle>();
            element.SetParentComponent(this);

            elementScheduleSelectToggles.Add(element);
        }

        // Element Schedule Select Toggle
        for (var i = 0; i < 3; i++)
        {
            var prefabObject = Object.Instantiate(panelScheduleStatPrefab, verticalListScheduleStat);
            var element = prefabObject.GetComponent<ElementScheduleStat>();
            element.SetParentComponent(this);

            scheduleStatPanels.Add(element);
        }

        // Element Schedule Select Toggle
        foreach (var element in scheduleSelectedPanels)
        {
            element.SetParentComponent(this);
        }
        buttonSavePreset.onClick.AddListener(OnButtonSavePresetClicked);
        buttonLoadPreset.onClick.AddListener(OnButtonLoadPresetClicked);
    }

    public override void OpenLayout(string layoutName)
    {
        base.OpenLayout(layoutName);
        SetScheduleType(ScheduleType.Class);
        SetLastSchedules(MSGameInstance.Get().lastSchedules);
        
        SetButtonPlaySchedulePalette();
    }

    public void SetScheduleType(ScheduleType type)
    {
        ClearToggleScheduleSelect();

        var scheduleDatas = MSDataManager.Get().GetScheduleDatasByType(type);
        for (var i = 0; i < scheduleDatas.Count; i++)
        {
            elementScheduleSelectToggles[i].SetScheduleData(scheduleDatas[i]);
            elementScheduleSelectToggles[i].gameObject.SetActive(true);
        }
        SetScheduleData(scheduleDatas[0]);
    }
    public void SetScheduleTypeClass   (bool status)
    {
        if (status)
            SetScheduleType(ScheduleType.Class);
    }
    public void SetScheduleTypePartTime(bool status)
    {
        if (status)
            SetScheduleType(ScheduleType.PartTime);
    }
    public void SetScheduleTypeOthers  (bool status)
    {
        if (status)
            SetScheduleType(ScheduleType.Others);
    }
    
    public void SetScheduleData(ScheduleData data)
    {
        if (data == null)
            return;
        
        ClearPanelScheduleStat();
        targetScheduleData = data;
        
        // Select Toggles
        foreach (var element in elementScheduleSelectToggles)
        {
            if (!element.IsScheduleAvailable())
                element.toggleScheduleSelect.SetIsOnWithoutNotify(false);
            else
                element.toggleScheduleSelect.SetIsOnWithoutNotify(element.schedule == targetScheduleData);
        }
        
        // Info
        var grade = MSGameInstance.Get().GetScheduleGrade(data);

        textScheduleDescription.GetComponent<LocalizeStringEvent>().SetTable("Descriptions");
        textScheduleDescription.GetComponent<LocalizeStringEvent>().SetEntry(data.keyScheduleName);

        scheduleStressPanel.SetValue(data.GetStress(grade));

        var statDatas = data.GetStats(grade);
        for (var i = 0; i < statDatas.Count; i++)
        {
            scheduleStatPanels[i].gameObject.SetActive(true);
            scheduleStatPanels[i].SetStat(statDatas[i].type, statDatas[i].modifier);
        }
    }

    public void SetLastSchedules(List<Schedule> schedules)
    {
        for (var i = 1; i < schedules.Count; i++)
        {
            if ((i - 1) >= scheduleSelectedPanels.Count)
                break;
            
            var targetData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == schedules[i]);
            scheduleSelectedPanels[i - 1].SetScheduleData(targetData, true);
        }
        
        CheckScheduleSelectFinished();
    }
    public void SetSchedulesFromPreset(SchedulePreset preset)
    {
        for (var i = 0; i < preset.Count; i++)
        {
            var targetData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == preset.schedules[i]);
            scheduleSelectedPanels[i].SetScheduleData(targetData, true);
        }
        
        CheckScheduleSelectFinished();
    }
    public void SetScheduleSelectFinished()
    {
        if (!CheckScheduleSelectFinished()) 
            return;
        
        var schedules = new List<Schedule>();
        schedules.Add(Schedule.LiberalArts);

        foreach (var element in scheduleSelectedPanels)
        {
            schedules.Add(element.schedule.schedule);
        }

        MSGameInstance.Get().lastSchedules = schedules;

        var layoutDaily = manager.OpenLayout(LayoutType.Daily) as LayoutDaily;
        layoutDaily.targetSchedules = schedules;
        layoutDaily.onDailyScheduleRoutineStarted.Invoke();

        CloseLayout();
    }

    public void ClearToggleScheduleSelect()
    {
        foreach (var element in elementScheduleSelectToggles)
        {
            element.gameObject.SetActive(false);
        }
    }
    public void ClearPanelScheduleStat()
    {
        foreach (var element in scheduleStatPanels)
        {
            element.gameObject.SetActive(false);
        }
    }

    public void SetButtonPlaySchedulePalette()
    {
        UIPaletteData buttonSavePresetPalette;
        UIPaletteData buttonPlaySchedulePalette;
        
        if (CheckScheduleSelectFinished())
        {
            buttonSavePresetPalette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Positive);
            buttonPlaySchedulePalette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Pointed);
        }
        else
        {
            buttonSavePresetPalette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Disabled);
            buttonPlaySchedulePalette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Disabled);
        }

        buttonSavePreset.GetComponent<Image>().sprite = buttonSavePresetPalette.buttonStandaloneSprite;
        textSavePresetOutline.effectColor = buttonSavePresetPalette.outlineColor;
        
        buttonPlaySchedule.GetComponent<Image>().sprite = buttonPlaySchedulePalette.buttonStandaloneSprite;
        textPlayScheduleOutline.effectColor = buttonPlaySchedulePalette.outlineColor;
    }
    public bool CheckScheduleSelectFinished()
    {
        foreach (var element in scheduleSelectedPanels)
        {
            if (!element.IsScheduleAvailable())
                return false;
        }
        return true;
    }


    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }

    void OnButtonSavePresetClicked()
    {
        var preset = new SchedulePreset();
        foreach (var element in scheduleSelectedPanels)
        {
            preset.Add(element.schedule.schedule);
        }
        MSGameInstance.Get().AddSchedulePreset(preset);
    }

    void OnButtonLoadPresetClicked()
    {
        manager.OpenPopUp(PopUpType.SchedulePreset);
    }
}
