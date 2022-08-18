using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[System.Serializable]
public class MSGamePreferences
{
    public int gameClearCount;
    public int prestigePoint;
    public List<int> unlockedPerkIDs;
    public List<Skill> globalSavedSkills;
    
    public List<CasualwearType> unlockedCasualwears;
    public List<CostumeType>    unlockedCostumes;
    public List<GlobalAchievementSave> achievementSaves;
    
    public List<int> collectedFullCGIDs;
    public List<int> collectedAnimationCGIDs;
    public List<int> collectedEndingIDs;
    
    // Settings
    public int volumeMusic;
    public int volumeSound;
    public LocaleIdentifier localeIdentifier;

    public void Initialize()
    {
        gameClearCount = 0;
        prestigePoint = 0;
        unlockedPerkIDs   = new List<int>();
        globalSavedSkills = new List<Skill>();
        
        unlockedCasualwears = new List<CasualwearType>();
        unlockedCostumes    = new List<CostumeType>();
        achievementSaves    = new List<GlobalAchievementSave>();
        
        collectedFullCGIDs      = new List<int>();
        collectedAnimationCGIDs = new List<int>();
        collectedEndingIDs      = new List<int>();
        
        // Settings
        volumeMusic = 2;
        volumeSound = 4;
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            localeIdentifier = locale.Identifier;
        }
    }
    
    public void Save()
    {
        var path = Path.Combine(Application.persistentDataPath, "Preferences.json");
        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, json);
    }
    public void Load()
    {
        var path = Path.Combine(Application.persistentDataPath, "Preferences.json");
        if (!File.Exists(path))
        {
            Initialize();
            Save();
        } 
        else 
        {
            var json = File.ReadAllText(path);
            var preferences = JsonUtility.FromJson<MSGamePreferences>(json);

            gameClearCount = preferences.gameClearCount;
            prestigePoint  = preferences.prestigePoint;
            unlockedPerkIDs = preferences.unlockedPerkIDs;
            globalSavedSkills = preferences.globalSavedSkills;
        
            unlockedCasualwears = preferences.unlockedCasualwears;
            unlockedCostumes    = preferences.unlockedCostumes;
            achievementSaves    = preferences.achievementSaves;
        
            collectedFullCGIDs      = preferences.collectedFullCGIDs;
            collectedAnimationCGIDs = preferences.collectedAnimationCGIDs;
            collectedEndingIDs      = preferences.collectedEndingIDs;
            
            volumeMusic = preferences.volumeMusic;
            volumeSound = preferences.volumeSound;
            localeIdentifier = preferences.localeIdentifier;
        }
    }
}

[System.Serializable]
public class GlobalAchievementSave
{
    public int id;
    public bool isUnlocked;
}