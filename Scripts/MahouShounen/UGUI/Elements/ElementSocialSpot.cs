using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;


public class ElementSocialSpot : ElementBase
{
    public WeekendEventData eventData;

    public LocalizeStringEvent localizeEventSpotName;
    public List<Image> baseImages;
    public List<OutlineColored> outlines;


    public void SetWeekendEvent(WeekendEventData eventData)
    {
        this.eventData = eventData;

        localizeEventSpotName.SetEntry(eventData.option.spotType.ToString());
        localizeEventSpotName.SetTable("Spots");
    }
}
