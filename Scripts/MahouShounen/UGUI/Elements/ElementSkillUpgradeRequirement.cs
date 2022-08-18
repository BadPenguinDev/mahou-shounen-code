using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSkillUpgradeRequirement : ElementBase
{
    public Image imageIconItem;
    public Text  textCount;
    public OutlineColored outlineCount;

    public void SetRequirementData(SkillUpgradeRequirementData data)
    {
        imageIconItem.sprite = data.itemData.sprite;

        var itemCount = MSGameInstance.Get().GetItemCountByItemData(data.itemData);
        var requireCount = data.itemCount;
        
        textCount.text = itemCount.ToString() + "/" + requireCount.ToString();
        
        // Set Outline Color
        UIPaletteData paletteData = UGUIManager.Get().GetPaletteData(itemCount < requireCount ? MSUGUIPalette.Negative : MSUGUIPalette.Positive);
        outlineCount.effectColor = paletteData.outlineColor;
    }
}
