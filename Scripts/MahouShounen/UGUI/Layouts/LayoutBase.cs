using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayoutType { Main, Schedule, Daily, WorldMap, Stat, Social, Skill, Journal, Inventory, DressRoom, Achievement, Dreampost, Dreamflow, DreamBattle }

public class LayoutBase : MSUIComponentBase
{
    public LayoutType type;
    public float openDelay;

    private Animator animator;
    private Canvas canvas;

    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
        animator = GetComponent<Animator>();
        canvas   = GetComponent<Canvas>();
    }
    public virtual void   OpenLayout(string layoutName = "On")
    {
        gameObject.SetActive(true);
        
        animator.Play(layoutName);
        canvas.sortingOrder = 2;
    }
    public virtual void  CloseLayout(string layoutName = "Off")
    {
        animator.Play(layoutName);
        canvas.sortingOrder = 1;
    }
    public virtual void UpdateLayout()
    {

    }

    public void SetActive(int flag)
    {
        gameObject.SetActive(System.Convert.ToBoolean(flag));
    }
}