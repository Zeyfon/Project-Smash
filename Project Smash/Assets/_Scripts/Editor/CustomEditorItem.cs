using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PSmash.Inventories;

[CustomEditor(typeof(CraftingItem))]
public class CustomEditorItem : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CraftingItem item = (CraftingItem)target;

        EditorGUILayout.BeginVertical();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Display Name");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"), GUIContent.none, true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Description");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), GUIContent.none, true,GUILayout.Height(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("UniqueIdentifier");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemID"), GUIContent.none, true);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sprite");
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"), GUIContent.none, true);
        Texture texture = null;
        if (item.sprite != null)
        {
            texture = item.sprite.texture;
        }
        GUILayout.Box(texture, GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
