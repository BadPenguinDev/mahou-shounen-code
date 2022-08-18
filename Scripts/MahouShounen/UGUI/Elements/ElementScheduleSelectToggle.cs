using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;


[System.Serializable] public class OnScheduleSelectToggleClicked : UnityEvent<ScheduleData> { }
public class ElementScheduleSelectToggle : ElementBase
{
    [Header("Schedule")]
    public ScheduleData  schedule;
    public ScheduleGrade grade;

    [Header("Components")] 
    public Toggle toggleScheduleSelect;
    public Image imageIconSchedule;
    public LocalizeStringEvent localizeEventScheduleName;
    public Text textCredit;
    public GameObject imageToggleSelected;

    public List<Image>  baseImages;
    public List<Image> fieldImages;
    public List<OutlineColored> outlines;


    public void SetScheduleData(ScheduleData scheduleData)
    {
        schedule = scheduleData;

        grade = MSGameInstance.Get().GetScheduleGrade(schedule);

        imageIconSchedule.sprite = schedule.spriteSchedule;

        localizeEventScheduleName.SetEntry(schedule.keyScheduleName);
        localizeEventScheduleName.SetTable("Schedules");

        textCredit.text = schedule.GetCost(grade).ToString();

        var paletteData = UGUIManager.Get().GetSchedulePaletteData(schedule.type, true);
        foreach (var image in baseImages)
        {
            image.sprite = paletteData.buttonStandaloneSprite;
        }
        foreach (var image in fieldImages)
        {
            image.sprite = paletteData.fieldSquareSprite;
        }
        foreach (var outline in outlines)
        {
            outline.effectColor = paletteData.outlineColor;
        }

        toggleScheduleSelect.interactable = IsScheduleAvailable();
    }
    
    public bool IsScheduleAvailable()
    {
        if (schedule == null)
            return false;
        
        return MSGameInstance.Get().IsScheduleAvailable(schedule);
    }
    
    public void OnScheduleSelectToggleClicked()
    {
        var layout = parentComponent as LayoutSchedule;
        if (layout == null)
            return;

        layout.SetScheduleData(schedule);
    }
}