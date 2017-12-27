using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{


    public enum Mode { Prefabs, Objects };

    public BaseMenu[] menus;

    public Mode mode = Mode.Prefabs;

    // TODO: change the hashtable to dictionary
    private Hashtable menuTable = null;

    public Stack navigationStack = null;

    private Hashtable createdMenus = null;



    #region MonoBehaviour: LifeCycle

    /* 
     * <summary>
     * 
     * </summary>
     */


    void Awake()
    {
        // 1. Initialize and populate the menu hash table
        PopulateMenuHashTable();

        // 2. Initialize the navigation stack
        InitializeNavigationStack();

        // 3. Hide all open menus (this is needed in case the developer left some menu open)
        HideAllMenus();

        // 4. Display the initial menu or display the menu at the top of the stack (returning from some other unity scene)
        if (navigationStack.Count == 0)
        {
            PushMenu(GameManager.Instance.initialState);
        }
        else
        {
            ShowMenu(GetMenuForState(NavigationStackPeek()));
        }
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    //void Update()
    //{

    //}

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.navigationManager.navigationStacks.ContainsKey(Application.loadedLevelName))
            {
                GameManager.Instance.navigationManager.navigationStacks.Remove(Application.loadedLevelName);
            }
            GameManager.Instance.navigationManager.navigationStacks.Add(Application.loadedLevelName, navigationStack);
        }
    }

    #endregion MonoBehaviour: LifeCycle


    #region GameManager: GameStateChange Delegate

    /* 
	 * <summary>
	 * 
	 * </summary>
	 */
    void HandleGameStateChanged(GameManager.GameState g)
    {
        // If we want to do anything on game change
    }

    #endregion GameManager: GameStateChange Delegate


    #region Menu Navigation Control Logic

    /* 
	 * <summary>
	 * 
	 * </summary>
	 */
    public void PushMenu(GameManager.GameState g)
    {

        // 1. If the incoming menu is a pop-up dont hide the last menu
        if (GetMenuForState(g).isPopup == false)
        {
            // 1.1. Hide the menu at the top of the stack
            if (navigationStack.Count != 0)
            {
                HideMenuAtState(NavigationStackPeek());
            }
        }

        // 2. Push the next menu
        navigationStack.Push(g);

        // 3. Inform the game manager about the game state change
        InformGameManager(g);

        GameManager.Instance.Stack_Peek = g;

        // 4. Show the new menu at the top of the stack
        ShowMenuAtState(g);
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    public void PopMenu()
    {
        // 1. Hide the menu at the top of the stack
        if (navigationStack.Count != 0)
        {
            HideMenuAtState(NavigationStackPeek());
        }

        // 2. Pop the menu from the top of the stack
        navigationStack.Pop();

        // 3. Get the menu at the top of the stack
        GameManager.GameState g = NavigationStackPeek();

        // 4. Inform the game manager about the game state
        InformGameManager(g);

        GameManager.Instance.Stack_Peek = g;

        // 5. Show the menu at the top of the stack 
        ShowMenuAtState(g);
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    public void PopMenuToState(GameManager.GameState g)
    {
        // 1. Hide the menu at the top of the stack
        if (navigationStack.Count != 0)
        {
            HideMenuAtState(NavigationStackPeek());
        }

        // 2. Keep popping till the desired menu is reached
        while (NavigationStackPeek() != g)
        {
            navigationStack.Pop();
            // TODO: check if this works
            HideMenuAtState(NavigationStackPeek());
        }

        // 3. Inform the game manager about the game state
        InformGameManager(g);

        GameManager.Instance.Stack_Peek = g;

        // 4. Show the menu at the top of the stack 
        ShowMenuAtState(g);
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    private void InformGameManager(GameManager.GameState g)
    {
        //GameManager.Instance.ChangeGameStateTo(g);
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    private void ShowMenuAtState(GameManager.GameState g)
    {

        ShowMenu(GetMenuForState(g));

    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    private void HideMenuAtState(GameManager.GameState g)
    {
        switch (g)
        {
            default:
                HideMenu(GetMenuForState(g));
                break;

        }
    }

    /* 
     * <summary>
     * This method disables all the menues assossiated with menu manager.
     * </summary>
     */
    private void HideAllMenus()
    {
        foreach (DictionaryEntry de in menuTable)
        {
            BaseMenu bm = de.Value as BaseMenu;
            //bm.gameObject.SetActive(false);

            HideMenu(bm);
        }
    }


    /* 
     * <summary>
     * 
     * </summary>
     */
    private void ShowMenu(BaseMenu bm)
    {
        if (mode == Mode.Prefabs)
        {
            BaseMenu tempBaseMenu = createdMenus[bm.state] as BaseMenu;
            if (tempBaseMenu != null)
            {
                tempBaseMenu.MenuWillAppear();
                tempBaseMenu.gameObject.SetActive(true);
                tempBaseMenu.MenuDidAppear();
                return;
            }


            BaseMenu newBM = BaseMenu.Instantiate(bm) as BaseMenu;

            GameObject root = GameObject.FindGameObjectWithTag("Root");

            newBM.transform.parent = root.transform;

            newBM.transform.localScale = Vector3.one;

            newBM.MenuWillAppear();
            newBM.gameObject.SetActive(true);
            newBM.MenuDidAppear();


            createdMenus.Add(bm.state, newBM);
        }
        else if (mode == Mode.Objects)
        {
            bm.MenuWillAppear();
            bm.gameObject.SetActive(true);
            bm.MenuDidAppear();
        }

    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    private void HideMenu(BaseMenu bm)
    {
        if (mode == Mode.Prefabs)
        {

            BaseMenu previousBM = createdMenus[bm.state] as BaseMenu;

            if (previousBM != null)
            {
                previousBM.MenuWillDisappear();
                previousBM.gameObject.SetActive(false);
                previousBM.MenuDidDisappear();

                createdMenus.Remove(bm.state);

                Destroy(previousBM.gameObject);
            }
        }
        else if (mode == Mode.Objects)
        {
            bm.MenuWillDisappear();
            bm.gameObject.SetActive(false);
            bm.MenuDidDisappear();
        }

        //Destroy(bm);


    }

    #endregion Menu Navigation Control Logic

    #region Initialization

    /* 
	 * <summary>
	 * 
	 * </summary>
	 */
    private void PopulateMenuHashTable()
    {
        menuTable = new Hashtable();

        createdMenus = new Hashtable();

        for (int i = 0; i < menus.Length; i++)
        {

            BaseMenu bm = menus[i];
            //	Debug.Log("Creating Menu " + bm.state);
            menuTable.Add(bm.state, bm);

            //bm.gameObject.SetActive(true);
            bm.gameObject.SetActive(false);
        }

    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    private void InitializeNavigationStack()
    {
        if (!(GameManager.Instance.navigationManager.navigationStacks.ContainsKey(Application.loadedLevelName)))
        {
            navigationStack = new Stack();
        }
        else
        {
            navigationStack = GameManager.Instance.navigationManager.navigationStacks[Application.loadedLevelName];

            GameManager.Instance.navigationManager.navigationStacks.Remove(Application.loadedLevelName);
        }

    }


    #endregion Initialization


    #region Utility Methods

    /* 
	 * <summary>
	 * 
	 * </summary>
	 */
    private BaseMenu GetMenuForState(GameManager.GameState g)
    {
        return menuTable[g] as BaseMenu;
    }

    /* 
     * <summary>
     * 
     * </summary>
     */
    public GameManager.GameState NavigationStackPeek()
    {
        return (GameManager.GameState)navigationStack.Peek();
    }

    public void PurgeNavigationStack()
    {
        if (GameManager.Instance.navigationManager.navigationStacks.ContainsKey(Application.loadedLevelName) == false)
        {
            navigationStack.Clear();
        }
        //else
        //{
        //    navigationStack.Clear();

        //    GameManager.Instance.navigationManager.navigationStacks.Remove(Application.loadedLevelName);
        //}
    }
    #endregion Utility Methods


}
