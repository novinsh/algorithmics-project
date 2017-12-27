using System;
using System.Linq;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{


    #region Core Tweakers & Attributes

    // An enum with all the possible states of the game. NOTE: depending on the game the game states may change. Please add at the end of the screen.
    public enum GameState
    {

        //---------------  GameRelated Scene -----------------------

        MainMenuScene, 	        //MainMenuScene
        SelectionScene,			//SelectionScene
        GamePlayScene,
        ExitPanel,
        DummyScene,


    };

    [Header("Core Tweakers")]
    [Space(5)]
    public GameState initialState = GameState.MainMenuScene;
    [Space(5)]
    public GameState Stack_Peek;
    public NavigationManager navigationManager = new NavigationManager();
    public Action GameStateChanged;

    #endregion Game States


    [Space(10)]
    [Header("Temp Attribs")]
    [Space(5)]
    public int GamePlayCount = 0;
    public bool Call_Rate_Us = false;



    #region Mono Life Cycle

    void OnEnable()
    {
        this.GameStateChanged += this.OnGameStateChanged;
    }

    void OnDisable()
    {
        this.GameStateChanged -= this.OnGameStateChanged;
    }

    public void OnGameStateChanged()
    {
        this.Stack_Peek = (GameManager.GameState)MenuManager.Instance.navigationStack.Peek();
    }

    // Use this for initialization
    void Start()
    {

        if (!PlayerPrefs.HasKey("RateUs"))
        {
            PlayerPrefs.SetInt("RateUs", 0);
        }

        if (!PlayerPrefs.HasKey("RemoveAds"))
        {
            PlayerPrefs.SetInt("RemoveAds", 0);
        }
        PlayerPrefs.Save();

    }


    #endregion Mono Life Cycle


    #region Custom_Behaviours

    //-------------------

    #endregion
}
