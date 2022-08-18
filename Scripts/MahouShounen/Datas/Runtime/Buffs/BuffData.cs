using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Buff", menuName = "Data/Buff", order = 80)]
public class BuffData : ScriptableObject
{
    [Header("Buff - Common")]
    [SerializeField]
    private int _id;
    public  int  id
    {
        get { return _id; }
    }
    
    [SerializeField]
    private BuffType _type;
    public  BuffType  type
    {
        get { return _type; }
    }

    [SerializeField]
    private int _timer;
    public  int  timer
    {
        get { return _timer; }
    }
    
    [SerializeField]
    private Sprite _iconStatusMini;
    public  Sprite  iconStatusMini
    {
        get { return _iconStatusMini; }
    }
}
