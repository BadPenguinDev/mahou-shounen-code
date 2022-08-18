using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dreampost", menuName = "Data/Dreampost - Random", order = 141)]
public class RandomDreampostData : DreampostData
{
    [Header("Dreampost - Random")]
    [SerializeField]
    private Sprite _icon;
    public  Sprite  icon
    {
        get { return _icon; }
    }

    public override Sprite GetPortraitMini()
    {
        return icon;
    }
}
