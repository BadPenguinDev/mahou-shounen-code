using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Skill", menuName = "Data/Player Skill", order = 41)]
public class PlayerSkillData : SkillData
{
    [SerializeField]
    private List<PlayerSkillRankData> _rankDatas;
    public  List<PlayerSkillRankData>  rankDatas
    {
        get { return _rankDatas; }
    }

    [SerializeField]
    private Sprite _iconSprite;
    public  Sprite  iconSprite
    {
        get { return _iconSprite; }
    }

    [SerializeField]
    private MSUGUIPalette _palette;
    public  MSUGUIPalette  palette
    {
        get { return _palette; }
    }

    [SerializeField]
    private LocalizedString _skillName;
    public  LocalizedString  skillName
    {
        get { return _skillName; }
    }

    [SerializeField]
    private LocalizedString _skillDesc;
    public  LocalizedString  skillDesc
    {
        get { return _skillDesc; }
    }
}

[System.Serializable]
public class PlayerSkillRankData
{
    [SerializeField]
    private SkillRank _rank;
    public  SkillRank  rank
    {
        get { return _rank; }
    }

    [SerializeField]
    private int _EXP;
    public  int  EXP
    {
        get { return _EXP; }
    }

    [SerializeField]
    private List<SkillTaskData> _tasks;
    public  List<SkillTaskData>  tasks
    {
        get { return _tasks; }
    }

    [SerializeField]
    private int _heatValue;
    public  int  heatValue
    {
        get { return _heatValue; }
    }
    
    [SerializeField]
    private List<SkillUpgradeRequirementData> _upgradeRequirements;
    public  List<SkillUpgradeRequirementData>  upgradeRequirements
    {
        get { return _upgradeRequirements; }
    }
}

[System.Serializable]
public class SkillUpgradeRequirementData
{
    [SerializeField]
    private ItemData _itemData;
    public  ItemData  itemData
    {
        get { return _itemData; }
    }
    
    [SerializeField]
    private int _itemCount;
    public  int  itemCount
    {
        get { return _itemCount; }
    }
}