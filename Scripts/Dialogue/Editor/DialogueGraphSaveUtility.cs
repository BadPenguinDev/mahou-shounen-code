using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DialogueGraphSaveUtility
{
    private DialogueGraphView TargetGraphView;
    private DialogueContainer ContainerCache; 

    private List<Edge> Edges => TargetGraphView.edges.ToList();
    private List<DialogueNodeBase> Nodes => TargetGraphView.nodes.ToList().Cast<DialogueNodeBase>().ToList();

    public static DialogueGraphSaveUtility GetInstance(DialogueGraphView targetViewGraph)
    {
        return new DialogueGraphSaveUtility
        {
            TargetGraphView = targetViewGraph
        };
    }

    public void SaveGraph(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any())
            return;

        dialogueContainer.LinkDatas.Clear();
        dialogueContainer.NodeDatas.Clear();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNodeBase;
            var inputNode = connectedPorts[i].input.node as DialogueNodeBase;

            DialogueLinkData data = new DialogueLinkData();
            data.PortName = connectedPorts[i].output.portName;
            data.BaseNodeGUID = outputNode.GUID;
            data.TargetNodeGUID = inputNode.GUID;
            dialogueContainer.LinkDatas.Add(data);
        }
        
        foreach (var node in Nodes)
        {
            if (node.GetType() == typeof(EntryPointNode))
            {
                EntryPointNode entryPointNode = (EntryPointNode)node;
                EntryPointNodeData nodeData = new EntryPointNodeData()
                {
                    GUID = entryPointNode.GUID,
                };
                dialogueContainer.NodeDatas.Add(nodeData);
                continue;
            }
            if (node.GetType() == typeof(ExitPointNode))
            {
                ExitPointNode exitPointNode = (ExitPointNode)node;
                ExitPointNodeData nodeData = new ExitPointNodeData()
                {
                    GUID = exitPointNode.GUID,
                    Position = exitPointNode.GetPosition().position,
                };
                dialogueContainer.NodeDatas.Add(nodeData);
                continue;
            }
            if (node.GetType() == typeof(DialogueNode))
            {
                DialogueNode dialogueNode = (DialogueNode)node;
                List<DialogueDecoratorData> decorators = new List<DialogueDecoratorData>();
                foreach (var decorator in dialogueNode.Decorators)
                {
                    if (decorator.GetType() == typeof(DialogueDecoratorSpeech))
                    {
                        DialogueDecoratorSpeech speechDecorator = (DialogueDecoratorSpeech)decorator;
                        DialogueDecoratorSpeechData decoratorData = new DialogueDecoratorSpeechData()
                        {
                            PortraitType = speechDecorator.PortraitType,
                            EmotionType = speechDecorator.EmotionType,
                            PortraitOrientation = speechDecorator.PortraitOrientation
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorFrameCG))
                    {
                        DialogueDecoratorFrameCG frameCGDecorator = (DialogueDecoratorFrameCG)decorator;
                        DialogueDecoratorFrameCGData decoratorData = new DialogueDecoratorFrameCGData()
                        {
                            SpriteFrameCG = frameCGDecorator.SpriteFrameCG
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorFullCG))
                    {
                        DialogueDecoratorFullCG frameCGDecorator = (DialogueDecoratorFullCG)decorator;
                        DialogueDecoratorFullCGData decoratorData = new DialogueDecoratorFullCGData()
                        {
                            SpriteFullCG = frameCGDecorator.SpriteFullCG,
                            isIgnoreDialogue = frameCGDecorator.isIgnoreDialogue
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorAnimationCG))
                    {
                        DialogueDecoratorAnimationCG animationCGDecorator = (DialogueDecoratorAnimationCG)decorator;
                        DialogueDecoratorAnimationCGData decoratorData = new DialogueDecoratorAnimationCGData()
                        {
                            AnimationCG = animationCGDecorator.AnimationCG,
                            isIgnoreDialogue = animationCGDecorator.isIgnoreDialogue
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorTransition))
                    {
                        DialogueDecoratorTransition transitionDecorator = (DialogueDecoratorTransition)decorator;
                        DialogueDecoratorTransitionData decoratorData = new DialogueDecoratorTransitionData()
                        {
                            Transition = transitionDecorator.Transition
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorPlaySFX))
                    {
                        var playSFXDecorator = (DialogueDecoratorPlaySFX)decorator;
                        var decoratorData = new DialogueDecoratorPlaySFXData()
                        {
                            clip = playSFXDecorator.clip
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorPlayMusic))
                    {
                        var playMusicDecorator = (DialogueDecoratorPlayMusic)decorator;
                        var decoratorData = new DialogueDecoratorPlayMusicData()
                        {
                            clip = playMusicDecorator.clip
                        };
                        decorators.Add(decoratorData);
                    }
                    else if (decorator.GetType() == typeof(DialogueDecoratorCameraShake))
                    {
                        DialogueDecoratorCameraShake animationCGDecorator = (DialogueDecoratorCameraShake)decorator;
                        DialogueDecoratorCameraShakeData decoratorData = new DialogueDecoratorCameraShakeData();
                        decorators.Add(decoratorData);
                    }
                }

                DialogueNodeData nodeData = new DialogueNodeData()
                {
                    GUID = dialogueNode.GUID,
                    DialogueKey = dialogueNode.DialogueKey,
                    Position = dialogueNode.GetPosition().position,
                    DecoratorList = decorators
                };
                dialogueContainer.NodeDatas.Add(nodeData);
                continue;
            }
            if (node.GetType() == typeof(BranchNode))
            {
                BranchNode dialogueNode = (BranchNode)node;
                BranchNodeData nodeData = new BranchNodeData()
                {
                    GUID = dialogueNode.GUID,
                    Position = dialogueNode.GetPosition().position,
                };
                dialogueContainer.NodeDatas.Add(nodeData);
                continue;
            }
        }

        EditorUtility.SetDirty(dialogueContainer);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void LoadGraph(DialogueContainer container)
    {
        ContainerCache = container;
        if (ContainerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found.", "Target dialogue graph file does not exists!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void CreateNodes()
    {
        foreach(var nodeData in ContainerCache.NodeDatas)
        {
            if (nodeData.GetType() == typeof(ExitPointNodeData))
            {
                ExitPointNodeData exitPointNodeData = (ExitPointNodeData)nodeData;
                ExitPointNode node = (ExitPointNode)Nodes.Find(x => x.GetType() == typeof(ExitPointNode));
                node.SetPosition(exitPointNodeData.Position);

                continue;
            }
            if (nodeData.GetType() == typeof(DialogueNodeData))
            {
                DialogueNodeData dialogueNodeData = (DialogueNodeData)nodeData;
                var tempNode = TargetGraphView.CreateDialogueNode(dialogueNodeData.DialogueKey, dialogueNodeData.Position);
                tempNode.GUID = nodeData.GUID;
                TargetGraphView.AddElement(tempNode);

                foreach (var decoratorData in dialogueNodeData.DecoratorList)
                {
                         if (decoratorData.GetType() == typeof(DialogueDecoratorSpeechData))
                    {
                        DialogueDecoratorSpeechData speachDecoratorData = (DialogueDecoratorSpeechData)decoratorData;
                        DialogueDecoratorSpeech decorator = new DialogueDecoratorSpeech(tempNode);

                        decorator.SetPortraitType(speachDecoratorData.PortraitType);
                        decorator.SetEmotionType(speachDecoratorData.EmotionType);
                        decorator.SetPortraitOrientation(speachDecoratorData.PortraitOrientation);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorFrameCGData))
                    {
                        DialogueDecoratorFrameCGData frameCGDecoratorData = (DialogueDecoratorFrameCGData)decoratorData;
                        DialogueDecoratorFrameCG decorator = new DialogueDecoratorFrameCG(tempNode);

                        decorator.SetSpriteFrameCG(frameCGDecoratorData.SpriteFrameCG);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorFullCGData))
                    {
                        DialogueDecoratorFullCGData fullCGDecoratorData = (DialogueDecoratorFullCGData)decoratorData;
                        DialogueDecoratorFullCG decorator = new DialogueDecoratorFullCG(tempNode);

                        decorator.SetSpriteFullCG(fullCGDecoratorData.SpriteFullCG);
                        decorator.SetIgnoreDialogue(fullCGDecoratorData.isIgnoreDialogue);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorAnimationCGData))
                    {
                        DialogueDecoratorAnimationCGData animationCGDecoratorData = (DialogueDecoratorAnimationCGData)decoratorData;
                        DialogueDecoratorAnimationCG decorator = new DialogueDecoratorAnimationCG(tempNode);

                        decorator.SetAnimationCG(animationCGDecoratorData.AnimationCG);
                        decorator.SetIgnoreDialogue(animationCGDecoratorData.isIgnoreDialogue);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorTransitionData))
                    {
                        DialogueDecoratorTransitionData animationCGDecoratorData = (DialogueDecoratorTransitionData)decoratorData;
                        DialogueDecoratorTransition decorator = new DialogueDecoratorTransition(tempNode);
                    
                        decorator.SetAnimationCG(animationCGDecoratorData.Transition);
                    
                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorPlaySFXData))
                    {
                        var playSFXDecoratorData = (DialogueDecoratorPlaySFXData)decoratorData;
                        var decorator = new DialogueDecoratorPlaySFX(tempNode);

                        decorator.SetAudioClip(playSFXDecoratorData.clip);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorPlayMusicData))
                    {
                        var playMusicDecoratorData = (DialogueDecoratorPlayMusicData)decoratorData;
                        var decorator = new DialogueDecoratorPlayMusic(tempNode);

                        decorator.SetAudioClip(playMusicDecoratorData.clip);

                        tempNode.AddDecorator(decorator);
                    }
                    else if (decoratorData.GetType() == typeof(DialogueDecoratorCameraShakeData))
                    {
                        DialogueDecoratorCameraShakeData animationCGDecoratorData = (DialogueDecoratorCameraShakeData)decoratorData;
                        DialogueDecoratorCameraShake decorator = new DialogueDecoratorCameraShake(tempNode);
                    
                        tempNode.AddDecorator(decorator);
                    }
                }

                continue;
            }
            if (nodeData.GetType() == typeof(BranchNodeData))
            {
                var tempNode = TargetGraphView.CreateBranchNode(nodeData.Position);
                tempNode.GUID = nodeData.GUID;
                TargetGraphView.AddElement(tempNode);
                
                var nodePorts = ContainerCache.LinkDatas.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
                nodePorts.ForEach(x => tempNode.AddBranchPort(x.PortName));

                continue;
            }
        }
    }
    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = ContainerCache.LinkDatas.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
        }
    }
    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
    
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
    
        TargetGraphView.Add(tempEdge);
    }
    private void ClearGraph()
    {
        foreach (var node in Nodes)
        {
            if (node.GetType() == typeof(EntryPointNode))
                continue;

            // 노드 연결 해제
            Edges.Where(x => x.input.node == node).ToList().
                ForEach(edge => TargetGraphView.RemoveElement(edge));

            // 노드 삭제
            if (node.GetType() != typeof(ExitPointNode))
                TargetGraphView.RemoveElement(node);
        }
    }
}
