using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UIDragObject))]
public class UIDragObjectEditor : Editor
{

    UIDragObject m_target;

    public override void OnInspectorGUI()
    {
        m_target = (UIDragObject)target;
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((UIDragObject)target), typeof(UIDragObject), false);
        GUI.enabled = true;

        DrawOffsets();
        DrawClamps();


    }



    private void DrawOffsets()
    {
        GUILayout.Space(15);

        EditorGUILayout.LabelField("OffSetting", EditorStyles.boldLabel);
        GUILayout.Space(5);
        m_target.UseOffset = EditorGUILayout.Toggle("Use OffSet", m_target.UseOffset);
        if (m_target.UseOffset)
        {
            GUILayout.Space(5);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginHorizontal();
            m_target.offset = EditorGUILayout.Vector2Field("Offset Factor", m_target.offset);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
        }
        GUILayout.Space(5);
    }

    private void DrawClamps()
    {
        EditorGUILayout.LabelField("Clamping", EditorStyles.boldLabel);
        GUILayout.Space(5);
        m_target.isClamped = EditorGUILayout.Toggle("Use Clamped", m_target.isClamped);
        if (m_target.isClamped)
        {
            GUILayout.Space(5);
            m_target.clampType = (UIDragObject.ClampType)EditorGUILayout.EnumPopup("ClampType", m_target.clampType);
            switch (m_target.clampType)
            {
                case UIDragObject.ClampType.X:
                    m_target.Min_X = EditorGUILayout.FloatField("Minimum X", m_target.Min_X);
                    m_target.Max_X = EditorGUILayout.FloatField("Maximum X", m_target.Max_X);
                    break;
                case UIDragObject.ClampType.Y:
                    m_target.Min_Y = EditorGUILayout.FloatField("Minimum Y", m_target.Min_Y);
                    m_target.Max_Y = EditorGUILayout.FloatField("Maximum Y", m_target.Max_Y);
                    break;
                case UIDragObject.ClampType.XY:
                    m_target.Min_X = EditorGUILayout.FloatField("Minimum X", m_target.Min_X);
                    m_target.Max_X = EditorGUILayout.FloatField("Maximum X", m_target.Max_X);
                    m_target.Min_Y = EditorGUILayout.FloatField("Minimum Y", m_target.Min_Y);
                    m_target.Max_Y = EditorGUILayout.FloatField("Maximum Y", m_target.Max_Y);
                    break;
            }
        }
        GUILayout.Space(8);
    }
}
