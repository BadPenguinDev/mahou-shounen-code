using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;


public class ElementStatSchedule : ElementBase
{
    [Header("Schedule")]
    public ScheduleData  schedule;
    public ScheduleGrade grade;

    [Header("Components")]
    public Image imageIconSchedule;
    public LocalizeStringEvent localizeEventScheduleName;
    public Text textCredit;

    public List<Image>  baseImages;
    public List<Image> fieldImages;
    public List<OutlineColored> outlines;

    private Button buttonStatSchedule;
    
    
    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
        
        buttonStatSchedule = GetComponent<Button>();
        buttonStatSchedule.onClick.AddListener(OnScheduleSelectToggleClicked);
    }
    
    public void SetScheduleData(ScheduleData scheduleData)
    {
        schedule = scheduleData;

        grade = MSGameInstance.Get().GetScheduleGrade(schedule);

        imageIconSchedule.sprite = schedule.spriteSchedule;

        localizeEventScheduleName.SetEntry(schedule.keyScheduleName);
        localizeEventScheduleName.SetTable("Schedules");

        textCredit.text = schedule.GetCost(grade).ToString();

        var paletteData = UGUIManager.Get().GetSchedulePaletteData(schedule.type);
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
    }
    public void OnScheduleSelectToggleClicked()
    {
        var popUp = UGUIManager.Get().OpenPopUp(PopUpType.ScheduleInfo) as PopUpStatScheduleInfo;
        popUp.SetScheduleData(schedule);
    }
}
