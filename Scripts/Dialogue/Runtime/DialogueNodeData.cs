using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DialogueNodeDataBase
{
    public string GUID;
    public Vector2 Position;
}

[System.Serializable]
public class DialogueNodeData : DialogueNodeDataBase
{
    public string DialogueKey;

    [SerializeReference]
    public List<DialogueDecoratorData> DecoratorList;
}

[System.Serializable]
public class BranchNodeData : DialogueNodeDataBase
{

}

[System.Serializable]
public class EntryPointNodeData : DialogueNodeDataBase
{

}

[System.Serializable]
public class ExitPointNodeData : DialogueNodeDataBase
{

}

[System.Serializable]
public class DialogueLinkData
{
    public string PortName;
    public string BaseNodeGUID;
    public string TargetNodeGUID;
}

