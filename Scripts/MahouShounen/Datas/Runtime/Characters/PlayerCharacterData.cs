using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Player", menuName = "Data/Player", order = 1)]
public class PlayerCharacterData : ScriptableObject
{
    [SerializeField]
    private Sprite _iconSprite;
    public  Sprite  iconSprite
    {
        get { return  _iconSprite; }
        set { _iconSprite = value; }
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
    private List<Sprite> _casualWearSprites;
    public  List<Sprite>  casualWearSprites
    {
        get { return  _casualWearSprites; }
        set { _casualWearSprites = value; }
    }

    [SerializeField]
    private List<Sprite> _costumeSprites;
    public  List<Sprite>  costumeSprites
    {
        get { return  _costumeSprites; }
        set { _costumeSprites = value; }
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
        if (emotionSprites == null)
        {
            emotionSprites = new List<Sprite>();
            foreach (EmotionType emotion in System.Enum.GetValues(typeof(EmotionType)))
            {
                if (emotionSprites.Count <= (int)emotion)
                    emotionSprites.Add(null);
            }
        }

        if (casualWearSprites == null)
        {
            casualWearSprites = new List<Sprite>();
            for (var i = 0; i < 3; i++)
            {
                if (casualWearSprites.Count <= i)
                    casualWearSprites.Add(null);
            }
        }

        if (costumeSprites == null)
        {
            costumeSprites = new List<Sprite>();
            for (var i = 0; i < 3; i++)
            {
                if (costumeSprites.Count <= i)
                    costumeSprites.Add(null);
            }
        }

        if (weekendEvents == null)
        {
            weekendEvents = new List<WeekendEventData>();
        }
    }
}

