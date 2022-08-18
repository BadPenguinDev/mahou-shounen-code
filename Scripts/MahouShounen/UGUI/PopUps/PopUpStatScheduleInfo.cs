using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PopUpStatScheduleInfo : PopUpBase
{
    public Button buttonReturn;
    
    [Header("Common Info")]
    public Image imageScheduleIcon;
    public LocalizeStringEvent localizedTextName;
    public LocalizeStringEvent localizedTextDescription;

    [Header("Grade Info")]
    public Image imageGradeIcon;
    
    public Scrollbar scrollbarSchedule;
    public Text textScheduleCount;
    
    public Text textCost;
    public List<ElementScheduleStat> elementScheduleStats;
    public ElementScheduleStress elementScheduleStress;
    
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
    }
    
    public void SetScheduleData(ScheduleData data)
    {
        if (data == null)
            return;
        
        // Common Info
        imageScheduleIcon.sprite = data.spriteSchedule;
        
        localizedTextName.SetEntry(data.keyScheduleName);
        localizedTextDescription.SetEntry(data.keyScheduleName);

        // Grade
        var grade = MSGameInstance.Get().GetScheduleGrade(data);
        imageGradeIcon.sprite = MSDataManager.Get().globalPaletteData.spriteGradeScheduleIcon[(int)grade];

        // EXP
        var maxCount = MSCommon.GetMaxCountByScheduleGrade(grade);
        if (maxCount < 0)
        {
            scrollbarSchedule.size = 1f;
            textScheduleCount.text = "MAX";
        }
        else
        {
            var currentCount = MSGameInstance.Get().playerSchedules[data.schedule];
            
            scrollbarSchedule.size = currentCount / (float)maxCount;
            textScheduleCount.text = currentCount.ToString() + "/" + maxCount.ToString();
        }
        
        // Currency
        textCost.text = data.GetCost(grade).ToString();
        
        // Stat
        var statDatas = data.GetStats(grade);
        for (var i = 0; i < elementScheduleStats.Count; i++)
        {
            elementScheduleStats[i].gameObject.SetActive(i < statDatas.Count);
            if (i < statDatas.Count)
                elementScheduleStats[i].SetStat(statDatas[i].type, statDatas[i].modifier);
        }
        elementScheduleStress.SetValue(data.GetStress(grade));
    }
    
    void OnButtonReturnClicked()
    {
        ClosePopUp();
    }
}
