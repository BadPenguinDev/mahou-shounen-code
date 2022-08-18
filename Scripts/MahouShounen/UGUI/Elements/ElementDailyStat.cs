using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ElementDailyStat : ElementBase
{
    public Stat stat;
    public int value;
    public int modifier;

    public LocalizeStringEvent localizeEventStatName;
    public Text textStatValue;

    public Scrollbar scrollBarStat;
    public Image     scrollBarImage;

    public Image imageStatRank;
    public GameObject plateStatLock;

    // Private
    private StatRank rank;


    public void SetStat(Schedule schedule, Stat stat)
    {
        this.stat = stat;

        // Set Schedule and Stat Data
        var targetScheduleData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == schedule);
        var statBase = MSGameInstance.Get().GetStatBase(stat);

        rank  = statBase.rank;
        value = statBase.value;

        // Set Modifier
        var grade = MSGameInstance.Get().GetScheduleGrade(targetScheduleData);
        var statData = targetScheduleData.GetStats(grade).Find(x => x.type == stat);

        modifier = statData.modifier;

        // Get Palette
        UIPaletteData palette;
        if (modifier >= 0)
            palette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Positive);
        else
            palette = MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == MSUGUIPalette.Negative);

        var rankSprite = MSDataManager.Get().globalPaletteData.spriteRankIcon[(int)rank];

        // Apply Element
        imageStatRank. sprite = rankSprite;
        scrollBarImage.sprite = palette.buttonSquareSprite;

        scrollBarImage.GetComponent<OutlineColored>().effectColor = palette.outlineColor;
        textStatValue. GetComponent<OutlineColored>().effectColor = palette.outlineColor;

        localizeEventStatName.SetEntry(stat.ToString());
        localizeEventStatName.SetTable("Stats");
        
        // Stat Gauge
        if (rank == StatRank.S)
        {
            textStatValue.gameObject.SetActive(false);
            scrollBarStat.size = 1.0f;
        }
        else
        {
            var upgradeValue = MSCommon.GetStatRankUpgradeValue(rank);
            scrollBarStat.size = value / (float)upgradeValue;
            textStatValue.text = value.ToString();
        }
        
        // Text Preset
        var scrollBarSize = scrollBarStat.size;
        UGUIManager.Get().SetScrollBarTextPreset(textStatValue, scrollBarSize);

        // Stat Lock
        if (statBase.isLocked)
        {
            plateStatLock.SetActive(true);
            
            scrollBarStat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -15f);
            scrollBarStat.GetComponent<RectTransform>().sizeDelta = new Vector2(-17f, 15f);
        }
        else
        {
            plateStatLock.SetActive(false);
            
            scrollBarStat.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 15f);
            scrollBarStat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -15f);
        }
    }

    public void UpdateStatResult(float time)
    {
        if (rank == StatRank.S)
            return;

        if (MSGameInstance.Get().playerStats[stat].isLocked)
            return;
        
        // Update Currencys
        var minimumStatLimit = MSCommon.GetStatRankStartValue(rank);
        var modifiedValue = value + Mathf.Lerp(0, modifier, time);
        if (modifiedValue < minimumStatLimit)
            modifiedValue = minimumStatLimit;

        float upgradeValue = MSCommon.GetStatRankUpgradeValue(rank);
        scrollBarStat.size = modifiedValue / upgradeValue;
        textStatValue.text = ((int)modifiedValue).ToString();
    }
}
