using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Schedule", menuName = "Data/Schedule", order = int.MaxValue)]
public class ScheduleData : ScriptableObject
{
    public ScheduleType type;
    public Schedule schedule;

    public Sprite spriteSchedule;
    public string keyScheduleName;

    public List<ScheduleDataGrade> grades;
    public AnimationClip dailyAnimationClip;
    public RuntimeAnimatorController dailyAnimation;


    public int GetCost  (ScheduleGrade grade)
    {
        ScheduleDataGrade dataGrade = grades.Find(x => x.type == grade);
        if (dataGrade == null)
            return 0;

        return dataGrade.cost;
    }
    public int GetStat  (ScheduleGrade grade, Stat stat)
    {
        ScheduleDataGrade dataGrade = grades.Find(x => x.type == grade);
        if (dataGrade == null)
            return 0;

        ScheduleDataStat  dataStat  = dataGrade.stats.Find(x => x.type == stat);
        if (dataGrade == null)
            return 0;

        return dataStat.modifier;
    }
    public int GetStress(ScheduleGrade grade)
    {
        ScheduleDataGrade dataGrade = grades.Find(x => x.type == grade);
        if (dataGrade == null)
            return 0;

        return dataGrade.stress;
    }

    public List<ScheduleDataStat> GetStats(ScheduleGrade grade)
    {
        List<ScheduleDataStat> stats = new List<ScheduleDataStat>();

        ScheduleDataGrade dataGrade = grades.Find(x => x.type == grade);
        if (dataGrade == null)
            return stats;

        return dataGrade.stats;
    }
}
[System.Serializable]
public class ScheduleDataGrade
{
    public ScheduleGrade type;
    public int cost;
    public int stress;

    public List<ScheduleDataStat>  stats;
}
[System.Serializable]
public class ScheduleDataStat
{
    public Stat type;
    public int modifier;
}
[System.Serializable]
public class ScheduleDataDailySprite
{
    public Sprite background;
    public Sprite foreground;
}
