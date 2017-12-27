using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationManager
{

    public Dictionary<string, Stack> navigationStacks = new Dictionary<string, Stack>();

    public void SwitchSceneAndState(string sceneName, GameManager.GameState g)
    {
        Application.LoadLevel(sceneName);
        //GameManager.Instance.ChangeGameStateTo(g);
    }
}
