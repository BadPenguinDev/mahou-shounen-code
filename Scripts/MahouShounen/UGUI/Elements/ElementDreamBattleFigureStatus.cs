using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleFigureStatus : ElementBase
{
    [SerializeField] GameObject prefabElementBuff;

    [SerializeField] GameObject panelHP;
    [SerializeField] GameObject panelArmor;
    [SerializeField] GameObject panelTimer;

    [SerializeField] Text textHP;
    [SerializeField] Text textArmor;
    [SerializeField] Text textTimer;

    List<ElementDreamBattleFigureStatusBuff>   activeBuffElements;
    List<ElementDreamBattleFigureStatusBuff> inactiveBuffElements;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);

          activeBuffElements = new List<ElementDreamBattleFigureStatusBuff>();
        inactiveBuffElements = new List<ElementDreamBattleFigureStatusBuff>();
    }

    public void SetTargetFigure(BattleFigureInstance instance)
    {
        OnHPChanged   (instance.hp); 
        OnArmorChanged(instance.armor); 
        OnTimerChanged(instance.timer); 

        instance.onFigureHPChanged.   AddListener(OnHPChanged); 
        instance.onFigureArmorChanged.AddListener(OnArmorChanged); 
        instance.onFigureTimerChanged.AddListener(OnTimerChanged);

        instance.onBuffAdded.  AddListener(OnBuffAdded);
        instance.onBuffRemoved.AddListener(OnBuffRemoved);

        for (int i = 0; i < activeBuffElements.Count; i++)
        {
            activeBuffElements[i].gameObject.SetActive(false);
            activeBuffElements.Remove(activeBuffElements[i]);
        }
        inactiveBuffElements.Clear();
    }

    public void OnHPChanged   (int value)
    {
        textHP.text = value.ToString();
    }
    public void OnArmorChanged(int value)
    {
        if (value > 0)
            panelArmor.SetActive(true);
        else
            panelArmor.SetActive(false);

        textArmor.text = value.ToString();
    }
    public void OnTimerChanged(int value)
    {
        if (value > 0)
            panelTimer.SetActive(true);
        else
            panelTimer.SetActive(false);

        textTimer.text = value.ToString();
    }

    public void OnBuffAdded  (BuffInstance buff)
    {
        for (int i = 0; i < activeBuffElements.Count; i++)
        {
            if (buff.data == activeBuffElements[i].GetBuffInstance().data)
            {
                activeBuffElements[i].SetBuffInstance(buff);
                return;
            }
        }

        GameObject instance = Instantiate(prefabElementBuff, transform);
        ElementDreamBattleFigureStatusBuff element = instance.GetComponent<ElementDreamBattleFigureStatusBuff>();

        activeBuffElements.Add(element);
        element.SetBuffInstance(buff);
    }
    public void OnBuffRemoved(BuffInstance buff)
    {
        ElementDreamBattleFigureStatusBuff element = null;
        for (int i = 0; i < activeBuffElements.Count; i++)
        {
            if (buff == activeBuffElements[i].GetBuffInstance())
            {
                element = activeBuffElements[i];
                break;
            }
        }

        if (element != null)
        {
            element.gameObject.SetActive(false);

              activeBuffElements.Remove(element);
            inactiveBuffElements.Add   (element);
        }
    }
}
