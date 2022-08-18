using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(WeekendEventData))]
public class WeekendEventDataEditor : EventDataEditor
{
    Texture2D textureWeekendOption;
    Sprite prevWeekendOptionSprite;
    
    const int WEEKEND_OPTION_TEXTURE_WIDTH  = 64;
    const int WEEKEND_OPTION_TEXTURE_HEIGHT = 88;

    protected new void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        serializedObject.Update();

        OnInspectorGUIPriority();
        OnInspectorGUIWeekend();
        conditionList.DoLayoutList();
        OnInspectorGUIDialogue();

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnInspectorGUIWeekend()
    {
        EditorGUILayout.BeginHorizontal();
        {
            // Weekend Event Data and Make Texture
            WeekendEventData weekendEventData = target as WeekendEventData;
            if (prevWeekendOptionSprite != weekendEventData.option.sprite)
            {
                prevWeekendOptionSprite = weekendEventData.option.sprite;
                textureWeekendOption = GetWeekendOptionTexture(prevWeekendOptionSprite);
            }

            if (textureWeekendOption == null)
                textureWeekendOption = GetWeekendOptionTexture(null);

            // GUI Style
            GUIStyle weekendOptionSpriteStyle = new GUIStyle();
            weekendOptionSpriteStyle.normal.background = textureWeekendOption;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(WEEKEND_OPTION_TEXTURE_WIDTH + 4));
            EditorGUILayout.LabelField(GUIContent.none, weekendOptionSpriteStyle, GUILayout.Width(WEEKEND_OPTION_TEXTURE_WIDTH), GUILayout.Height(WEEKEND_OPTION_TEXTURE_HEIGHT));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            {
                SerializedProperty element = serializedObject.FindProperty("_option");

                // Item Sprite
                Sprite sprite = (Sprite)element.FindPropertyRelative("_sprite").objectReferenceValue;

                // Spot Type Property
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
                    EditorGUILayout.LabelField("Spot Type", GUILayout.MaxWidth(96), GUILayout.ExpandWidth(false));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("_spotType"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();

                // Event Data Property
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(72));
                    EditorGUILayout.LabelField("Sprite", GUILayout.MaxWidth(96));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("_sprite"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();

                // Event Data Property
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
                    EditorGUILayout.LabelField("Position", GUILayout.MaxWidth(84));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("_position"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();

                // Space
                EditorGUILayout.LabelField(GUIContent.none);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(GUIContent.none);
    }

    Texture2D GetWeekendOptionTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D(WEEKEND_OPTION_TEXTURE_WIDTH, WEEKEND_OPTION_TEXTURE_HEIGHT);

        // Set Background White
        for (int x = 0; x < WEEKEND_OPTION_TEXTURE_WIDTH; x++)
        {
            for (int y = 0; y < WEEKEND_OPTION_TEXTURE_HEIGHT; y++)
            {
                if (((x / 8) % 2) - ((y / 8) % 2) == 0)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, new Color(0.75f, 0.75f, 0.75f, 1.0f));
            }
        }

        if (sprite != null)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                int xStart = (WEEKEND_OPTION_TEXTURE_WIDTH  - (int)sprite.rect.width)  / 2 + (int)sprite.textureRectOffset.x;
                int yStart = (WEEKEND_OPTION_TEXTURE_HEIGHT - (int)sprite.rect.height) / 2 + (int)sprite.textureRectOffset.y;

                for (int x = 0; x < (int)sprite.textureRect.width; x++)
                {
                    if (x + xStart < 0 ||
                        x + xStart >= WEEKEND_OPTION_TEXTURE_WIDTH)
                        continue;

                    for (int y = 0; y < (int)sprite.textureRect.height; y++)
                    {
                        if (y + yStart < 0 ||
                            y + yStart >= WEEKEND_OPTION_TEXTURE_HEIGHT)
                            continue;

                        var color = Color.Lerp(texture.GetPixel(x + xStart, y + yStart),
                                               sprite.texture.GetPixel((int)sprite.textureRect.x + x, (int)sprite.textureRect.y + y),
                                               sprite.texture.GetPixel((int)sprite.textureRect.x + x, (int)sprite.textureRect.y + y).a);

                        texture.SetPixel(x + xStart, y + yStart, color);
                    }
                }
            }
            else
            {
                int xStart = (WEEKEND_OPTION_TEXTURE_WIDTH  - sprite.texture.width) / 2;
                int yStart = (WEEKEND_OPTION_TEXTURE_HEIGHT - sprite.texture.height) / 2;

                for (int x = 0; x < sprite.texture.width; x++)
                {
                    if (x + xStart < 0 ||
                        x + xStart >= WEEKEND_OPTION_TEXTURE_WIDTH)
                        continue;

                    for (int y = 0; y < sprite.texture.height; y++)
                    {
                        if (y + yStart < 0 ||
                            y + yStart >= WEEKEND_OPTION_TEXTURE_HEIGHT)
                            continue;

                        var color = Color.Lerp(texture.GetPixel(x + xStart, y + yStart),
                                               sprite.texture.GetPixel(x, y),
                                               sprite.texture.GetPixel(x, y).a);

                        texture.SetPixel(x + xStart, y + yStart, color);
                    }
                }
            }
        }

        texture.Apply();
        return texture;
    }
}
