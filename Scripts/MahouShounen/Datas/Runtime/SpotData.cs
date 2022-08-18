using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spot", menuName = "Data/Spot", order = int.MaxValue)]
public class SpotData : ScriptableObject
{
    public Spot type;
    public Sprite spotSprite;
    public Sprite backgroundSprite;
}
