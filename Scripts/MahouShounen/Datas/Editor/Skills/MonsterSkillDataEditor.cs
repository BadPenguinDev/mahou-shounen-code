using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(MonsterSkillData))]
public class MonsterSkillDataEditor : SkillDataEditor
{
    protected new void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        serializedObject.Update();

        OnInspectorGUICommon(true);
        OnInspectorGUIMonster();
        
        EditorGUILayout.Space();
        
        taskList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnInspectorGUIMonster()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("Timer");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_timer"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal(); 
    }
}
