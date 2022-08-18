using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class MSNewGameController : MonoBehaviour
{
    public GameObject layoutSetBirthday;

    public Animator shadowAnimator;

    public InputField inputFieldName;
    public Text textBirthday;

    public Dropdown dropdownDay;
    public Dropdown dropdownWeek;
    public Dropdown dropdownMonth;
    
    public Button buttonComplete;

    public AudioSource source;
    public AudioClip   clipButton;

    private IEnumerator playButtonSoundCoroutine;
    private Date tempBirthday = new Date();

    // Start is called before the first frame update
    void Start()
    {
        shadowAnimator.enabled = true;

        inputFieldName.onEndEdit.AddListener(SetPlayerName);
        buttonComplete.onClick.AddListener(StartInGame);

        // Dropdown Day
        dropdownDay.onValueChanged.AddListener(SetPlayerTempBirthdayDay);
        var dayLocStrings = new List<string>();
        for (var day = Day.Sunday; day <= Day.Saturday; day++)
        {
            var dayKey = day.ToString();
            var dayLocString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", dayKey);
            
            dayLocStrings.Add(dayLocString);
        }
        dropdownDay.AddOptions(dayLocStrings);
        
        // Dropdown Week
        dropdownWeek.onValueChanged.AddListener(SetPlayerTempBirthdayWeek);
        var weekLocStrings = new List<string>();
        for (var week = Week.Week1; week <= Week.Week4; week++)
        {
            var weekKey = week.ToString();
            var weekLocString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", weekKey);
            
            weekLocStrings.Add(weekLocString);
        }
        dropdownWeek.AddOptions(weekLocStrings);
        
        // Dropdown Month
        dropdownMonth.onValueChanged.AddListener(SetPlayerTempBirthdayMonth);
        var monthLocStrings = new List<string>();
        for (var month = Month.January; month <= Month.December; month++)
        {
            var monthKey = month.ToString();
            var monthLocString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", monthKey);
            
            monthLocStrings.Add(monthLocString);
        }
        dropdownMonth.AddOptions(monthLocStrings);
        
        SetPlayerBirthday();
        
        source.volume = MSGameInstance.Get().preferences.volumeMusic * 0.2f;
    }

    public void StartInGame()
    {
        if (string.IsNullOrEmpty(MSGameInstance.Get().playerName)) 
            return;
        
        StartCoroutine("StartInGameCoroutine");
        shadowAnimator.Play("ShadowOn");
    }
    IEnumerator StartInGameCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        MSGameInstance.Get().isNewGame = true;
        SceneManager.LoadScene("InGameScene", LoadSceneMode.Single);
    }

    public void SetPlayerName(string playerName)
    {
        buttonComplete.interactable = playerName != "";
        MSGameInstance.Get().playerName = playerName;
    }
    public void SetPlayerBirthday()
    {
        MSGameInstance.Get().playerBirthday = new Date(tempBirthday);

        var   dayKey = MSGameInstance.Get().playerBirthday.  day.ToString();
        var  weekKey = MSGameInstance.Get().playerBirthday. week.ToString();
        var monthKey = MSGameInstance.Get().playerBirthday.month.ToString();

        // if (LocalizationSystem.Get().IsUsingContractionInFullDate())
        // {
        //     monthKey = "Cont" + monthKey;
        //       dayKey = "Cont" +   dayKey;
        // }

        var birthdayString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", dayKey) + ", "
                           + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", weekKey) + ", "
                           + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", monthKey);

        textBirthday.text = birthdayString;
    }

    public void SetPlayerTempBirthdayDay  (int   day)
    {
        tempBirthday.day   = (Day)  day;
    }
    public void SetPlayerTempBirthdayWeek (int  week)
    {
        tempBirthday.week  = (Week) week;
    }
    public void SetPlayerTempBirthdayMonth(int month)
    {
        tempBirthday.month = (Month)month;
    }


    public void StartPlayButtonSound()
    {
        if (playButtonSoundCoroutine != null)
            StopCoroutine(playButtonSoundCoroutine);

        playButtonSoundCoroutine = PlayButtonSoundCoroutine();
        StartCoroutine(playButtonSoundCoroutine);
    }
    IEnumerator PlayButtonSoundCoroutine()
    {
        source.PlayOneShot(clipButton, 0.2f * MSGameInstance.Get().preferences.volumeSound);
        for (var t = 0f; t < 1.0f; t += Time.deltaTime / 0.1f)
        {
            yield return null;
        }
    }
}
