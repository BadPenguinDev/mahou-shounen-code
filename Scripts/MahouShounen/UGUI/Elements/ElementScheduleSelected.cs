using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

public class ElementScheduleSelected : ElementBase
{
    [Header("Schedule")]
    public ScheduleData schedule;
    public Day day;

    [Header("Components")]
    public GameObject selectEffect;
    public GameObject panelScheduleSelect;

    public LocalizeStringEvent localizeEventScheduleName;
    public List<Image> baseImages;
    public List<OutlineColored> outlines;

    public void SetScheduleData(ScheduleData data, bool isForced = false)
    {
        if (data == null)
        {
            panelScheduleSelect.SetActive(false);
            return;
        }

        if (!isForced &&
            !MSGameInstance.Get().IsScheduleAvailable(data))
        {
            panelScheduleSelect.SetActive(false);
            return;
        }

        // Set Schedule
        schedule = data;

        panelScheduleSelect.SetActive(true);

        localizeEventScheduleName.SetEntry(data.keyScheduleName);
        localizeEventScheduleName.SetTable("Schedules");

        var paletteData = UGUIManager.Get().GetSchedulePaletteData(data.type, true);
        foreach (var image in baseImages)
        {
            image.sprite = paletteData.buttonSquareSprite;
        }
        foreach (var outline in outlines)
        {
            outline.effectColor = paletteData.outlineColor;
        }
    }
    public void SetSelectEffectActive(bool status)
    {
        selectEffect.SetActive(status);
    }

    public bool IsScheduleAvailable()
    {
        if (schedule == null)
            return false;
        
        return MSGameInstance.Get().IsScheduleAvailable(schedule);
    }

    public void OnScheduleSelectedClicked()
    {
        var layout = parentComponent as LayoutSchedule;
        if (layout == null)
            return;

        SetScheduleData(layout.targetScheduleData);
        layout.SetButtonPlaySchedulePalette();
    }
}
