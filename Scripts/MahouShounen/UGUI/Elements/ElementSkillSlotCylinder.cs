using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSkillSlotCylinder : ElementBase
{
    Skill skill;

    [SerializeField] Image imageButton;
    [SerializeField] Image imageSkillIcon;
    [SerializeField] Image imageRankIcon;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
    }

    public void SetSkill(Skill inSkill)
    {
        if (skill != null)
            skill.onSkillUpgraded.RemoveListener(OnSkillUpgraded);
        
        skill = inSkill;
        skill.onSkillUpgraded.AddListener(OnSkillUpgraded);

        var skillData = skill.data as PlayerSkillData;
        var paletteData = UGUIManager.Get().GetPaletteData(skillData.palette);

        imageButton.   sprite = paletteData.buttonSkillSprite;
        imageSkillIcon.sprite = skillData.iconSprite;
        
        OnSkillUpgraded(skill.rank);
    }

    public void OnSkillUpgraded(SkillRank rank)
    {
        imageRankIcon.sprite = MSDataManager.Get().globalPaletteData.spriteRankSkillIcon[(int)rank];
    }
}
