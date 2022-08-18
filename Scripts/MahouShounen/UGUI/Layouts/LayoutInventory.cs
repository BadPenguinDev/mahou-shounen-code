using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class LayoutInventory : LayoutBase
{
    public GameObject prefabElementItemSlot;

    public GameObject buttonReturn;

    // Item List
    public RectTransform contentItemSlots;

    // Item Info
    public Image imageItemIcon;
    public Text textItemName;
    public Text textItemDescription;

    Item targetItem;
    List<Item> items;

    List<ElementInventoryItemSlot>   activeElementItemSlots;
    List<ElementInventoryItemSlot> inactiveElementItemSlots;

    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        buttonReturn.GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);

          activeElementItemSlots = new List<ElementInventoryItemSlot>();
        inactiveElementItemSlots = new List<ElementInventoryItemSlot>();
    }

    public override void OpenLayout(string layoutName)
    {
        base.OpenLayout(layoutName);

        float elementWidth  = -1;
        float elementHeight = -1;

        // Clear Active Element List
        for (var i = 0; i < activeElementItemSlots.Count; i++)
        {
            activeElementItemSlots[i].gameObject.SetActive(false);
            inactiveElementItemSlots.Add(activeElementItemSlots[i]);
        }
        activeElementItemSlots.Clear();

        // Add Item Element
        items = MSGameInstance.Get().items;
        for (var i = 0; i < items.Count; i++)
        {
            ElementInventoryItemSlot element;
            RectTransform elementTransform;

            if (inactiveElementItemSlots.Count > 0)
            {
                element = inactiveElementItemSlots[0];
                element.SetItem(items[i]);
                element.gameObject.SetActive(true);

                elementTransform = element.GetComponent<RectTransform>();

                inactiveElementItemSlots.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabElementItemSlot, contentItemSlots);

                element = instance.GetComponent<ElementInventoryItemSlot>();
                element.SetParentComponent(this);
                element.SetItem(items[i]);
                element.buttonItemSlot.onClick.AddListener(delegate { OnButtonItemSlotClicked(element); });

                elementTransform = element.GetComponent<RectTransform>();
            }
            activeElementItemSlots.Add(element);

            if (elementWidth  == -1) 
                elementWidth  = elementTransform.sizeDelta.x + 2f;
            if (elementHeight == -1) 
                elementHeight = elementTransform.sizeDelta.y + 2f;

            var elementPosX = i % 4;
            var elementPosY = i / 4;

            element.SetItem(items[i]);
            elementTransform.anchoredPosition = new Vector2(3f + (elementWidth * elementPosX), -3f + (elementHeight * elementPosY));
        }
        contentItemSlots.sizeDelta = Vector2.zero;
        contentItemSlots.sizeDelta += new Vector2(0f, 2f + elementHeight * (items.Count + 1));
    }

    public void OnButtonItemSlotClicked(ElementInventoryItemSlot element)
    {
        var item = element.GetItem();

        imageItemIcon.sprite = item.itemData.sprite;

        textItemName.text        = item.itemData.itemName.GetLocalizedString();
        textItemDescription.text = item.itemData.itemDescription.GetLocalizedString();
    }

    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
}
