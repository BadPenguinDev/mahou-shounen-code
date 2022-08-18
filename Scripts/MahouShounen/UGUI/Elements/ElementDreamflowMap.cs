using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OnTargetNodeSelectedEvent : UnityEvent<DreamflowNodeData> { };

public class ElementDreamflowMap : ElementBase
{
    [SerializeField] GameObject prefabElementNode;

    [SerializeField] ElementDreamflowNode elementNodeHome;
    [SerializeField] ElementDreamflowNode elementNodeBoss;

    [SerializeField] List<ElementDreamflowNode> elementNodeColumn0;
    [SerializeField] List<ElementDreamflowNode> elementNodeColumn1;
    [SerializeField] List<ElementDreamflowNode> elementNodeColumn2;

    [SerializeField] ElementDreamflowNodeSelector elementNodeSelector;

    [SerializeField] ElementDreamflowNode currentElementNode;
    [SerializeField] ElementDreamflowNode selectedElementNode;

    bool isMoveToNextNode;

    public OnTargetNodeSelectedEvent onTargetNodeSelected;

    DreampostData dreampost;

    // Start is called before the first frame update
    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(this);

        onTargetNodeSelected = new OnTargetNodeSelectedEvent();

        var nodes = new List<ElementDreamflowNode>();
        nodes.Add(elementNodeHome);
        nodes.AddRange(elementNodeColumn0);
        nodes.AddRange(elementNodeColumn1);
        nodes.AddRange(elementNodeColumn2);
        nodes.Add(elementNodeBoss);

        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].SetParentComponent(this);
            nodes[i].onElementDreamflowNodeClicked.AddListener(SelectNode);
        }
    }

    void OnEnable()
    {
        if (isMoveToNextNode)
        {
            var nextNodes = currentElementNode.GetNextNodeElements();
            var pathIndex = nextNodes.IndexOf(selectedElementNode);
            currentElementNode.SetNodePassed(pathIndex);

            isMoveToNextNode = false;
        }

        if (selectedElementNode != null)
            SetCurrentNode(selectedElementNode);
    }

    public void SetDreampost(DreampostData dreampostData)
    {
        dreampost = dreampostData;

        InitializeElementNode(elementNodeHome, dreampost.homeNode, 1, false);
        for (var i = 0; i < 3; i++)
        {
            InitializeElementNode(elementNodeColumn0[i], dreampost.nodeColumns[0].nodes[i], 2);
            InitializeElementNode(elementNodeColumn1[i], dreampost.nodeColumns[1].nodes[i], 3);
            InitializeElementNode(elementNodeColumn2[i], dreampost.nodeColumns[2].nodes[i], 4);
        }
        InitializeElementNode(elementNodeBoss, dreampost.bossNode, -1, false);

        selectedElementNode = elementNodeHome;
        currentElementNode = null;
    }
    public void InitializeElementNode(ElementDreamflowNode element, DreamflowNodeData data, int column, bool isIconOverriding = true)
    {
        element.SetNodeData(data, column == 4, isIconOverriding);
        element.SetNextNodeElements(GetNextNodeElements(column, data));
        element.SetDreamflowNodeStatus(DreamflowNodeStatus.Unreachable);
    }
    public List<ElementDreamflowNode> GetNextNodeElements(int column, DreamflowNodeData data)
    {
        if (column < 0)
            return null;

        if (!data.isActive)
            return null;

        var nodes = new List<ElementDreamflowNode>();
        if (column >= 4)
        {
            nodes.Add(elementNodeBoss);
        }
        else
        {
            var originalNodes = new List<ElementDreamflowNode>();
            switch (column)
            {
                case 1:     originalNodes = elementNodeColumn0; break;
                case 2:     originalNodes = elementNodeColumn1; break;
                case 3:     originalNodes = elementNodeColumn2; break;
            }

            for (var i = 0; i < 3; i++)
            {
                if (data.connections[i])
                    nodes.Add(originalNodes[i]);
            }
        }
        return nodes;
    }

    public void SetNextNodeAsCurrentNode()
    {
        isMoveToNextNode = true;
    }
    public void SetCurrentNode(ElementDreamflowNode node)
    {
        // Prev Nodes
        if (currentElementNode)
        {
            currentElementNode.SetCurrentNode(false);
            currentElementNode.SetDreamflowNodeStatus(DreamflowNodeStatus.Passed);

            List<ElementDreamflowNode> prevNextNodes = currentElementNode.GetNextNodeElements();
            for (int i = 0; i < prevNextNodes.Count; i++)
            {
                prevNextNodes[i].SetDreamflowNodeStatus(DreamflowNodeStatus.Unreachable);
            }
        }

        // Current Node
        currentElementNode = node;
        currentElementNode.SetCurrentNode(true);
        currentElementNode.SetDreamflowNodeStatus(DreamflowNodeStatus.Current);

        // Check Boss Node
        if (currentElementNode == elementNodeBoss)
        {
            if (!MSEventManager.Get().PlayDreampostVictoryEvent())
            {
                SetRewardPopUp();
            }
        }

        // Next Nodes
        var nextNodes = currentElementNode.GetNextNodeElements();
        if (nextNodes == null)
            return;

        if (nextNodes.Count < 0)
            return;

        foreach (var element in nextNodes)
        {
            element.SetDreamflowNodeStatus(DreamflowNodeStatus.Next);
        }

        selectedElementNode = null;
        SelectNode(nextNodes[0]);
    }
    
    public void SelectNode(ElementDreamflowNode element)
    {
        if (selectedElementNode)
            selectedElementNode.SetDreamflowNodeStatus(DreamflowNodeStatus.Next);

        selectedElementNode = element;

        var data = element.GetNodeData();
        element.SetDreamflowNodeStatus(DreamflowNodeStatus.Selected);

        elementNodeSelector.StartMove(element.transform as RectTransform);

        onTargetNodeSelected.Invoke(data);
    }

    public void SetRewardPopUp()
    {
        var popUp = UGUIManager.Get().OpenPopUp(PopUpType.DreamflowReward) as PopUpDreamflowReward;
        popUp.SetDreampostData(dreampost);
    }
}
