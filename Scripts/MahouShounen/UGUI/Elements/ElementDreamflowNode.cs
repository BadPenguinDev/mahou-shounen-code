using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public enum  DreamflowNodeStatus { Current, Next, Selected, Passed, Unreachable }
public class OnElementDreamflowNodeClickedEvent : UnityEvent<ElementDreamflowNode> { };

public class ElementDreamflowNode : ElementBase
{
    [Header("Node Data")]
    [SerializeField] Image iconSpot;
    [SerializeField] Image fieldNode;
    [SerializeField] Image nodeOutline;
    [SerializeField] List<Image> imagePathes;
    [SerializeField] List<Image> imagePathEffects;

    [Header("Element")]
    [SerializeField] float effectTimer;
    [SerializeField] Button button;

    [Header("Brushes")]
    [SerializeField] List<Sprite> spritePathNormal;
    [SerializeField] List<Sprite> spritePathPassed;
    [SerializeField] List<Sprite> spritePathSelected;

    [SerializeField] Sprite spriteFieldCurrent;
    [SerializeField] Sprite spriteFieldNext;
    [SerializeField] Sprite spriteFieldSelected;
    [SerializeField] Sprite spriteFieldPassed;
    [SerializeField] Sprite spriteFieldUnreachable;

    DreamflowNodeData node;
    DreamflowNodeStatus status;

    IEnumerator effectCoroutine;
    List<ElementDreamflowNode> nextNodeElements;

    public OnElementDreamflowNodeClickedEvent onElementDreamflowNodeClicked;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
        
        onElementDreamflowNodeClicked = new OnElementDreamflowNodeClickedEvent();
    }

    public void SetNodeData(DreamflowNodeData nodeData, bool isLastColumn = false, bool isIconOverriding = true)
    {
        node = nodeData;

        if (isIconOverriding)
        {
            ScheduleData scheduleData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == nodeData.schedule);
            iconSpot.sprite = scheduleData.spriteSchedule;
        }

        for (int i = 0; i < 3; i++)
        {
            if (nodeData != null)
            {
                if (!nodeData.isActive)
                {
                    gameObject.SetActive(false);
                    continue;
                }

                if (nodeData.connections[i] == false)
                    imagePathes[i].gameObject.SetActive(false);
                else
                    imagePathes[i].gameObject.SetActive(true);
            }

            imagePathes[i].sprite = spritePathNormal[i];
            imagePathEffects[i].gameObject.SetActive(false);
        }

        if (isLastColumn)
        {
            imagePathes[1].gameObject.SetActive(true);
        }
    }
    public void SetNextNodeElements(List<ElementDreamflowNode> nodes)
    {
        if (nextNodeElements != null)
            nextNodeElements.Clear();
        else
            nextNodeElements = new List<ElementDreamflowNode>();

        nextNodeElements = nodes;
    }

    public DreamflowNodeData GetNodeData() { return node; }
    public List<ElementDreamflowNode> GetNextNodeElements() { return nextNodeElements; }

    public void SetNodePassed(int index)
    {
        imagePathes[index].sprite = spritePathPassed[index];
    }
    public void SetDreamflowNodeStatus(DreamflowNodeStatus nodeStatus)
    {
        button.interactable = false;

        switch (nodeStatus)
        {
            case DreamflowNodeStatus.Current:
                fieldNode.sprite = spriteFieldCurrent;
                break;
            case DreamflowNodeStatus.Next:
                fieldNode.sprite = spriteFieldNext;
                button.interactable = true;
                break;
            case DreamflowNodeStatus.Selected:
                fieldNode.sprite = spriteFieldSelected;
                break;
            case DreamflowNodeStatus.Passed:
                fieldNode.sprite = spriteFieldPassed;
                break;
            case DreamflowNodeStatus.Unreachable:
                fieldNode.sprite = spriteFieldUnreachable;
                break;
        }
    }

    public void SetCurrentNode(bool status)
    {
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);

        if (status)
        {
            effectCoroutine = StartNodeEffect();
            StartCoroutine(effectCoroutine);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                imagePathEffects[i].gameObject.SetActive(false);
            }
            nodeOutline.gameObject.SetActive(false);
        }
    }

    public void SetNodeSelected()
    {
        onElementDreamflowNodeClicked.Invoke(this);
    }

    IEnumerator StartNodeEffect()
    {
        List<Image> targetEffects = new List<Image>();
        for (int i = 0; i < 3; i++)
        {
            if (node.connections[i])
            {
                targetEffects.Add(imagePathEffects[i]);

                imagePathEffects[i].sprite = spritePathSelected[i];
                imagePathEffects[i].gameObject.SetActive(true);
            }
        }
        nodeOutline.gameObject.SetActive(true);

        while (true)
        {
            // Glow On
            for (float t = 0; t < 1.0f; t += Time.deltaTime / (effectTimer * (3f / 8f)))
            {
                for (int i = 0; i < targetEffects.Count; i++)
                {
                    targetEffects[i].color = new Color(1f, 1f, 1f, t);
                }

                yield return null;
            }

            // Stay
            for (float t = 0; t < 1.0f; t += Time.deltaTime / (effectTimer * (1f / 8f)))
            {
                yield return null;
            }

            // Glow On
            for (float t = 0; t < 1.0f; t += Time.deltaTime / (effectTimer * (3f / 8f)))
            {
                for (int i = 0; i < targetEffects.Count; i++)
                {
                    targetEffects[i].color = new Color(1f, 1f, 1f, 1f - t);
                }

                yield return null;
            }

            // Stay
            for (float t = 0; t < 1.0f; t += Time.deltaTime / (effectTimer * (1f / 8f)))
            {
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
