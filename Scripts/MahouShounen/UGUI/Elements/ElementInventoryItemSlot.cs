using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ElementInventoryItemSlot : ElementBase
{
    public Button buttonItemSlot;
    public Image  imageItemIcon;
    public Text   textItemCount;

    Item item;

    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        imageItemIcon.sprite = item.itemData.sprite;
        textItemCount.text   = item.itemCount.ToString();

        if (item.itemCount > 1)
            textItemCount.gameObject.SetActive(true);
        else
            textItemCount.gameObject.SetActive(false);
    }
    public Item GetItem() { return item; }

    public void OnButtonItemSlotClicked()
    {
        // LayoutInventory layout = component as LayoutInventory;
        // layout.SetElementItemSlotSelected(this);
    }
}
