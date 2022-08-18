using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Global Initialize Data", menuName = "Global/Initialize", order = 999)]
public class GlobalInitializeData : ScriptableObject
{
    [Header("Player")] 
    public LocalizedString playerName;
    public Date            playerBirthday;

    public Date currentDate;
    public int  currency;

    public List<Item> items;
    
    [Header("Battle")] 
    public List<Skill> playerSkills;
    
    public int playerHP;
    public int playerArmor;

    public int maxCylinderSlots;
    public int maxHeatGauge;
    public int spinCoolingCount;
    public int reloadCoolingCount;
    
    [Header("Event")] 
    public EventData introEvent;
    public List<EventData> globalDreampostBeginEvents;
    public List<EventData> globalDreampostVictoryEvents;
    public List<EventData> globalDreampostDepeatEvents;
}
