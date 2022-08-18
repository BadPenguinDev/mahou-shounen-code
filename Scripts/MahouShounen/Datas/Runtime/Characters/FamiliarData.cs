using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Familiar", menuName = "Data/Familiar", order = 2)]
public class FamiliarData : ScriptableObject
{
    [SerializeField]
    private Sprite _iconSprite;
    public  Sprite  iconSprite
    {
        get { return  _iconSprite; }
        set { _iconSprite = value; }
    }

    // Fortraits - Familiar
    [SerializeField]
    private List<Sprite> _familiarEmotionSprites;
    public  List<Sprite>  familiarEmotionSprites
    {
        get { return  _familiarEmotionSprites; }
        set { _familiarEmotionSprites = value; }
    }

    // Fortraits - Human
    [SerializeField]
    private List<Sprite> _humanEmotionSprites;
    public  List<Sprite>  humanEmotionSprites
    {
        get { return  _humanEmotionSprites; }
        set { _humanEmotionSprites = value; }
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
        if (familiarEmotionSprites == null)
        {
            familiarEmotionSprites = new List<Sprite>();
            foreach (EmotionType emotion in System.Enum.GetValues(typeof(EmotionType)))
            {
                if (familiarEmotionSprites.Count <= (int)emotion)
                    familiarEmotionSprites.Add(null);
            }
        }

        if (humanEmotionSprites == null)
        {   
            humanEmotionSprites = new List<Sprite>();
            foreach (EmotionType emotion in System.Enum.GetValues(typeof(EmotionType)))
            {
                if (humanEmotionSprites.Count <= (int)emotion)
                    humanEmotionSprites.Add(null);
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
    }
}
