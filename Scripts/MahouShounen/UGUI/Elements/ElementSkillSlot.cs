using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSkillSlot : ElementBase
{
    Skill skill;

    public Toggle toggleSkillSlot;

    [SerializeField] Image imageButton;
    [SerializeField] Image imageField;
    [SerializeField] Image imageSkillIcon;
    [SerializeField] Image imageRankIcon;
    [SerializeField] Image imageOrderIcon;

    [SerializeField] Text textName;
    [SerializeField] OutlineColored outlineTextName;

    
    public void SetSkill(Skill inSkill)
    {
        if (skill != null)
            skill.onSkillUpgraded.RemoveListener(OnSkillUpgraded);
        
        skill = inSkill;
        skill.onSkillUpgraded.AddListener(OnSkillUpgraded);

        var skillData = skill.data as PlayerSkillData;
        var paletteData = UGUIManager.Get().GetPaletteData(skillData.palette);

        imageButton.   sprite = paletteData.buttonStandaloneSprite;
        imageField.    sprite = paletteData.fieldRoundSprite;
        imageSkillIcon.sprite = skillData.iconSprite;
        
        OnSkillUpgraded(skill.rank);

        if (MSGameInstance.Get().activeSkills.Contains(skill))
        {
            var order = MSGameInstance.Get().activeSkills.IndexOf(skill);
            
            imageOrderIcon.sprite = MSDataManager.Get().globalPaletteData.spriteOrderIcon[order];
            imageOrderIcon.gameObject.SetActive(true);
            
            toggleSkillSlot.SetIsOnWithoutNotify(true);
        }
        else
        {
            imageOrderIcon.gameObject.SetActive(false);
            
            toggleSkillSlot.SetIsOnWithoutNotify(false);
        }

        textName.text = skillData.skillName.GetLocalizedString();
        outlineTextName.effectColor = paletteData.outlineColor;
    }
    public Skill GetSkill() { return skill; }
    
    public void OnSkillUpgraded(SkillRank rank)
    {
        imageRankIcon.sprite = MSDataManager.Get().globalPaletteData.spriteRankSkillIcon[(int)rank];
    }
}
