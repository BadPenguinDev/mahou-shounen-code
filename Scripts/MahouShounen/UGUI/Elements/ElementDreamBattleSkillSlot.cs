using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleSkillSlot : ElementBase
{
    Skill skill;
    public bool isSlotEmpty;

    Image imageButton;

    [SerializeField] Image imageSkillIcon;
    [SerializeField] Image imageRankIcon;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);

        imageButton = GetComponent<Image>();
    }

    public void SetSkillData(Skill skill)
    {
        var skillData = skill.data as PlayerSkillData;
        isSlotEmpty = false;

        var paletteData = UGUIManager.Get().GetPaletteData(skillData.palette);

        imageButton.   sprite = paletteData.buttonSkillSprite;
        imageSkillIcon.sprite = skillData.iconSprite;
        imageRankIcon. sprite = MSDataManager.Get().globalPaletteData.spriteRankSkillIcon[(int)skill.rank]; 
    }
    public void ClearSkillData()
    {
        isSlotEmpty = true;

        imageButton.   sprite = MSDataManager.Get().globalPaletteData.spriteTransparent;
        imageSkillIcon.sprite = MSDataManager.Get().globalPaletteData.spriteTransparent;
        imageRankIcon. sprite = MSDataManager.Get().globalPaletteData.spriteTransparent;
    }
}
