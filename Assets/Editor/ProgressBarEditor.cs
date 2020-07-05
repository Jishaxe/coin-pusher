using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressBarController))]
public class ProgressBarEditor : Editor
{
    private float _targetValue;
    private bool _skipAnimation;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Test value");
            _targetValue = EditorGUILayout.FloatField(_targetValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Skip animation");
            _skipAnimation = EditorGUILayout.Toggle(_skipAnimation);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Set test value"))
            {
                var element = (ProgressBarController) target;
                
                element.SetValue(_targetValue, _skipAnimation);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play mode to enter test values here", MessageType.Info);
        }
    }
}
