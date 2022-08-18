using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task - Change Timer", order = 63)]
public class ChangeTimerSkillTaskData : SkillTaskData
{
    [Header("Skill Task - Change Timer")]
    [SerializeField]
    private int _modifier;
    public  int  modifier
    {
        get { return _modifier; }
    }
}
