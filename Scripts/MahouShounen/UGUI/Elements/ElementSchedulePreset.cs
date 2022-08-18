using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSchedulePreset : ElementBase
{
    SchedulePreset preset;
    
    public Toggle toggleSchedulePreset;
    [SerializeField] List<Image> imageScheduleIcons;
    
    public void SetSchedulePreset(SchedulePreset targetPreset)
    {
        preset = targetPreset;
        for (var i = 0; i < imageScheduleIcons.Count; i++)
        {
            if (i < preset.Count)
            {
                var data = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == preset[i]);
                imageScheduleIcons[i].sprite = data.spriteSchedule;
            }
            else
                imageScheduleIcons[i].sprite = MSDataManager.Get().globalPaletteData.spriteTransparent;
        }
    }
    public SchedulePreset GetSchedulePreset() { return preset; }
}
