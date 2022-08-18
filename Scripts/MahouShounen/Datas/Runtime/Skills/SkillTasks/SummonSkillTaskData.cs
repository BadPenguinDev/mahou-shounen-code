using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task - Summon", order = 64)]
public class SummonSkillTaskData : SkillTaskData
{
    [Header("Skill Task - Summon")]
    [SerializeField]
    private MinionFigureData _minion;
    public  MinionFigureData  minion
    {
        get { return _minion; }
    }
}
