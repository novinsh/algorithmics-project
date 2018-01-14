using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour {

    public GameObject[] panels;
    public Population_Controller popController;


    private void Start()
    {
        panels[0].SetActive(true);
    }


    public void OnOk(InputField text)
    {

        panels[0].SetActive(false);
        panels[1].SetActive(true);
        

        int value = int.Parse(text.text);
        if (value > 0)
        {
            popController.numOfGenerations = value;
        }
    }

    public void OnStart()
    {
        panels[2].SetActive(true);
        if (!popController.gameObject.active)
        {
            popController.gameObject.SetActive(true);
        }
    }

    public void OnPauseUnPause(Text toggle)
    {
        toggle.text = (toggle.text=="Pause") ? "UnPause": "Pause";
        popController.Pause = !popController.Pause;
    }


    public void OnReset()
    {
        Application.LoadLevel(0);
    }
}
