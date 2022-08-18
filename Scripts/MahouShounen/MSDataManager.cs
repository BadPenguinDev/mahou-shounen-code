using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class MSDataManager
{
    public static MSDataManager dataManager;

    public GlobalInitializeData globalInitializeData;
    public GlobalPaletteData    globalPaletteData;

    public PlayerCharacterData playerCharacterData;
    public        FamiliarData        familiarData;
    public List <CharacterData>      characterDatas;

    public List<ScheduleData> scheduleDatas;
    public List    <SpotData>     spotDatas;

    public List<ItemData> itemDatas;

    public List    <RandomDreampostData>     randomDreampostDatas;
    public List<FriendshipDreampostData> friendshipDreampostDatas;

    public List<PlayerSkillData> playerSkillDatas;
    
    
    // Access
    public static MSDataManager Get()
    {
        if (dataManager == null)
        {
            dataManager = new MSDataManager();
            dataManager.LoadDatabase();
        }
        return dataManager;
    }

    // Initializer
    public void LoadDatabase()
    {
    #if UNITY_EDITOR
        AssetDatabase.Refresh();
    #endif
        
        // Global
        globalInitializeData = Resources.Load("Datas/Global Initialize Data", typeof(GlobalInitializeData)) as GlobalInitializeData;
        globalPaletteData    = Resources.Load("Datas/Global Palette Data",    typeof(GlobalPaletteData))    as GlobalPaletteData;
        
        // Base Character Datas
        playerCharacterData = Resources.Load("Datas/Characters/PlayerCharacter", typeof(PlayerCharacterData)) as PlayerCharacterData;
        familiarData        = Resources.Load("Datas/Characters/Familiar",        typeof(FamiliarData))        as FamiliarData;


        #region Character Datas
        characterDatas = new List<CharacterData>();

        var characterObjects = Resources.LoadAll("", typeof(CharacterData));
        foreach (var characterObject in characterObjects)
        {
            characterDatas.Add((CharacterData)characterObject);
        }
        characterDatas = characterDatas.OrderBy(w => w.friendshipType).ToList();
        #endregion


        #region Schedule Datas
        scheduleDatas = new List<ScheduleData>();

        var scheduleObjects = Resources.LoadAll("", typeof(ScheduleData));
        foreach (var scheduleObject in scheduleObjects)
        {
            scheduleDatas.Add((ScheduleData)scheduleObject);
        }
        scheduleDatas = scheduleDatas.OrderBy(w => w.schedule).ToList();
        #endregion

        
        #region Item Datas
        itemDatas = new List<ItemData>();

        var itemObjects = Resources.LoadAll("", typeof(ItemData));
        foreach (var itemObject in itemObjects)
        {
            itemDatas.Add((ItemData)itemObject);
        }
        itemDatas = itemDatas.OrderBy(w => w.id).ToList();
        #endregion

        
        #region Spot Datas
        spotDatas = new List<SpotData>();

        var spotObjects = Resources.LoadAll("", typeof(SpotData));
        foreach (var spotObject in spotObjects)
        {
            spotDatas.Add((SpotData)spotObject);
        }
        spotDatas = spotDatas.OrderBy(w => w.type).ToList();
        #endregion


        #region Dreampost Datas
        randomDreampostDatas = new List<RandomDreampostData>();

        var randomDreampostObjects = Resources.LoadAll("", typeof(RandomDreampostData));
        foreach (var randomDreampostObject in randomDreampostObjects)
        {
            randomDreampostDatas.Add((RandomDreampostData)randomDreampostObject);
        }
        randomDreampostDatas = randomDreampostDatas.OrderBy(w => w.id).ToList();


        friendshipDreampostDatas = new List<FriendshipDreampostData>();

        var friendshipDreampostObjects = Resources.LoadAll("", typeof(FriendshipDreampostData));
        foreach (var friendshipDreampostObject in friendshipDreampostObjects)
        {
            friendshipDreampostDatas.Add((FriendshipDreampostData)friendshipDreampostObject);
        }
        friendshipDreampostDatas = friendshipDreampostDatas.OrderBy(w => w.id).ToList();
        #endregion


        #region Skill Datas
        playerSkillDatas = new List<PlayerSkillData>();

        var playerSkillDataObjects = Resources.LoadAll("", typeof(PlayerSkillData));
        foreach (var playerSkillDataObject in playerSkillDataObjects)
        {
            playerSkillDatas.Add((PlayerSkillData)playerSkillDataObject);
        }
        playerSkillDatas = playerSkillDatas.OrderBy(w => w.id).ToList();
        #endregion
    }

    public FriendshipType FindCharacterTypeByDailySchedule(Day day, Schedule schedule)
    {
        var data = characterDatas.Find(x => x.CheckCharacterDailySchedule(day, schedule));
        if (data == null)
            return FriendshipType.None;

        return data.friendshipType;
    }

    // Schedule Datas
    public int GetMaxScheduleCountByType()
    {
        var scheduleCountByType = new Dictionary<ScheduleType, int>();
        foreach (var data in scheduleDatas)
        {
            if (scheduleCountByType.ContainsKey(data.type))
                scheduleCountByType[data.type] += 1;
            else
                scheduleCountByType.Add(data.type, 1);
        }
        return scheduleCountByType.Aggregate((x, y) => x.Value > y.Value ? x : y).Value;
    }
    public List<ScheduleData> GetScheduleDatasByType(ScheduleType type)
    {
        var datas = new List<ScheduleData>();
        foreach (var data in scheduleDatas)
        {
            if (data.type == type)
                datas.Add(data);
        }
        return datas;
    }
}
