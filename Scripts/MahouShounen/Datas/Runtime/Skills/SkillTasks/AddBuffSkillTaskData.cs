using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task - Add Buff", order = 65)]
public class AddBuffSkillTaskData : SkillTaskData
{
    [Header("Skill Task - Add Buff")]
    [SerializeField]
    private BuffData _buff;
    public  BuffData  buff
    {
        get { return _buff; }
    }
}
