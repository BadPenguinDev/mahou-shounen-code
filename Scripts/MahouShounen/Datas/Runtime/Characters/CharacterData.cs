using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Character", menuName = "Data/Character", order = 0)]
public class CharacterData : ScriptableObject
{
    // Character Type
    [SerializeField]
    private FriendshipType _friendshipType;
    public  FriendshipType  friendshipType
    {
        get { return  _friendshipType; }
        set { _friendshipType = value; }
    }
    
    [SerializeField]
    private Sprite _iconSprite;
    public  Sprite  iconSprite
    {
        get { return  _iconSprite; }
        set { _iconSprite = value; }
    }
    
    // Birthday
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

    // Fortraits
    [SerializeField]
    private List<CharacterSchedule> _characterSchedules;
    public  List<CharacterSchedule>  characterSchedules
    {
        get { return  _characterSchedules; }
        set { _characterSchedules = value; }
    }

    // Fortraits
    [SerializeField]
    private List<Sprite> _emotionSprites;
    public  List<Sprite>  emotionSprites
    {
        get { return  _emotionSprites; }
        set { _emotionSprites = value; }
    }
    
    [SerializeField]
    private List<Sprite> _costumeSprites;
    public  List<Sprite>  costumeSprites
    {
        get { return  _costumeSprites; }
        set { _costumeSprites = value; }
    }

    // Gift Tastes
    [SerializeField] 
    private List<GiftTaste> _giftTastes;
    public  List<GiftTaste>  giftTastes
    {
        get { return  _giftTastes; }
        set { _giftTastes = value; }
    }

    // Weekend Event Options
    [SerializeField]
    private List<WeekendEventData> _weekendEvents;
    public  List<WeekendEventData>  weekendEvents
    {
        get { return  _weekendEvents; }
        set { _weekendEvents = value; }
    }


    void Awake()
    {
        if (characterSchedules == null)
        {
            characterSchedules = new List<CharacterSchedule>();
            foreach (Day day in System.Enum.GetValues(typeof(Day)))
            {
                if (characterSchedules.Count <= (int)day)
                {
                    CharacterSchedule schedule = new CharacterSchedule();
                    schedule.day = day;

                    characterSchedules.Add(schedule);
                }
            }
        }

        if (emotionSprites == null)
        {
            emotionSprites = new List<Sprite>();
            foreach (EmotionType emotion in System.Enum.GetValues(typeof(EmotionType)))
            {
                if (emotionSprites.Count <= (int)emotion)
                    emotionSprites.Add(null);
            }
        }

        if (costumeSprites == null)
        {
            costumeSprites = new List<Sprite>();
            for (int i = 0; i < 2; i++)
            {
                if (costumeSprites.Count <= i)
                    costumeSprites.Add(null);
            }
        }

        if (giftTastes == null)
            giftTastes = new List<GiftTaste>();

        if (weekendEvents == null)
            weekendEvents =  new List<WeekendEventData>();
    }

    public bool CheckCharacterDailySchedule(Day day, Schedule schedule)
    {
        CharacterSchedule scheduleData = characterSchedules.Find(x => x.day == day &&
                                                                      x.schedule == schedule);
        return scheduleData != null;
    }
}

[System.Serializable]
public class CharacterSchedule
{
    [SerializeField]
    private Day _day;
    public  Day  day
    {
        get { return  _day; }
        set { _day = value; }
    }

    [SerializeField]
    private Schedule _schedule;
    public  Schedule  schedule
    {
        get { return  _schedule; }
        set { _schedule = value; }
    }
}

[System.Serializable]
public class GiftTaste
{
    [SerializeField]
    private ItemData _item;
    public  ItemData  item
    {
        get { return  _item; }
        set { _item = value; }
    }

    [SerializeField]
    private GiftTasteType _taste;
    public  GiftTasteType  taste
    {
        get { return  _taste; }
        set { _taste = value; }
    }
}

