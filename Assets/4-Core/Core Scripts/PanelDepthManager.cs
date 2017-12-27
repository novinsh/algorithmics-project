using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Use this script for rapidly designing pop-ups
///  Caution : Don't use it on non-pop-up Game Panels
/// </summary>

public class PanelDepthManager : MonoBehaviour
{

    public int SpriteRenderer_Depth_Offset;
    public int Canvas_Depth_Offset;
    public bool inHundreds = false;

    void Start()
    {
        if (inHundreds)
        {
            SpriteRenderer_Depth_Offset *= 100;
            Canvas_Depth_Offset *= 100;
        }
        foreach (SpriteRenderer temp in transform.GetComponentsInChildren<SpriteRenderer>(true))
        {
            temp.sortingOrder += SpriteRenderer_Depth_Offset;
        }

        foreach (Canvas temp in transform.GetComponentsInChildren<Canvas>(true))
        {
            temp.sortingOrder += Canvas_Depth_Offset;
        }

    }

}
