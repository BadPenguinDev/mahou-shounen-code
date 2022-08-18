using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDressRoomSlot : ElementBase
{
    public PlayerClothingType clothingType;
        
    public CasualwearType casualwear;
    public CostumeType    costume;

    public Button button;
    public Image icon;
    
    public void SetCasualwearType(CasualwearType type)
    {
        clothingType = PlayerClothingType.CasualWear;
        casualwear = type;
        
        icon.sprite = MSDataManager.Get().globalPaletteData.spriteCasualwearIcon[(int)type];
    }

    public void SetCostumeType(CostumeType type)
    {
        clothingType = PlayerClothingType.Costume;
        costume = type;
        
        icon.sprite = MSDataManager.Get().globalPaletteData.spriteCostumeIcon[(int)type];
    }
}
