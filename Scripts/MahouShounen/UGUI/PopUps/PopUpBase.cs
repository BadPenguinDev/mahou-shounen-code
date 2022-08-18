using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PopUpType { SkillUpgrade, ScheduleInfo, SchedulePreset,
                        Settings = 100, ReturnToTitleMessage, 
                        WorldMapSpot = 200, WorldMapSpotDetail, DreamflowReward, DreamflowExitMessage }

public class PopUpBase : MSUIComponentBase
{
    public PopUpType  type;
    public float openDelay;

    protected Animator animator;
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        animator = GetComponent<Animator>();
    }

    public virtual void    OpenPopUp()
    {
        gameObject.SetActive(true);
        animator.Play("On");
    }
    public virtual void   ClosePopUp()
    {
        animator.Play("Off");
    }
    public virtual void DisablePopUp()
    {
        gameObject.SetActive(false);
    }
}
