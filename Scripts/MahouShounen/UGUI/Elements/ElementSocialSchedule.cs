using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;


public class ElementSocialSchedule : ElementBase
{
    public Schedule schedule;
    public Day day;

    public LocalizeStringEvent localizeEventScheduleName;
    public Text textScheduleName;
    public List<Image> baseImages;
    public List<OutlineColored> outlines;


    public void SetSchedule(Schedule schedule)
    {
        this.schedule = schedule;

        localizeEventScheduleName.SetEntry(schedule.ToString());
        localizeEventScheduleName.SetTable("Schedules");

        var type = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == schedule).type;

        var paletteData = UGUIManager.Get().GetSchedulePaletteData(type);
        foreach (var image in baseImages)
        {
            image.sprite = paletteData.boxSquareSprite;
        }
        foreach (var outline in outlines)
        {
            outline.effectColor = paletteData.outlineColor;
        }
    }
}
