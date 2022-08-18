using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff", menuName = "Data/Buff - Change HP", order = 81)]
public class ChangeHPBuffData : BuffData
{
    [Header("Buff - ChangeHP")]
    [SerializeField]
    private int _value;
    public  int  value
    {
        get { return _value; }
    }
}
