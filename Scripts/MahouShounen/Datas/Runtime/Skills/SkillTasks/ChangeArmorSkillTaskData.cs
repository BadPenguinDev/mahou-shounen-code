using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task - Change Armor", order = 62)]
public class ChangeArmorSkillTaskData : SkillTaskData
{
    [Header("Skill Task - Change Armor")]
    [SerializeField]
    private int _modifier;
    public  int  modifier
    {
        get { return _modifier; }
    }
}
