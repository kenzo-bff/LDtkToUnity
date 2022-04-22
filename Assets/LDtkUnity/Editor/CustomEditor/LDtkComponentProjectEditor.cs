﻿using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor
{
    [CustomEditor(typeof(LDtkComponentProject), true)]
    internal class LDtkComponentProjectEditor : UnityEditor.Editor
    {
        private static readonly GUIContent ProjectContent = new GUIContent
        {
            text = "Json Data",
            tooltip = "Reference to the Json. Call FromJson in this component to get it's data"
        };
        
        private static readonly GUIContent LevelsContent = new GUIContent
        {
            text = "This project uses separate level files. The levels are available from their separate levels.",
        };

        public override void OnInspectorGUI()
        {
            SerializedProperty projectProp = serializedObject.FindProperty(LDtkComponentProject.PROPERTY_PROJECT);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(projectProp, ProjectContent);
            }
            
            SerializedProperty levelsProp = serializedObject.FindProperty(LDtkComponentProject.PROPERTY_SEPARATE_LEVELS);
            if (levelsProp.boolValue)
            {
                EditorGUILayout.HelpBox(LevelsContent, LDtkIconUtility.LoadLevelFileIcon());
            }
        }
    }
}