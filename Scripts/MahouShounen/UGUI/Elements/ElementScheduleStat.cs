using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ElementScheduleStat : ElementBase
{
    public Stat stat;
    public int  value;

    public LocalizeStringEvent localizeEventStatName;
    public Text textStatValue;


    public void SetStat(Stat stat, int value)
    {
        this.stat  = stat;
        this.value = value;

        localizeEventStatName.SetEntry(stat.ToString());
        localizeEventStatName.SetTable("Stats");

        textStatValue.text = (value > 0) ? "+" + value.ToString() : 
                                                 value.ToString();
    }
}
