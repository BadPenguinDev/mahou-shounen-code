using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class DialogueDecoratorSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueNode Node;
    private Texture2D IdentationIcon;

    public void Init(DialogueNode node)
    {
        Node = node;

        IdentationIcon = new Texture2D(1, 1);
        IdentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        IdentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Decorators"), 0),
            new SearchTreeEntry(new GUIContent("Speach",       IdentationIcon))  { userData = DialogueDecoratorType.Speech,      level = 1 },
            new SearchTreeEntry(new GUIContent("Frame CG",     IdentationIcon))  { userData = DialogueDecoratorType.FrameCG,     level = 1 },
            new SearchTreeEntry(new GUIContent("Full CG",      IdentationIcon))  { userData = DialogueDecoratorType.FullCG,      level = 1 },
            new SearchTreeEntry(new GUIContent("Animation CG", IdentationIcon))  { userData = DialogueDecoratorType.AnimationCG, level = 1 },
            new SearchTreeEntry(new GUIContent("Transition",   IdentationIcon))  { userData = DialogueDecoratorType.Transition,  level = 1 },
            new SearchTreeEntry(new GUIContent("Play SFX",     IdentationIcon))  { userData = DialogueDecoratorType.PlaySFX,     level = 1 },
            new SearchTreeEntry(new GUIContent("Play Music",   IdentationIcon))  { userData = DialogueDecoratorType.PlayMusic,   level = 1 },
            new SearchTreeEntry(new GUIContent("CameraShake",  IdentationIcon))  { userData = DialogueDecoratorType.CameraShake, level = 1 }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        DialogueDecoratorType type = (DialogueDecoratorType)SearchTreeEntry.userData;
        return Node.AddDecorator(type);
    }
}