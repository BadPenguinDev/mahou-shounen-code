using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Figure", menuName = "Data/Monster Figure", order = 102)]
public class MonsterFigureData : BattleFigureData
{
    [Header("Skills - Monster")]
    [SerializeField]
    private List<SkillData> _skillData;
    public  List<SkillData>  skillData
    {
        get { return _skillData; }
    }

    [SerializeField]
    private int _startTick;
    public  int  startTick
    {
        get { return _startTick; }
    }
}
