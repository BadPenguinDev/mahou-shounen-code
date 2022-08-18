using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSchedulePreset : PopUpBase
{
    [Header("Schedule Preset List")]
    public GameObject prefabSchedulePresetElement;
    public RectTransform contentSchedulePresets;

    SchedulePreset preset;

    List<ElementSchedulePreset>   activeSchedulePresetElements;
    List<ElementSchedulePreset> inactiveSchedulePresetElements;
    
    [Header("Schedule Preset Info")]
    public List<ElementScheduleSelected> scheduleSelectedPanels;
    
    public Button buttonReturn;
    public Button buttonLoadPreset;
    public Button buttonDeletePreset;
    
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

          activeSchedulePresetElements = new List<ElementSchedulePreset>();
        inactiveSchedulePresetElements = new List<ElementSchedulePreset>();

        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        buttonLoadPreset.onClick.AddListener(OnButtonLoadPresetClicked);
        buttonDeletePreset.onClick.AddListener(OnButtonDeletePresetClicked);
    }
    public override void OpenPopUp()
    {
        base.OpenPopUp();
        UpdateSchedulePresetList();
        
        for (var i = 0; i < scheduleSelectedPanels.Count; i++)
        {
            scheduleSelectedPanels[i].SetScheduleData(null);
        }
        foreach (var element in activeSchedulePresetElements)
        {
            element.toggleSchedulePreset.SetIsOnWithoutNotify(false);
        }
    }

    public void UpdateSchedulePresetList()
    {
        var presetList = MSGameInstance.Get().schedulePresets;
        
        foreach (var element in activeSchedulePresetElements)
        {
            element.gameObject.SetActive(false);
            inactiveSchedulePresetElements.Add(element);
        }
        activeSchedulePresetElements.Clear();
        
        float elementHeight = -1;
        var elementPos = prefabSchedulePresetElement.GetComponent<RectTransform>().anchoredPosition;
        
        for (var i = 0; i < presetList.Count; i++)
        {
            ElementSchedulePreset element;
            RectTransform elementTransform;

            if (inactiveSchedulePresetElements.Count > 0)
            {
                element = inactiveSchedulePresetElements[0];
                element.gameObject.SetActive(true);
                elementTransform = element.GetComponent<RectTransform>();

                inactiveSchedulePresetElements.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabSchedulePresetElement, contentSchedulePresets);

                element = instance.GetComponent<ElementSchedulePreset>();
                element.SetParentComponent(this);
                element.toggleSchedulePreset.onValueChanged.AddListener(delegate { OnButtonSchedulePresetClicked(element); });

                elementTransform = element.GetComponent<RectTransform>();
            }
            activeSchedulePresetElements.Add(element);

            if (elementHeight < 0f)
                elementHeight = elementTransform.sizeDelta.y + 4f;

            element.SetSchedulePreset(presetList[i]);
            elementTransform.anchoredPosition = elementPos - new Vector2(0f, elementHeight * i);

            contentSchedulePresets.sizeDelta += new Vector2(0f, -elementHeight);
        }
        contentSchedulePresets.sizeDelta = new Vector2(0f, 2f + elementHeight * (presetList.Count));
        contentSchedulePresets.anchoredPosition = Vector2.zero;
    }
    public void SetSchedulePreset(SchedulePreset targetPreset)
    {
        preset = targetPreset;
        for (var i = 0; i < scheduleSelectedPanels.Count; i++)
        {
            if (i < targetPreset.Count)
            {
                var data = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == targetPreset[i]);
                scheduleSelectedPanels[i].SetScheduleData(data);
            }
            else
            {
                scheduleSelectedPanels[i].SetScheduleData(null);
            }
        }
    }
    
    private void OnButtonSchedulePresetClicked(ElementSchedulePreset targetElement)
    {
        foreach (var element in activeSchedulePresetElements)
        {
            element.toggleSchedulePreset.SetIsOnWithoutNotify(false);
        }
        targetElement.toggleSchedulePreset.SetIsOnWithoutNotify(true);
        
        SetSchedulePreset(targetElement.GetSchedulePreset());
    }

    void OnButtonReturnClicked()
    {
        ClosePopUp();
    }
    void OnButtonLoadPresetClicked()
    {
        if (preset == null)
            return;
        
        var layoutSchedule = (LayoutSchedule)manager.layouts[LayoutType.Schedule];
        layoutSchedule.SetSchedulesFromPreset(preset);
        
        ClosePopUp();
    }
    void OnButtonDeletePresetClicked()
    {
        if (preset == null)
            return;
        
        MSGameInstance.Get().RemoveSchedulePreset(preset.id);
        
        UpdateSchedulePresetList();
        preset = null;
        
        for (var i = 0; i < scheduleSelectedPanels.Count; i++)
        {
            scheduleSelectedPanels[i].SetScheduleData(null);
        }
        foreach (var element in activeSchedulePresetElements)
        {
            element.toggleSchedulePreset.SetIsOnWithoutNotify(false);
        }
    }
}
