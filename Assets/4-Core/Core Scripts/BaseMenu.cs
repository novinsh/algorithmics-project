using UnityEngine;
using UnityEngine.Events;


interface IBackButtonPress
{
    void OnBackBtnPress();
}

public class BaseMenu : MonoBehaviour, IBackButtonPress
{

    public GameManager.GameState state;
    public bool isPopup = false;



    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackBtnPress();                   // If no override than Base Behaviour is Called
        }
    }


    #region Button Handlers

    public virtual void OnBackBtnPress()
    {
        //Debug.Log("Base Back Button Press");
        if (MenuManager.Instance.navigationStack.Count != 1)
        {
            MenuManager.Instance.PopMenu();
        }
        else
        {
            if (PlayerPrefs.GetInt("RemoveAds") != 1)
            {
                //Show Ad on Exit
            }
            MenuManager.Instance.PushMenu(GameManager.GameState.ExitPanel);
        }
    }




    #endregion Button Handlers


    #region Mono Life Cycle

    #endregion

    #region BaseMenu: Life Cycle

    /* 
     * <summary>
     * This method is called right before your view appears, good for hiding/showing fields or any operations 
     * that you want to happen every time before the view is visible. Because you might be going back and 
     * forth between views, this will be called every time your view is about to appear on the screen.
     * </summary>
     */
    public void MenuWillAppear()
    {

    }

    /* 
     * <summary>
     * This method is called after the view appears - great place to start an animations or the loading of 
     * external data from an API.
     * </summary>
     */
    public void MenuDidAppear()
    {

    }

    /* 
     * <summary>
     * This method is called right before the view disappears - its a good place to stop all animations, API calls
     * etc
     * </summary>
     */
    public void MenuWillDisappear()
    {

    }

    /* 
     * <summary>
     * This method is called after the view disappears
     * </summary>
     */
    public void MenuDidDisappear()
    {

    }

    #endregion BaseMenu: Life Cycle
}


public class CustomEvents : UnityEvent
{

}