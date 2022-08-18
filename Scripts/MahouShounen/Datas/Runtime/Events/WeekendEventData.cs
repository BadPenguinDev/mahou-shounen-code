using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weekend Event", menuName = "Data/Weekend Event", order = 21)]
public class WeekendEventData : EventData
{
    [SerializeField] 
    private WeekendEventOption _option;
    public  WeekendEventOption  option
    {
        get { return  _option; }
        set { _option = value; }
    }
}

[System.Serializable]
public class WeekendEventOption
{
    [SerializeField]
    private Spot _spotType;
    public  Spot  spotType
    {
        get { return  _spotType; }
        set { _spotType = value; }
    }

    [SerializeField]
    private Sprite _sprite;
    public  Sprite  sprite
    {
        get { return  _sprite; }
        set { _sprite = value; }
    }

    [SerializeField]
    private Vector2 _position;
    public  Vector2  position
    {
        get { return  _position; }
        set { _position = value; }
    }
}