using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class DialogueNodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView GraphView;
    private EditorWindow Window;
    private Texture2D IdentationIcon;

    public void Init(EditorWindow window, DialogueGraphView graphView) 
    {
        Window = window;
        GraphView = graphView;

        IdentationIcon = new Texture2D(1, 1);
        IdentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        IdentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 0),
            new SearchTreeEntry(new GUIContent("Dialogue Node", IdentationIcon)) { userData = DialogueNodeType.Dialogue, level = 1 },
            new SearchTreeEntry(new GUIContent("Branch Node", IdentationIcon)) { userData = DialogueNodeType.Branch, level = 1 }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = Window.rootVisualElement.ChangeCoordinatesTo(
            Window.rootVisualElement.parent, 
            context.screenMousePosition - Window.position.position);
        var localMousePosition = GraphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case DialogueNodeType.Dialogue:
                GraphView.CreateNode(DialogueNodeType.Dialogue, "Dialogue Key", localMousePosition);
                return true;
            case DialogueNodeType.Branch:
                GraphView.CreateNode(DialogueNodeType.Branch, "Dialogue Key", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
