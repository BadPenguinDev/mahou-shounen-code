using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutSkill : LayoutBase
{
    public Button buttonReturn;

    [Header("Skill List")]
    public GameObject prefabElementSkillSlot;
    public RectTransform contentSkillSlots;

    [Header("Skill Info")]
    public Image imageSkillIcon;
    public Image imageRankIcon;
    public Scrollbar scrollBarEXP;
    public Text textEXPValue;
    public Text textName;
    public Text textDescription;
    public Button buttonUpgradeSkill;
    public Button buttonSetSkill;
    public Button buttonRemoveSkill;

    [Header("Skill Cylinder")]
    public List<GameObject> imageSkillBullets;
    public GameObject prefabElementSkillSlotCylinder;
    public RectTransform panelSkillSlot;

    int maxCylinderSlots;
    Skill selectedSkill;

    List<Skill> skills;
    List<Skill> activeSkills;

    List<ElementSkillSlot>   activeElementSkillSlots;
    List<ElementSkillSlot> inactiveElementSkillSlots;

    List<Image> imageCylinderBullets;
    List<ElementSkillSlotCylinder> elementSkillSlotCylinders;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        buttonReturn.onClick.AddListener(OnButtonReturnClicked);

        buttonUpgradeSkill.onClick.AddListener(OnButtonUpgradeSkillClicked);
        
        buttonSetSkill.   onClick.AddListener(OnButtonSetSkillClicked);
        buttonRemoveSkill.onClick.AddListener(OnButtonRemoveSkillClicked);

          activeElementSkillSlots = new List<ElementSkillSlot>();
        inactiveElementSkillSlots = new List<ElementSkillSlot>();

        elementSkillSlotCylinders = new List<ElementSkillSlotCylinder>();

        for (var i = 0; i < 6; i++)
        {
            var instance = Instantiate(prefabElementSkillSlotCylinder, panelSkillSlot);

            var element = instance.GetComponent<ElementSkillSlotCylinder>();
            element.SetParentComponent(this);

            var elementTransform = element.GetComponent<RectTransform>();
            elementTransform.anchoredPosition += new Vector2(i * 32, 0f);

            instance.SetActive(false);

            elementSkillSlotCylinders.Add(element);
        }
    }
    public override void OpenLayout(string layoutName = "On")
    {
        base.OpenLayout(layoutName);

        // Skill Index
        maxCylinderSlots = MSGameInstance.Get().maxCylinderSlots;

        // Set Active Skill Element
        activeSkills = MSGameInstance.Get().activeSkills;
        for (var i = 0; i < activeSkills.Count; i++)
        {
            elementSkillSlotCylinders[i].gameObject.SetActive(true);
            elementSkillSlotCylinders[i].SetSkill(activeSkills[i]);
        }

        // Update Skill Element
        UpdateSkillSlotElements();
        
        OnButtonSkillSlotClicked(activeElementSkillSlots[0]);
    }

    public void UpdateSkillSlotElements()
    {
        foreach (var element in activeElementSkillSlots)
        {
            element.gameObject.SetActive(false);
            inactiveElementSkillSlots.Add(element);
        }
        activeElementSkillSlots.Clear();
        
        float elementHeight = -1;
        var elementPos = prefabElementSkillSlot.GetComponent<RectTransform>().anchoredPosition;

        var   activeSkills = new List<Skill>(MSGameInstance.Get().activeSkills);
        var deactiveSkills = new List<Skill>(MSGameInstance.Get().playerSkills);
        foreach (var skill in activeSkills)
        {
            deactiveSkills.Remove(skill);
        }
        
        skills = new List<Skill>();
        skills.AddRange(activeSkills);
        skills.AddRange(deactiveSkills);
        
        for (var i = 0; i < skills.Count; i++)
        {
            ElementSkillSlot element;
            RectTransform elementTransform;

            if (inactiveElementSkillSlots.Count > 0)
            {
                element = inactiveElementSkillSlots[0];
                element.gameObject.SetActive(true);

                inactiveElementSkillSlots.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabElementSkillSlot, contentSkillSlots);

                element = instance.GetComponent<ElementSkillSlot>();
                element.SetParentComponent(this);

                element.toggleSkillSlot.onValueChanged.AddListener(delegate { OnButtonSkillSlotClicked(element); });
            }
            activeElementSkillSlots.Add(element);

            element.SetSkill(skills[i]);
            elementTransform = element.GetComponent<RectTransform>();

            if (elementHeight == -1)
                elementHeight = elementTransform.sizeDelta.y + 4f;

            elementTransform.anchoredPosition = elementPos - new Vector2(0f, elementHeight * i);
        }
        contentSkillSlots.sizeDelta = new Vector2(0f, 2f + elementHeight * (skills.Count));
        
        // Bullets
        for (var i = 0; i < imageSkillBullets.Count; i++)
        {
            imageSkillBullets[i].SetActive(i < activeSkills.Count);
        }
    }

    private void OnButtonSkillSlotClicked(ElementSkillSlot element)
    {
        element.toggleSkillSlot.SetIsOnWithoutNotify(MSGameInstance.Get().activeSkills.Contains(element.GetSkill()));
        
        selectedSkill = element.GetSkill();
        UpdateSelectedSkillData();
    }

    public void UpdateSelectedSkillData()
    {
        var skillData = selectedSkill.data as PlayerSkillData;
        var maxExpValue = selectedSkill.GetExpValue();

        imageSkillIcon.sprite = skillData.iconSprite;
        imageRankIcon.sprite = MSDataManager.Get().globalPaletteData.spriteRankIcon[(int)selectedSkill.rank];
        scrollBarEXP.size = selectedSkill.exp / (float)maxExpValue;
        textEXPValue.text = selectedSkill.exp.ToString() + "/" + maxExpValue.ToString();
        textName.text = skillData.skillName.GetLocalizedString();
        textDescription.text = skillData.skillDesc.GetLocalizedString();

        buttonUpgradeSkill.gameObject.SetActive(selectedSkill.exp == maxExpValue);

        if (activeSkills.Contains(selectedSkill))
        {
            buttonSetSkill.gameObject.SetActive(false);
            buttonRemoveSkill.gameObject.SetActive(true);
        }
        else
        {
            buttonSetSkill.gameObject.SetActive(true);
            buttonRemoveSkill.gameObject.SetActive(false);
        }
    }

    private void OnButtonUpgradeSkillClicked()
    {
        var popUp = UGUIManager.Get().OpenPopUp(PopUpType.SkillUpgrade) as PopUpSkillUpgrade;
        popUp.SetSkill(selectedSkill);
    }
    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
    void OnButtonSetSkillClicked()
    {
        if (MSGameInstance.Get().activeSkills.Count < 6)
            MSGameInstance.Get().activeSkills.Add(selectedSkill);
        else
            MSGameInstance.Get().activeSkills[5] = selectedSkill;

        for (var i = 0; i < 6; i++)
        {
            elementSkillSlotCylinders[i].gameObject.SetActive(false);
        }

        activeSkills = MSGameInstance.Get().activeSkills;
        for (var i = 0; i < activeSkills.Count; i++)
        {
            elementSkillSlotCylinders[i].gameObject.SetActive(true);
            elementSkillSlotCylinders[i].SetSkill(activeSkills[i]);
        }

        UpdateSkillSlotElements();
        
        // Activate set and remove button.
        buttonSetSkill.gameObject.SetActive(false);
        buttonRemoveSkill.gameObject.SetActive(true);
    }
    void OnButtonRemoveSkillClicked()
    {
        // Set Active Skill Element
        MSGameInstance.Get().activeSkills.Remove(selectedSkill);

        for (var i = 0; i < 6; i++)
        {
            elementSkillSlotCylinders[i].gameObject.SetActive(false);
        }

        activeSkills = MSGameInstance.Get().activeSkills;
        for (var i = 0; i < activeSkills.Count; i++)
        {
            elementSkillSlotCylinders[i].gameObject.SetActive(true);
            elementSkillSlotCylinders[i].SetSkill(activeSkills[i]);
        }

        UpdateSkillSlotElements();
        
        // Activate set and remove button. 
        buttonSetSkill.gameObject.SetActive(true);
        buttonRemoveSkill.gameObject.SetActive(false);
    }
}
