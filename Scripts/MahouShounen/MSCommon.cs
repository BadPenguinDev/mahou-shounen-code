using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Common Enum
public enum DateType { Year, Month, Week, Day }
public enum Month { January, February, March, April, May, June, July, August, September, October, November, December }
public enum Week  { Week1, Week2, Week3, Week4 }
public enum Day   { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

public enum Stat { Sensitivity, Intelligence, Art, Strength, Elegance, Charisma }
public enum StatRank { S, A, B, C, D }

public enum SkillRank { S, A, B, C, D }

public enum Schedule { LiberalArts, SocialStudies, NaturalSciences, ComputerSciences, FineArts, Music, MartialArts, Fitness, Performance,
                       Supermarket, PostOffice, Electronics, FastFoods, Restaurant, Bookstore, Pharmacy, 
                       Dormitory, CentralPark, Beach, Forest }
public enum ScheduleType  { Class, PartTime, Others }
public enum ScheduleGrade { Beginner, Intermediate, Advanced }

public enum Spot { MainHall, ArtHall, TechHall, Stadium,
                   Supermarket, PostOffice, Electronics, FastFoods, Restaurant, Bookstore, Pharmacy, 
                   Dormitory, CentralPark, Beach, Forest }

public enum FriendshipType { None, Familiar, Scientist, Enterprise, ArcheryPlayer, Violinist, Chef, Cartoonist, Programmer, Idol, Prince, Ballerino };
public enum  GiftTasteType { Loves, Likes, Neutral, Dislike, Hates }

// Dialogue Enum
public enum   EmotionType { Idle, Smile, Grin, Sad, Angry, Surprised, Embarrassed, Thinking, Hurt, Staring };
public enum CharacterType { None, Shounen, Familiar, Scientist, Enterprise, ArcheryPlayer, Violinist, Chef, Cartoonist, Programmer, Idol, Prince, Ballerino };

public enum PlayerClothingType { CasualWear, Costume };
public enum   FamiliarFormType { Familiar, Human };

// Battle
public enum BuffType { Heal, Damage };
public enum MinionFormationType { Front, Back };


// Common Class and Struct
[System.Serializable]
public class Date
{
    public static bool Compare(Date date1, Date date2, ConstantCondition condition)
    {
        switch (condition)
        {
            case ConstantCondition.Less:
                return date1.month <  date2.month ||
                      (date1.month == date2.month && date1.week <  date2.week) ||
                      (date1.month == date2.month && date1.week == date2.week && date1.day <  date2.day);

            case ConstantCondition.LessEqual:
                return date1.month <= date2.month ||
                      (date1.month == date2.month && date1.week <= date2.week) ||
                      (date1.month == date2.month && date1.week == date2.week && date1.day <= date2.day);

            case ConstantCondition.Equal:
                return date1.month == date2.month &&
                       date1.week  == date2.week  &&
                       date1.day   == date2.day;

            case ConstantCondition.Greater:
                return date1.month >  date2.month ||
                      (date1.month == date2.month && date1.week >  date2.week) ||
                      (date1.month == date2.month && date1.week == date2.week && date1.day >  date2.day);

            case ConstantCondition.GreaterEqual:
                return date1.month >= date2.month ||
                      (date1.month == date2.month && date1.week >= date2.week) ||
                      (date1.month == date2.month && date1.week == date2.week && date1.day >= date2.day);

            case ConstantCondition.NotEqual:
                return date1.month != date2.month && 
                       date1.week  != date2.week  && 
                       date1.day   != date2.day;

            default:
                return false;
        }
    }

    public Month month;
    public Week  week;
    public Day   day;

    public Date()
    {
        month = Month.January;
        week  = Week.Week1;
        day   = Day.Sunday;
    }
    public Date(Date date)
    {
        month = date.month;
        week  = date.week;
        day   = date.day;
    }
    public Date(Month month, Week week, Day day)
    {
        this.month = month;
        this.week  = week;
        this.day   = day;
    }

    public void SetNextDay()
    {
        if (day == Day.Saturday)
        {
            day = Day.Sunday;
            if (week == Week.Week4)
            {
                week = Week.Week1;
                if (month == Month.December)
                {
                    month = Month.January;
                }
                else
                    month += 1;
            }
            else
                week += 1;
        }
        else
            day += 1;
    }
}

public class MSCommon
{
    // Stress
    public const int MaxStressGauge = 200;

    // Friendship
    public const int MaxFriendshipGauge = 1000;

    // Compare by condition
    public static bool CompareConstant(int value1, int value2, ConstantCondition condition)
    {
        switch (condition)
        {
            case ConstantCondition.Less:
                return value1 <  value2;
            case ConstantCondition.LessEqual:
                return value1 <= value2;
            case ConstantCondition.Equal:
                return value1 == value2;
            case ConstantCondition.Greater:
                return value1 >  value2;
            case ConstantCondition.GreaterEqual:
                return value1 >= value2;
            case ConstantCondition.NotEqual:
                return value1 != value2;
            default:
                return false;
        }
    }

    // Stat Rank Value
    public const int StatRankUpgradeValueA = 400;
    public const int StatRankUpgradeValueB = 200;
    public const int StatRankUpgradeValueC = 100;
    public const int StatRankUpgradeValueD =  50;

    public static int GetStatRankUpgradeValue(StatRank rank)
    {
        switch (rank)
        {
            case StatRank.A:
                return StatRankUpgradeValueA;
            case StatRank.B:
                return StatRankUpgradeValueB;
            case StatRank.C:
                return StatRankUpgradeValueC;
            case StatRank.D:
                return StatRankUpgradeValueD;
            default:
                return -1;
        }
    }
    public static int GetStatRankStartValue(StatRank rank)
    {
        switch (rank)
        {
            case StatRank.S:
                return StatRankUpgradeValueA;
            case StatRank.A:
                return StatRankUpgradeValueB;
            case StatRank.B:
                return StatRankUpgradeValueC;
            case StatRank.C:
                return StatRankUpgradeValueD;
            case StatRank.D:
            default:
                return 0;
        }
    }

    // Schedule
    public static Spot GetSpotTypeFromSchedule(Schedule schedule)
    {
        switch (schedule)
        {
            case Schedule.LiberalArts:    
            case Schedule.SocialStudies:        return Spot.MainHall;
            case Schedule.NaturalSciences:
            case Schedule.ComputerSciences:     return Spot.TechHall;
            case Schedule.FineArts: 
            case Schedule.Music:                return Spot.ArtHall;
            case Schedule.MartialArts:
            case Schedule.Fitness:    
            case Schedule.Performance:          return Spot.Stadium;

            case Schedule.Supermarket:          return Spot.Supermarket;
            case Schedule.PostOffice:           return Spot.PostOffice; 
            case Schedule.Electronics:          return Spot.Electronics;
            case Schedule.FastFoods:            return Spot.FastFoods;  
            case Schedule.Restaurant:           return Spot.Restaurant; 
            case Schedule.Bookstore:            return Spot.Bookstore;  
            case Schedule.Pharmacy:             return Spot.Pharmacy;   

            case Schedule.Dormitory:            return Spot.Dormitory;  
            case Schedule.CentralPark:          return Spot.CentralPark;
            case Schedule.Beach:                return Spot.Beach;
            case Schedule.Forest:               return Spot.Forest;     

            default:                            return Spot.MainHall;
        }
    }

    // Schedule Count
    public const int IntermediateScheduleCount = 50;
    public const int     AdvancedScheduleCount = 100;

    public static ScheduleGrade GetScheduleGradeByCount(int count)
    {
        if (count < IntermediateScheduleCount)
            return ScheduleGrade.Beginner;
        else if (count < AdvancedScheduleCount)
            return ScheduleGrade.Intermediate;
        else 
            return ScheduleGrade.Advanced;
    }

    public static int GetMaxCountByScheduleGrade(ScheduleGrade grade)
    {
        switch (grade)
        {
            case ScheduleGrade.Beginner:
                return IntermediateScheduleCount;
            case ScheduleGrade.Intermediate:
                return AdvancedScheduleCount;
            case ScheduleGrade.Advanced:
                return -1;
        }
        return -1;
    }

}

