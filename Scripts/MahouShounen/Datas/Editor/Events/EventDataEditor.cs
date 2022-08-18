using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(EventData))]
public class EventDataEditor : Editor
{
    protected EventData eventData;
    protected SerializedProperty conditionsProperty;

    protected ReorderableList conditionList;

    protected float lineHeight;
    protected float lineHeightSpace;

    protected void OnEnable()
    {
        if (target != null)
            eventData = (EventData)target;

        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 10;

        conditionsProperty = serializedObject.FindProperty("_conditions");
        conditionList = new ReorderableList(serializedObject, conditionsProperty, true, true, true, true);
        conditionList.elementHeight = 22;
        conditionList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Conditions");
        conditionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = conditionList.serializedProperty.GetArrayElementAtIndex(index);

            SerializedProperty scheduleTypeProperty = element.FindPropertyRelative("_type");
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, 84, rect.height),
                scheduleTypeProperty, GUIContent.none);

            string[] constantConditionString = { "  <", " <=", " ==", "  >", " >=", " !=" };
            string[]   eventComplitionString = { "완료", "미완" };

            EventConditionType eventConditionType = (EventConditionType)scheduleTypeProperty.intValue;
            switch (eventConditionType)
            {
                case EventConditionType.Date:
                    // EditorGUI.PropertyField(new Rect(rect.x + 88,  rect.y + 2, 120, rect.height),
                    //     element.FindPropertyRelative("_constantCondition"), GUIContent.none);
                    element.FindPropertyRelative("_constantCondition").enumValueIndex = (int)(ConstantCondition)EditorGUI.Popup(new Rect(rect.x + 88, rect.y + 2, 48, rect.height),
                                                                                                                                element.FindPropertyRelative("_constantCondition").enumValueIndex,
                                                                                                                                constantConditionString);
                    EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y + 2, 136, rect.height),
                        element.FindPropertyRelative("_month"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 280, rect.y + 2, 64, rect.height),
                        element.FindPropertyRelative("_week"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 348, rect.y + 2, rect.width - 348, rect.height),
                        element.FindPropertyRelative("_day"), GUIContent.none);
                    break;

                case EventConditionType.Stat:
                    element.FindPropertyRelative("_constantCondition").enumValueIndex = (int)(ConstantCondition)EditorGUI.Popup(new Rect(rect.x + 88, rect.y + 2, 48, rect.height),
                                                                                                                                element.FindPropertyRelative("_constantCondition").enumValueIndex,
                                                                                                                                constantConditionString);
                    EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y + 2, 136, rect.height),
                        element.FindPropertyRelative("_stat"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 280, rect.y + 2, rect.width - 280, 18),
                        element.FindPropertyRelative("_statValue"), GUIContent.none);
                    break;

                case EventConditionType.Schedule:
                    element.FindPropertyRelative("_constantCondition").enumValueIndex = (int)(ConstantCondition)EditorGUI.Popup(new Rect(rect.x + 88, rect.y + 2, 48, rect.height),
                                                                                                                                element.FindPropertyRelative("_constantCondition").enumValueIndex,
                                                                                                                                constantConditionString);
                    EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y + 2, 136, rect.height),
                        element.FindPropertyRelative("_schedule"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 280, rect.y + 2, rect.width - 280, 18),
                        element.FindPropertyRelative("_scheduleValue"), GUIContent.none);
                    break;

                case EventConditionType.Friendship:
                    element.FindPropertyRelative("_constantCondition").enumValueIndex = (int)(ConstantCondition)EditorGUI.Popup(new Rect(rect.x + 88, rect.y + 2, 48, rect.height),
                                                                                                                                element.FindPropertyRelative("_constantCondition").enumValueIndex,
                                                                                                                                constantConditionString);
                    EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y + 2, 136, rect.height),
                        element.FindPropertyRelative("_friendship"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x + 280, rect.y + 2, rect.width - 280, 18),
                        element.FindPropertyRelative("_friendshipValue"), GUIContent.none);
                    break;

                case EventConditionType.Event:
                    // EditorGUI.PropertyField(new Rect(rect.x + 88, rect.y + 2, 120, rect.height),
                    //     element.FindPropertyRelative("_eventComplition"), GUIContent.none);
                    element.FindPropertyRelative("_eventComplition").enumValueIndex = (int)(ConstantCondition)EditorGUI.Popup(new Rect(rect.x + 88, rect.y + 2, 48, rect.height),
                                                                                                                              element.FindPropertyRelative("_eventComplition").enumValueIndex,
                                                                                                                              eventComplitionString);
                    EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y + 2, rect.width - 140, 18), 
                        element.FindPropertyRelative("_eventData"), GUIContent.none);
                    break;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        serializedObject.Update();

        OnInspectorGUIPriority();
        conditionList.DoLayoutList();
        OnInspectorGUIDialogue();

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnInspectorGUIPriority()
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

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("Priority");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_priority"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
            EditorGUILayout.LabelField("Dialogue");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dialogueContainer"), GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(GUIContent.none);
    }
    protected void OnInspectorGUIDialogue()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (eventData.dialogueContainer != null)
        {
            DialogueNodeDataBase currentNodeData = eventData.dialogueContainer.GetEntryPointNode();
            currentNodeData = eventData.dialogueContainer.GetNextNode(currentNodeData);

            int lineCount = 0;
            while (true)
            {
                if (currentNodeData == null)
                    break;

                if (currentNodeData.GetType() == typeof(DialogueNodeData))
                {
                    // GUI Global
                    Color defaultColor = GUI.color; 

                    // Style
                    float colorValue = 0.25f - ((lineCount % 2) * 0.05f);

                    GUIStyle horizontalStyle = new GUIStyle();
                    Texture2D horizontalTexture = new Texture2D(1, 1);
                    horizontalTexture.SetPixel(0, 0, new Color(colorValue, colorValue, colorValue));
                    horizontalTexture.Apply();
                    horizontalStyle.normal.background = horizontalTexture;

                    // Dialogue Base
                    DialogueNodeData dialogueNodeData = (DialogueNodeData)currentNodeData;
                    EditorGUILayout.BeginHorizontal(horizontalStyle);
                    {
                        // Name
                        string nameString = "";
                        foreach (var decorator in dialogueNodeData.DecoratorList)
                        {
                            if (decorator.GetType() == typeof(DialogueDecoratorSpeechData))
                            {
                                GUIStyle textStyle = EditorStyles.boldLabel;
                                textStyle.alignment = TextAnchor.UpperLeft;

                                DialogueDecoratorSpeechData speechDecorator = (DialogueDecoratorSpeechData)decorator;
                                PortraitType portraitType = speechDecorator.PortraitType;

                                nameString += " ";
                                if (portraitType == PortraitType.Shounen ||
                                    portraitType == PortraitType.ShounenMagical)
                                {
                                    GUI.color = new Color(0.75f, 0.75f, 1.0f);
                                    nameString += MSGameInstance.Get().playerName;
                                }
                                else if (portraitType == PortraitType.Familiar ||
                                         portraitType == PortraitType.FamiliarHuman)
                                {
                                    GUI.color = new Color(1.00f, 0.75f, 0.75f);
                                    nameString += LocalizationSettings.StringDatabase.GetLocalizedString("Characters", portraitType.ToString());
                                }
                                else
                                {
                                    GUI.color = new Color(0.75f, 1.0f, 0.75f);
                                    nameString += LocalizationSettings.StringDatabase.GetLocalizedString("Characters", portraitType.ToString());
                                }
                                nameString += ": ";

                                Vector2 nameWidth = GUI.skin.label.CalcSize(new GUIContent(nameString));
                                EditorGUILayout.LabelField(nameString, textStyle, GUILayout.MinWidth(nameWidth.x), GUILayout.MaxWidth(nameWidth.x));

                                break;
                            }
                        }
                        GUI.color = defaultColor;

                        // Dialogue
                        string targetString = "";
                        if (nameString == "")
                        {
                            GUIStyle textStyle = EditorStyles.boldLabel;
                            textStyle.wordWrap = true;
                            textStyle.stretchWidth = true;
                            textStyle.alignment = TextAnchor.UpperLeft;

                            targetString += LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", dialogueNodeData.DialogueKey);
                            targetString = targetString.Replace("@p", MSGameInstance.Get().playerName);
                            EditorGUILayout.LabelField(targetString, textStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1024));
                        }
                        else
                        {
                            GUIStyle textStyle = EditorStyles.label;
                            textStyle.wordWrap = true;
                            textStyle.stretchWidth = true;
                            textStyle.alignment = TextAnchor.UpperLeft;

                            targetString += LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", dialogueNodeData.DialogueKey);
                            targetString = targetString.Replace("@p", MSGameInstance.Get().playerName);
                            EditorGUILayout.LabelField(targetString, textStyle, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(1024));
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    lineCount += 1;
                    currentNodeData = eventData.dialogueContainer.GetNextNode(currentNodeData);
                }
                else
                    break;
            }
        }
        EditorGUILayout.EndVertical();
    }
}

