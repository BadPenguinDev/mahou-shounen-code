using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Palette Data", menuName = "Global/Palette", order = 999)]
public class GlobalPaletteData : ScriptableObject
{
    public List<UIPaletteData> paletteDatas;
    public List<SchedulePalette> schedulePalettes;
    
    public List<Sprite> spriteRankIcon;
    public List<Sprite> spriteRankSkillIcon;
    public List<Sprite> spriteOrderIcon;
    public List<Sprite> spriteGradeScheduleIcon;
    
    public List<Sprite> spriteCasualwearIcon;
    public List<Sprite> spriteCostumeIcon;
    
    public List<ScrollBarTextPreset> scrollBarTextPresets;
    public Sprite spriteTransparent;
}


[System.Serializable]
public class UIPaletteData
{
    public MSUGUIPalette type;

    public Sprite buttonRoundSprite;
    public Sprite buttonSquareSprite;
    public Sprite buttonStandaloneSprite;
    public Sprite buttonSkillSprite;

    public Sprite boxRoundSprite;
    public Sprite boxSquareSprite;

    public Sprite fieldRoundSprite;
    public Sprite fieldSquareSprite;

    public Color outlineColor;
}

[System.Serializable]
public class SchedulePalette
{
    public ScheduleType  type;
    public MSUGUIPalette palette;
}

[System.Serializable]
public class ScrollBarTextPreset
{
    public float size;
    public Vector2 position;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 pivot;
    public TextAnchor alignment;
}
