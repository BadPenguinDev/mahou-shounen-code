using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ElementStatInfo : ElementBase
{
    public Stat stat;
    int value;
    int modifier;

    public LocalizeStringEvent localizeEventStatName;
    public Text textStatValue;

    public Scrollbar scrollBarStat;
    public Image     scrollBarImage;

    public Image imageStatRank;
    public Toggle toggleLock;

    // Private
    private StatRank rank;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
        
        localizeEventStatName.SetEntry(stat.ToString());
        localizeEventStatName.SetTable("Stats");

        MSGameInstance.Get().onStatChanged.AddListener(UpdateStat);
        
        toggleLock.onValueChanged.AddListener(OnToggleLockValueChanged);
        toggleLock.SetIsOnWithoutNotify(MSGameInstance.Get().playerStats[stat].isLocked);
        
        UpdateStat(stat, MSGameInstance.Get().GetStatBase(stat));
    }

    private void UpdateStat(Stat statType, StatBase statBase)
    {
        if (stat != statType)
            return;
        
        rank  = statBase.rank;
        value = statBase.value;
        
        // Get Palette
        imageStatRank.sprite = MSDataManager.Get().globalPaletteData.spriteRankIcon[(int)rank];

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
    }

    private void OnToggleLockValueChanged(bool state)
    {
        MSGameInstance.Get().playerStats[stat].isLocked = state;
    }
}

