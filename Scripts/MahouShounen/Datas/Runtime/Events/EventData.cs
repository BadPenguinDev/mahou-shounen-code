using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum
public enum EventConditionType { Date, Stat, Schedule, Friendship, Event }
public enum  ConstantCondition { Less, LessEqual, Equal, Greater, GreaterEqual, NotEqual }
public enum    EventComplition { Completed, NotCompleted }


[CreateAssetMenu(fileName = "Event", menuName = "Data/Event", order = 20)]
public class EventData : ScriptableObject
{
    [SerializeField] 
    private int _id;
    public  int  id
    {
        get { return  _id; }
        set { _id = value; }
    }

    [SerializeField] 
    private int _priority;
    public  int  priority
    {
        get { return  _priority; }
        set { _priority = value; }
    }

    [SerializeField] 
    private List<EventDataCondition> _conditions;
    public  List<EventDataCondition>  conditions
    {
        get { return  _conditions; }
        set { _conditions = value; }
    }

    [SerializeField] 
    private DialogueContainer _dialogueContainer;
    public  DialogueContainer  dialogueContainer
    {
        get { return  _dialogueContainer; }
        set { _dialogueContainer = value; }
    }
}

[System.Serializable]
public class EventDataCondition
{
    // Type
    [SerializeField]
    private EventConditionType _type;
    public  EventConditionType  type
    {
        get { return _type; }
        set { _type = value; }
    }

    // Parameter
    [SerializeField]
    private ConstantCondition _constantCondition;
    public  ConstantCondition  constantCondition
    {
        get { return  _constantCondition; }
        set { _constantCondition = value; }
    }

    [SerializeField]
    private Month _month;
    public  Month  month
    {
        get { return  _month; }
        set { _month = value; }
    }
    [SerializeField]
    private Week _week;
    public  Week  week
    {
        get { return  _week; }
        set { _week = value; }
    }
    [SerializeField]
    private Day _day;
    public  Day  day
    {
        get { return  _day; }
        set { _day = value; }
    }

    [SerializeField]
    private Stat _stat;
    public  Stat  stat
    {
        get { return  _stat; }
        set { _stat = value; }
    }
    [SerializeField]
    private int _statValue;
    public  int  statValue
    {
        get { return  _statValue; }
        set { _statValue = value; }
    }

    [SerializeField]
    private Schedule _schedule;
    public  Schedule  schedule
    {
        get { return  _schedule; }
        set { _schedule = value; }
    }
    [SerializeField]
    private int _scheduleValue;
    public  int  scheduleValue
    {
        get { return  _scheduleValue; }
        set { _scheduleValue = value; }
    }

    [SerializeField]
    private FriendshipType _friendship;
    public  FriendshipType  friendship
    {
        get { return  _friendship; }
        set { _friendship = value; }
    }
    [SerializeField]
    private int _friendshipValue;
    public  int  friendshipValue
    {
        get { return  _friendshipValue; }
        set { _friendshipValue = value; }
    }

    [SerializeField]
    private EventComplition _eventComplition;
    public  EventComplition  eventComplition
    {
        get { return  _eventComplition; }
        set { _eventComplition = value; }
    }
    [SerializeField]
    private EventData _eventData;
    public  EventData  eventData
    {
        get { return  _eventData; }
        set { _eventData = value; }
    }
}
