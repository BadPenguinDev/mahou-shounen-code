using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class DialogueGraph : EditorWindow
{
    private DialogueContainer TargetDialogueContainer;
    private DialogueGraphView GraphView;

    private Toolbar TargetToolBar;

    [MenuItem("Assets/Create/Dialogue Container")]
    public static void CreateDialogueContainer()
    {
        // Set Vaild Path
        string targetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID) + "/New Dialogue.asset";
        for (int i = 1; i < 1024; i++)
        {
            if (File.Exists(targetPath))
                targetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID) + "/New Dialogue " + i + ".asset";
            else
                break;
        }

        // Create String Table Asset
        DialogueContainer container = CreateInstance<DialogueContainer>();

        AssetDatabase.CreateAsset(container, targetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = container;
    }

    [MenuItem("Window/Dialogue Graph")]
    public static DialogueGraph OpenDialogueGraphWindow()
    {
        DialogueGraph window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
        return window;
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var container = EditorUtility.InstanceIDToObject(instanceID) as DialogueContainer;
        if (container != null)
        {
            var window = OpenDialogueGraphWindow();
            window.TargetDialogueContainer = container;
            window.InitializeContainer();

            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        ConstructGraphView();
    }
    private void OnDisable()
    {
        rootVisualElement.Remove(GraphView);
    }

    private void OnGUI()
    {
        // Toolbar
        EditorGUI.LabelField(new Rect(2, 1, position.width, 18), AssetDatabase.GetAssetPath(TargetDialogueContainer));
        if (GUI.Button(new Rect(position.width - 71, 1, 72, 18), "Save Data", new GUIStyle("ToolbarButton")))
            RequestSaveData();
        EditorGUI.DrawRect(new Rect(0, 20, position.width, 1), new Color(0.125f, 0.125f, 0.125f));
    }

    private void InitializeContainer()
    {
        if (TargetDialogueContainer == null)
            TargetDialogueContainer = CreateInstance<DialogueContainer>();
        else
            DialogueGraphSaveUtility.GetInstance(GraphView).LoadGraph(TargetDialogueContainer);

        EditorUtility.SetDirty(TargetDialogueContainer);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void ConstructGraphView()
    {
        GraphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        GraphView.StretchToParentSize();
        GraphView.style.marginTop = 21;
        rootVisualElement.Add(GraphView);
    }

    private void RequestSaveData()
    {
        DialogueGraphSaveUtility.GetInstance(GraphView).SaveGraph(TargetDialogueContainer);
    }
}
