using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ElementDreamflowReward : ElementBase
{
    public Image imageIconItem;
    public Text  textCount;

    public void SetRewardData(DreamflowRewardData data)
    {
        imageIconItem.sprite = data.item.sprite;
        textCount.text = data.amount.ToString();
    }
}
