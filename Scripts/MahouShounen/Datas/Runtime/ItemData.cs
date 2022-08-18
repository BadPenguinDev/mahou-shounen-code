using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[System.Serializable, CreateAssetMenu(fileName = "Item", menuName = "Data/Item", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField] 
    private int _id;
    public  int  id
    {
        get { return  _id; }
        set { _id = value; }
    }

    [SerializeField]
    private Sprite _sprite;
    public  Sprite  sprite
    {
        get { return  _sprite; }
        set { _sprite = value; }
    }
    
    [SerializeField]
    private LocalizedString _itemName;
    public  LocalizedString  itemName
    {
        get { return  _itemName; }
        set { _itemName = value; }
    }
    
    [SerializeField]
    private LocalizedString _itemDescription;
    public  LocalizedString  itemDescription
    {
        get { return  _itemDescription; }
        set { _itemDescription = value; }
    }
}

[System.Serializable]
public class Item
{
    public ItemData itemData;
    public int      itemCount;
    
    public Item(ItemSave save)
    {
        itemData  = MSDataManager.Get().itemDatas.Find(x => x.id == save.id);
        itemCount = save.count;
    }
    
    public Item(ItemData data, int count)
    {
        itemData  = data;
        itemCount = count;
    }

    public void ChangeItemCount(int value) { itemCount += value; }
}
