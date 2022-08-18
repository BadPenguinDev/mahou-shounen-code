using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDailyStress : ElementBase
{
    public int value;
    public int modifier;

    public Text textStatValue;

    public Scrollbar scrollBarStat;
    public Image     scrollBarImage;


    public void SetStress(Schedule schedule)
    {
        // Set Schedule and Stat Data
        var targetScheduleData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == schedule);
        value = MSGameInstance.Get().playerStress;
        
        // Set Modifier
        var grade = MSGameInstance.Get().GetScheduleGrade(targetScheduleData);
        modifier = targetScheduleData.GetStress(grade);
        
        // Apply Element
        scrollBarStat.size = value / (float)MSCommon.MaxStressGauge;
        textStatValue.text = value.ToString();
        
        // Text Preset
        var scrollBarSize = scrollBarStat.size;
        UGUIManager.Get().SetScrollBarTextPreset(textStatValue, scrollBarSize);
    }

    public void UpdateStressResult(float time)
    {
        // Update Currencys
        var modifiedValue = value + Mathf.Lerp(0, modifier, time);
        if (modifiedValue < 0)
            modifiedValue = 0;

        scrollBarStat.size = modifiedValue / MSCommon.MaxStressGauge;
        textStatValue.text = ((int)modifiedValue).ToString();
    }
}
