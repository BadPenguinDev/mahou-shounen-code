using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;


[System.Serializable] public class OnDailyScheduleRoutineStartedEvent : UnityEvent { }
[System.Serializable] public class OnDailyScheduleFinishedEvent : UnityEvent<Schedule> { }

public class LayoutDaily : LayoutBase
{
    public List<Schedule> targetSchedules;
    public Schedule       targetSchedule;

    [Header("Daily Date")]
    public Text textMonth;
    public Text textWeek;
    public Text textDay;

    [Header("Daily Stat")]
    public Transform verticalListDailyStat;
    public GameObject fieldDailyStatPrefab;
    public List<ElementDailyStat> dailyStatFields;
    public ElementDailyStress     dailyStressField;

    [Header("Daily Animations")]
    public Transform  fieldDailyAnimation;
    public GameObject dailyAnimation;
    public Image imageDailyScoreGrade;

    [Header("Daily Cost")]
    public Text textCurrency;
    public Text textCurrencyAddon;

    [Header("Events")]
    public OnDailyScheduleRoutineStartedEvent onDailyScheduleRoutineStarted;
    public OnDailyScheduleFinishedEvent onDailyScheduleFinished;

    // Private
    private const float NextDayDelayStart = 2.0f;

    private int   currencyValue;
    private int   currencyModifier;


    public void Awake()
    {
        for (var i = 0; i < 3; i++)
        {
            var prefabObject = Object.Instantiate(fieldDailyStatPrefab, verticalListDailyStat);
            var element = prefabObject.GetComponent<ElementDailyStat>();

            dailyStatFields.Add(element);
        }
    }
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        dailyAnimation.GetComponent<ElementDailyAnimation>().onDailyAnimationEnded.AddListener(SetDayEnds);

        var gameInstance = MSGameInstance.Get();

        onDailyScheduleFinished.AddListener(gameInstance.OnDailyScheduleFinished);
        onDailyScheduleRoutineStarted.AddListener(gameInstance.OnDailyScheduleRoutineStarted);

        gameInstance.onDateChanged.AddListener(OnDateChanged);
    }

    public void SetDayEnds()
    {
        imageDailyScoreGrade.gameObject.SetActive(true);
        StartCoroutine(PlayDailyScheduleResult(NextDayDelayStart));
    }
    public void OnDateChanged(Date date)
    {
        if (date.day == Day.Sunday)
        {
            LayoutMain layoutMain = UGUIManager.Get().GetLayout(LayoutType.Main) as LayoutMain;
            layoutMain.SetLayoutMainMode(LayoutMainMode.WeekendDay);

            manager.OpenLayout(LayoutType.Main);
            CloseLayout();
        }
        else
        {
            // Initialize
            ClearFieldDailyStat();

            imageDailyScoreGrade.gameObject.SetActive(false);

            // Set Date
            var monthKey = "Cont" + date.month.ToString();

            textMonth.GetComponent<LocalizeStringEvent>().SetEntry(monthKey);
            textMonth.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");

            textWeek.GetComponent<LocalizeStringEvent>().SetEntry(date.week.ToString());
            textWeek.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");

            textDay.GetComponent<LocalizeStringEvent>().SetEntry(date.day.ToString());
            textDay.GetComponent<LocalizeStringEvent>().SetTable("DateTimes");

            // Schedule
            targetSchedule = targetSchedules[(int)date.day];
            var targetScheduleData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == targetSchedule);

            // Schedule Stat
            var grade = MSGameInstance.Get().GetScheduleGrade(targetScheduleData);
            var statDatas = targetScheduleData.GetStats(grade);
            for (var i = 0; i < statDatas.Count; i++)
            {
                dailyStatFields[i].SetStat(targetSchedule, statDatas[i].type);
                dailyStatFields[i].gameObject.SetActive(true);
            }
            dailyStressField.SetStress(targetSchedule);

            // Get Character Daily Schedule
            var friendship = MSDataManager.Get().FindCharacterTypeByDailySchedule(date.day, targetSchedule);

            // Schedule Animation
            dailyAnimation.GetComponent<Animator>().runtimeAnimatorController = null;
            dailyAnimation.GetComponent<Animator>().runtimeAnimatorController = targetScheduleData.dailyAnimation;
            dailyAnimation.GetComponent<ElementDailyAnimation>().SetDailyAnimationFigure(friendship);

            // Daily Currency
            currencyValue    = MSGameInstance.Get().currency;
            currencyModifier = targetScheduleData.GetCost(grade);

            string currencyModifierString = (currencyModifier > 0) ? "+" + currencyModifier.ToString() :
                                                                           currencyModifier.ToString();
            textCurrency.     text = currencyValue.ToString();
            textCurrencyAddon.text = currencyModifierString;
        }
    }

    public void ClearFieldDailyStat()
    {
        foreach (ElementDailyStat field in dailyStatFields)
        {
            field.gameObject.SetActive(false);
        }
    }

    IEnumerator PlayDailyScheduleResult(float time)
    {
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            foreach (var element in dailyStatFields)
            {
                element.UpdateStatResult(t * 2.0f);
            }
            dailyStressField.UpdateStressResult(t * 2.0f);

            // Update Currencys
            var value = Mathf.Lerp(0, currencyModifier, t * 2.0f);

            var modifiedCurrencyValue = (int)(currencyValue + value);
            var modifiedCurrencyString = modifiedCurrencyValue.ToString();

            var modifiedAddonValue  = (int)(currencyModifier - value);
            var modifiedAddonString = (modifiedAddonValue > 0) ? "+" + modifiedAddonValue.ToString() :
                                                                          modifiedAddonValue.ToString();
            textCurrency.     text = modifiedCurrencyString;
            textCurrencyAddon.text = modifiedAddonString;

            yield return null;
        }
        onDailyScheduleFinished.Invoke(targetSchedule);
    }
}
