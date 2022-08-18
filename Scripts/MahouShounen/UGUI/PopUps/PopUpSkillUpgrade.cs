using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSkillUpgrade : PopUpBase
{
    public GameObject prefabRewardElement;
    
    public Image imageSkillIcon;
    
    public Image imageRankIconBefore;
    public Image imageRankIconAfter;
    
    public Text textName;
    public Text textDescription;
    
    public Button buttonReturn;
    public Button buttonUpgrade;
    public OutlineColored outlineTextUpgrade;
    
    public RectTransform contentRequirementSlots;


    private Skill skill;
    
    private List<ElementSkillUpgradeRequirement>   activeRequirementElements;
    private List<ElementSkillUpgradeRequirement> inactiveRequirementElements;
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
          activeRequirementElements = new List<ElementSkillUpgradeRequirement>();
        inactiveRequirementElements = new List<ElementSkillUpgradeRequirement>();
        
        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        buttonUpgrade.onClick.AddListener(OnButtonUpgradeClicked);
    }
    
    public void SetSkill(Skill targetSkill)
    {
        skill = targetSkill;

        var skillData = skill.data as PlayerSkillData;

        imageSkillIcon.sprite = skillData.iconSprite;
        
        imageRankIconBefore.sprite = MSDataManager.Get().globalPaletteData.spriteRankIcon[(int)skill.rank];
        imageRankIconAfter. sprite = MSDataManager.Get().globalPaletteData.spriteRankIcon[(int)skill.rank - 1];
        
        textName.text = skillData.skillName.GetLocalizedString();
        textDescription.text = skillData.skillDesc.GetLocalizedString();
        
        contentRequirementSlots.sizeDelta = new Vector2(3f, 0);

        foreach (var element in activeRequirementElements)
        {
            element.gameObject.SetActive(false);
            inactiveRequirementElements.Add(element);
        }
        activeRequirementElements.Clear();
        
        var paletteData = UGUIManager.Get().GetPaletteData(MSUGUIPalette.Pointed);
        buttonUpgrade.interactable = true;
        
        var requirements = skill.GetUpgradeRequirements();
        for (var i = 0; i < requirements.Count; i++)
        {
            var itemCount = MSGameInstance.Get().GetItemCountByItemData(requirements[i].itemData);
            var requireCount = requirements[i].itemCount;
            if (itemCount < requireCount)
            {
                paletteData = UGUIManager.Get().GetPaletteData(MSUGUIPalette.Disabled);
                buttonUpgrade.interactable = false;
            }
            
            ElementSkillUpgradeRequirement elementRequirement;
            RectTransform elementTransform;

            if (inactiveRequirementElements.Count > 0)
            {
                elementRequirement = inactiveRequirementElements[0];
                elementRequirement.gameObject.SetActive(true);
                elementTransform = elementRequirement.GetComponent<RectTransform>();

                inactiveRequirementElements.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabRewardElement, contentRequirementSlots);

                elementRequirement = instance.GetComponent<ElementSkillUpgradeRequirement>();
                elementRequirement.SetParentComponent(this);

                elementTransform = elementRequirement.GetComponent<RectTransform>();
            }
            activeRequirementElements.Add(elementRequirement);

            var elementWidth = elementTransform.sizeDelta.x + 3f;

            elementRequirement.SetRequirementData(requirements[i]);
            elementTransform.anchoredPosition = new Vector2(3f + (elementWidth * i), 0f);

            contentRequirementSlots.sizeDelta += new Vector2(elementWidth, 0f);
        }
        contentRequirementSlots.anchoredPosition = Vector2.zero;
        
        // Set button interactable by currentExp / maxExp;
        if (skill.exp < skill.GetExpValue())
        {
            paletteData = UGUIManager.Get().GetPaletteData(MSUGUIPalette.Disabled);
            buttonUpgrade.interactable = false;
        }
        
        buttonUpgrade.GetComponent<Image>().sprite = paletteData.buttonStandaloneSprite;
        outlineTextUpgrade.effectColor = paletteData.outlineColor;
    }
    
    void OnButtonReturnClicked()
    {
        ClosePopUp();
    }
    void OnButtonUpgradeClicked()
    {
        skill.UpgradeSkill();

        // Skill Layout
        var layout = manager.GetLayout(LayoutType.Skill) as LayoutSkill;
        layout.UpdateSelectedSkillData();
        
        var requirements = skill.GetUpgradeRequirements();
        foreach (var requirement in requirements)
        {
            MSGameInstance.Get().ChangeItemCount(new Item(requirement.itemData, -requirement.itemCount));
        }
        
        // Palette
        SetSkill(skill);
    }
}
