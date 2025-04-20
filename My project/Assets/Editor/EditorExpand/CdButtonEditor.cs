
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CdButton))]
public class CdButtonEditor : Editor
{
    SerializedProperty clickcdProperty;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (clickcdProperty == null)
        {
            clickcdProperty = serializedObject.FindProperty("clickCd");
        }
        EditorGUILayout.PropertyField(clickcdProperty, new GUIContent("点击cd"));
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
        
    }
}