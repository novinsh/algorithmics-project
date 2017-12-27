using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomSprite))]
public class CustomSpriteEditor : Editor
{
    CustomSprite m_target;

    public override void OnInspectorGUI()
    {
        m_target = (CustomSprite)target;
        GUILayout.Space(10);


        EditorGUILayout.BeginHorizontal();

        m_target.customPixelVector = EditorGUILayout.Vector2Field("", m_target.customPixelVector);

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Custom Pixel", GUILayout.Width(90), GUILayout.Height(25)))
        {
            m_target.CustomPixel();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Make Pixel Perfect"))
        {
            m_target.PixelPerfect();
        }

    }

}
