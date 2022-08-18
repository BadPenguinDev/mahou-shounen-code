using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DialogueContainer : ScriptableObject
{
    [SerializeField]
    private bool _playTransition;
    public  bool  playTransition
    {
        get { return  _playTransition; }
        private set { _playTransition = value; }
    }

    [SerializeReference]
    public List<DialogueNodeDataBase> NodeDatas = new List<DialogueNodeDataBase>();
    public List<DialogueLinkData> LinkDatas = new List<DialogueLinkData>();

    public EntryPointNodeData GetEntryPointNode()
    {
        foreach (var nodeData in NodeDatas)
        {
            if (nodeData.GetType() == typeof(EntryPointNodeData))
                return (EntryPointNodeData)nodeData;
        }
        return null;
    }
    public DialogueNodeDataBase GetNextNode(DialogueNodeDataBase start)
    {
        if (start == null)
        {
            return null;
        }

        foreach (var linkData in LinkDatas)
        {
            if (linkData.BaseNodeGUID == start.GUID)
                return GetNextNode(linkData);
        }
        return null;
    }
    public DialogueNodeDataBase GetNextNode(DialogueLinkData link)
    {
        return NodeDatas.Find(x => x.GUID == link.TargetNodeGUID);
    }
    public List<DialogueLinkData> GetBranches(BranchNodeData data)
    {
        List<DialogueLinkData> linkDatas = new List<DialogueLinkData>();
        foreach (var linkData in LinkDatas)
        {
            if (linkData.BaseNodeGUID == data.GUID)
                linkDatas.Add(linkData);
        }
        return linkDatas;
    }
}
