using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    protected ReorderableList taskList;
    
    float lineHeight;
    float lineHeightSpace;

    protected void OnEnable()
    {
        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 10;

        var tasksProperty = serializedObject.FindProperty("_tasks");
        taskList = new ReorderableList(serializedObject, tasksProperty, true, true, true, true)
        {
            elementHeight = 22,
            drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Tasks")
        };
        taskList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = taskList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, rect.width, rect.height - 3), element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        serializedObject.Update();

        OnInspectorGUICommon(true);
        EditorGUILayout.Space();
        
        taskList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnInspectorGUICommon(bool isBaseTasksVisible = false)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("ID");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_id"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
    }
}
