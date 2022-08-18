using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class LayoutDreamflow : LayoutBase
{
    [Header("Dreamflow Map")]
    [SerializeField] ElementDreamflowMap elementDreamflowMap;

    [Header("Dreamflow Node Info")]
    [SerializeField] Image spotImage;
    [SerializeField] Button buttonReturn;
    [SerializeField] Button buttonStartBattle;
    [SerializeField] List<ElementDreamflowMonster> elementMonsters;

    DreamflowNodeData node;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        elementDreamflowMap.SetParentComponent(this);
        elementDreamflowMap.onTargetNodeSelected.AddListener(OnTargetNodeSelected);

        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        buttonStartBattle.onClick.AddListener(OnButtonStartBattleClicked);
    }

    public void SetDreampostData(DreampostData dreampostData)
    {
        elementDreamflowMap.SetDreampost(dreampostData);
    }

    public void OnTargetNodeSelected(DreamflowNodeData nodeData)
    {
        node = nodeData;

        var spot = MSCommon.GetSpotTypeFromSchedule(nodeData.schedule);
        var spotData = MSDataManager.Get().spotDatas.Find(x => x.type == spot);

        spotImage.sprite = spotData.spotSprite;

        for (var i = 0; i < 3; i++)
        {
            elementMonsters[i].SetFigureData(nodeData.monsters[i]);
        }
    }

    public void OnButtonReturnClicked()
    {
        UGUIManager.Get().OpenPopUp(PopUpType.DreamflowExitMessage);
    }
    public void OnButtonStartBattleClicked()
    {
        manager.OpenLayout(LayoutType.DreamBattle);
        CloseLayout();

        var battleController = UGUIManager.Get().GetComponent<MSBattleController>();
        battleController.SetDreamflowNodeData(node);
    }

    public void SetBattleEnded()
    {
        elementDreamflowMap.SetNextNodeAsCurrentNode();
    }
    public void SetRewardPopUp()
    {
        elementDreamflowMap.SetRewardPopUp(); 
    }
}
