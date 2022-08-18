using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(PlayerSkillData))]
public class PlayerSkillDataEditor : SkillDataEditor
{
    Dictionary<SkillRank, PlayerSkillRankDataEditor> rankDataEditors;
    bool isRankDataVisible = true;
    
    protected new void OnEnable()
    {
        base.OnEnable();

        rankDataEditors = new Dictionary<SkillRank, PlayerSkillRankDataEditor>();
        
        var rankDatasProperty = serializedObject.FindProperty("_rankDatas");
        for (var i = 0; i < rankDatasProperty.arraySize; i++)
        {
            var rankDataEditor = new PlayerSkillRankDataEditor();
            
            var rankDataProperty = rankDatasProperty.GetArrayElementAtIndex(i);
            var rank = (SkillRank)rankDataProperty.FindPropertyRelative("_rank").enumValueIndex;

            rankDataEditor.property = rankDataProperty;
            
            var tasksProperty = rankDataProperty.FindPropertyRelative("_tasks");
            var taskList = new ReorderableList(serializedObject, tasksProperty, true, true, true, true)
            {
                elementHeight = 22,
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Tasks")
            };
            taskList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = taskList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, rect.width, rect.height - 3), element, GUIContent.none);
            };
            rankDataEditor.taskList = taskList;
            
            var upgradeProperty = rankDataProperty.FindPropertyRelative("_upgradeRequirements");
            var upgradeList = new ReorderableList(serializedObject, upgradeProperty, true, true, true, true)
            {
                elementHeight = 42,
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Upgrade Requirements")
            };
            upgradeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = upgradeList.serializedProperty.GetArrayElementAtIndex(index);
                
                // Item Data Property
                var itemDataProperty = element.FindPropertyRelative("_itemData");
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 64, 18), "Item Data");
                EditorGUI.PropertyField(new Rect(rect.x + 10 + 64, rect.y + 2, 
                    rect.width - (10 + 64), 18), itemDataProperty, GUIContent.none);

                // Item Count Property
                var giftTasteTypeProperty = element.FindPropertyRelative("_itemCount");
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 20, 64, 18), "Count");
                EditorGUI.PropertyField(new Rect(rect.x + 10 + 64, rect.y + 22, 
                    rect.width - (10 + 64), 18), giftTasteTypeProperty, GUIContent.none);
            };
            rankDataEditor.upgradeList = upgradeList;
            
            rankDataEditor.foldOutState = true;
            rankDataEditors.Add(rank, rankDataEditor);
        }
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        serializedObject.Update();

        OnInspectorGUICommon(false);
        EditorGUILayout.Space();
        
        OnInspectorGUIRank();

        EditorGUILayout.Space();
        OnInspectorGUIPlayer();

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnInspectorGUIRank()
    {
        var headerStyle = GUI.skin.label;
        headerStyle.fontSize = 12;
        headerStyle.richText = true;
        EditorGUILayout.LabelField("<b>Rank Datas</b>", headerStyle);

        foreach (var pair in rankDataEditors)
        {
            var rankDataEditor = pair.Value;
            
            rankDataEditor.foldOutState = EditorGUILayout.Foldout(rankDataEditor.foldOutState, pair.Key.ToString() + " Rank Data");
            if (!rankDataEditor.foldOutState) 
                continue;
            
            // Header Style
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
                    EditorGUILayout.LabelField("Heat Value");
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(rankDataEditor.property.FindPropertyRelative("_heatValue"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal(); 
                
                if (pair.Key != SkillRank.S)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
                        EditorGUILayout.LabelField("EXP");
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(rankDataEditor.property.FindPropertyRelative("_EXP"), GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndHorizontal(); 
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
            rankDataEditor.taskList.DoLayoutList();
            EditorGUILayout.Space(); 
            
            if (pair.Key != SkillRank.S)
                rankDataEditor.upgradeList.DoLayoutList();
        }
    }
    
    protected void OnInspectorGUIPlayer()
    {
        var headerStyle = GUI.skin.label;
        headerStyle.fontSize = 12;
        headerStyle.richText = true;
        EditorGUILayout.LabelField("<b>UI Element Options</b>", headerStyle);
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("Icon");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_iconSprite"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal(); 
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("Palette");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_palette"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal(); 
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(84));
            EditorGUILayout.LabelField("Name");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_skillName"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal(); 
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(84));
            EditorGUILayout.LabelField("Description");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_skillDesc"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal(); 
    }
}

[System.Serializable]
public class PlayerSkillRankDataEditor
{
    public SerializedProperty property;
    public ReorderableList taskList;
    public ReorderableList upgradeList;
    public bool foldOutState;
}
