using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

public class LayoutDressRoom : LayoutBase
{
    public GameObject prefabElementCostumeSlot;
    
    public Button buttonReturn;
    
    // Portraits
    public Image imageClothing;
    
    // Dress Room List
    public RectTransform contentItemSlots;

    public Toggle tabCasualwear;
    public Toggle tabCostume;

    List<ElementDressRoomSlot>   activeElementCostumeSlots;
    List<ElementDressRoomSlot> inactiveElementCostumeSlots;
    
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        
        tabCasualwear.onValueChanged.AddListener(SetClothingTypeCasualwear);
        tabCostume.   onValueChanged.AddListener(SetClothingTypeCostume);

          activeElementCostumeSlots = new List<ElementDressRoomSlot>();
        inactiveElementCostumeSlots = new List<ElementDressRoomSlot>();
    }

    public override void OpenLayout(string layoutName)
    {
        base.OpenLayout(layoutName);

        if (MSGameInstance.Get().layoutMainMode == LayoutMainMode.WeekendNight)
        {
            SetClothingTypeCostume(true);
            tabCostume.SetIsOnWithoutNotify(true);
        }
        else
        {
            SetClothingTypeCasualwear(true);
            tabCasualwear.SetIsOnWithoutNotify(true);
        }
    }
    
    public void SetClothingTypeCasualwear(bool status)
    {
        tabCostume.SetIsOnWithoutNotify(false);
        
        float elementWidth  = -1;
        float elementHeight = -1;

        // Clear Active Element List
        foreach (var element in activeElementCostumeSlots)
        {
            element.gameObject.SetActive(false);
            inactiveElementCostumeSlots.Add(element);
        }
        activeElementCostumeSlots.Clear();

        // Add Costume Element
        var casualwears = MSGameInstance.Get().unlockedCasualwears;
        for (var i = 0; i < casualwears.Count; i++)
        {
            ElementDressRoomSlot element;
            RectTransform elementTransform;

            if (inactiveElementCostumeSlots.Count > 0)
            {
                element = inactiveElementCostumeSlots[0];
                element.gameObject.SetActive(true);

                elementTransform = element.GetComponent<RectTransform>();

                inactiveElementCostumeSlots.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabElementCostumeSlot, contentItemSlots);

                element = instance.GetComponent<ElementDressRoomSlot>();
                element.SetParentComponent(this);
                element.button.onClick.AddListener(delegate { OnButtonItemSlotClicked(element); });

                elementTransform = element.GetComponent<RectTransform>();
            }
            activeElementCostumeSlots.Add(element);

            if (elementWidth  == -1) 
                elementWidth  = elementTransform.sizeDelta.x + 2f;
            if (elementHeight == -1) 
                elementHeight = elementTransform.sizeDelta.y + 2f;

            var elementPosX = i % 4;
            var elementPosY = i / 4;

            element.SetCasualwearType(casualwears[i]);
            elementTransform.anchoredPosition = new Vector2(3f + (elementWidth * elementPosX), -3f + (elementHeight * elementPosY));
        }
        contentItemSlots.sizeDelta = Vector2.zero;
        contentItemSlots.sizeDelta += new Vector2(0f, 2f + elementHeight * (casualwears.Count + 1));

        var targetElement = activeElementCostumeSlots.Find(x => x.casualwear == MSGameInstance.Get().casualwear);
        OnButtonItemSlotClicked(targetElement);
    }
    public void SetClothingTypeCostume(bool status)
    {
        tabCasualwear.SetIsOnWithoutNotify(false);
        
        float elementWidth  = -1;
        float elementHeight = -1;

        // Clear Active Element List
        foreach (var element in activeElementCostumeSlots)
        {
            element.gameObject.SetActive(false);
            inactiveElementCostumeSlots.Add(element);
        }
        activeElementCostumeSlots.Clear();

        // Add Costume Element
        var costumes = MSGameInstance.Get().unlockedCostumes;
        for (var i = 0; i < costumes.Count; i++)
        {
            ElementDressRoomSlot element;
            RectTransform elementTransform;

            if (inactiveElementCostumeSlots.Count > 0)
            {
                element = inactiveElementCostumeSlots[0];
                element.gameObject.SetActive(true);

                elementTransform = element.GetComponent<RectTransform>();

                inactiveElementCostumeSlots.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabElementCostumeSlot, contentItemSlots);

                element = instance.GetComponent<ElementDressRoomSlot>();
                element.SetParentComponent(this);
                element.button.onClick.AddListener(delegate { OnButtonItemSlotClicked(element); });

                elementTransform = element.GetComponent<RectTransform>();
            }
            activeElementCostumeSlots.Add(element);

            if (elementWidth  == -1) 
                elementWidth  = elementTransform.sizeDelta.x + 2f;
            if (elementHeight == -1) 
                elementHeight = elementTransform.sizeDelta.y + 2f;

            var elementPosX = i % 4;
            var elementPosY = i / 4;

            element.SetCostumeType(costumes[i]);
            elementTransform.anchoredPosition = new Vector2(3f + (elementWidth * elementPosX), -3f + (elementHeight * elementPosY));
        }
        contentItemSlots.sizeDelta = Vector2.zero;
        contentItemSlots.sizeDelta += new Vector2(0f, 2f + elementHeight * (costumes.Count + 1));

        var targetElement = activeElementCostumeSlots.Find(x => x.costume == MSGameInstance.Get().costume);
        OnButtonItemSlotClicked(targetElement);
    }
    
    public void OnButtonItemSlotClicked(ElementDressRoomSlot element)
    {
        var clothingType = element.clothingType;
        if (clothingType == PlayerClothingType.CasualWear)
        {
            MSGameInstance.Get().casualwear = element.casualwear;
            imageClothing.sprite = MSDataManager.Get().playerCharacterData.casualWearSprites[(int)element.casualwear];
        }
        else
        {
            MSGameInstance.Get().costume = element.costume;
            imageClothing.sprite = MSDataManager.Get().playerCharacterData.costumeSprites[(int)element.costume];
        }
    }
    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
}
