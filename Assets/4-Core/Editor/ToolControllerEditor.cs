using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolController))]
public class ToolControllerEditor : Editor
{
    ToolController m_target;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        m_target = (ToolController)target;


        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ToolController)target), typeof(ToolController), false);
        GUI.enabled = true;
        GUILayout.Space(5);
        m_target.Delayed_Position = EditorGUILayout.Toggle("Delay Joint Postion", m_target.Delayed_Position);

        GUILayout.Space(5);
        if (m_target.Delayed_Position)
        {
            GUILayout.Space(5);
            m_target.Delay_Timer = EditorGUILayout.FloatField("Delay Timer", m_target.Delay_Timer);
        }
    }

}
