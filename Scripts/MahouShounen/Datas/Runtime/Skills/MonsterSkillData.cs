using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "Data/Monster Skill", order = 42)]
public class MonsterSkillData : SkillData
{
    [SerializeField]
    private int _timer;
    public  int  timer
    {
        get { return _timer; }
    }
}
