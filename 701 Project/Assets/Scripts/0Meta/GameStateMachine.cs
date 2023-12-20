using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine instance;
    [SerializeField] GameObject tutorialManagerGO;

    #region States
    public enum StartStateEnum
    {
        MainMenu,
        Tutorial,
        Setup,
        Battle
    };
    [Header("States")]
    [SerializeField] StartStateEnum startStateEnum;

    [HideInInspector]
    public State currentState;
    State mainMenuState = new MainMenuState();
    State tutorialState = new TutorialState();
    [HideInInspector]
    public State setupState = new SetupState();     // Made these to public so NudgeManager can access it - Daniel :)
    State battleState = new BattleState();       
    #endregion

    #region GUI Groups
    [Header("GUI Groups")]
    [SerializeField] GameObject mainMenuGO; //GO = GameObject
    public GameObject tutorialScreenGO;
    public GameObject tutorialRollButtonsGroupGO;
    public GameObject tutorialOpenHandbookButtonGO;
    public GameObject tutorialHandbookOxygenButtonGO;
    public GameObject tutorialGoToFightButtonGO;
    [SerializeField] GameObject setupGO;
    [SerializeField] GameObject battleGO;
    public GameObject moreCoinsLeftPopupGO;
    #endregion

    #region Other
    [Header("Other")]
    public bool useDebug;
    public bool isFirstRound;
    public bool isFirstTimePlaying;
    public bool allowBattleCheatEnd;
    public bool showQuitButton;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    { 
        //TODO: find another solution, like putting this into the individual states instead
        if      (startStateEnum == StartStateEnum.MainMenu) { currentState = mainMenuState; mainMenuGO.SetActive(true);  setupGO.SetActive(false); battleGO.SetActive(false); }
        else if (startStateEnum == StartStateEnum.Tutorial) { currentState = tutorialState; mainMenuGO.SetActive(false); setupGO.SetActive(false); battleGO.SetActive(false); }
        else if (startStateEnum == StartStateEnum.Setup)    { currentState = setupState;    mainMenuGO.SetActive(false); setupGO.SetActive(true);  battleGO.SetActive(false); }
        else if (startStateEnum == StartStateEnum.Battle)   { currentState = battleState;   mainMenuGO.SetActive(false); setupGO.SetActive(false); battleGO.SetActive(true); }

        currentState.Begin();
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(State newState)
    {
        if(currentState != null) { currentState.End(); }

        currentState = newState;

        //TODO: find another solution, like putting this into the individual states instead
        if      (currentState == mainMenuState) { currentState = mainMenuState; mainMenuGO.SetActive(true);  setupGO.SetActive(false); battleGO.SetActive(false); }
        else if (currentState == tutorialState) { currentState = tutorialState; mainMenuGO.SetActive(false); setupGO.SetActive(false); battleGO.SetActive(false); }
        else if (currentState == setupState)    { currentState = setupState;    mainMenuGO.SetActive(false); setupGO.SetActive(true);  battleGO.SetActive(false); }
        else if (currentState == battleState)   { currentState = battleState;   mainMenuGO.SetActive(false); setupGO.SetActive(false); battleGO.SetActive(true); }

        currentState.Begin();
    }

    public void ChangeToMainMenuState() { ChangeState(mainMenuState); }
    public void ChangeToTutorialOrSetup()
    {
        if (isFirstTimePlaying) { ChangeToTutorialState(); Debug.Log("Switching to tutorial..."); }
        else { ChangeToSetupState(); Debug.Log("Switching to setup..."); }
    }

    public void ChangeToTutorialState() { ChangeState(tutorialState); }
    public void ChangeToSetupState() { ChangeState(setupState); }
    public void ChangeToBattleState() {  ChangeState(battleState); }

    public void SetIsFirstTimePlaying(bool newBool) { isFirstTimePlaying = newBool; }

    public void MoreCoinsLeftPopupBeforeBattle(){
        Debug.Log("Coins left: " + ShopControl.instance.numberOfCC);

        // Skip this if we are in the tutorial
        if (tutorialManagerGO.activeInHierarchy) { 
            Debug.Log("We are in the tutorial, skipping asking for coin spendage...");
            ChangeToBattleState(); 
            return;
        }

        if (ShopControl.instance.numberOfCC > 0){
            Debug.Log("Player has coins left!");
            
            moreCoinsLeftPopupGO.SetActive(true);
        }

        else {
            Debug.Log("Player has no coins left!");
            ChangeToBattleState(); 
        }
    }
}

public abstract class State
{
    public abstract void Begin();
    public abstract void Update();
    public abstract void End();
    public abstract void Echo(string message);
}

public class MainMenuState : State
{
    bool useDebug;
    public override void Begin()
    {
        useDebug = GameStateMachine.instance.useDebug;
        GameStateMachine.instance.isFirstRound = true;

        if (useDebug) Echo("Begin()");

        #region Playing/Stopping SOUND
        AudioManager.instance.Stop("SetupMusic");
        AudioManager.instance.Stop("BattleMusic");
        AudioManager.instance.Stop("PlayerWonGameMusic");
        AudioManager.instance.Stop("PlayerLostGameMusic");

        AudioManager.instance.Play("MainMenuMusic");
        #endregion
    }

    public override void End()
    {
        if (useDebug) Echo("End()");
    }

    public override void Update()
    {
        if (useDebug) Echo("Update()");
    }

    public override void Echo(string message)
    {
        Debug.Log("Main Menu State: " + message);
        
    }
}

public class TutorialState : State
{
    bool useDebug;
    public override void Begin()
    {
        useDebug = GameStateMachine.instance.useDebug;
        if (useDebug) Echo("Begin()");
        //TutorialManager.instance.OnlyActivateFirstPopUp();
    }

    public override void Echo(string message)
    {
        Debug.Log("Tutorial State: " + message);
    }

    public override void End()
    {
        GameStateMachine.instance.isFirstTimePlaying = false;
    }

    public override void Update()
    {
        
    }
}

public class SetupState : State
{
    #region Declaring BOOLEANS
    bool isFirstRound; //used to determine whether or not this is the first round
    bool isFirstTimePlaying;
    bool useDebug; //used for controlling whether or not we want to have debugs from this script appear in the console
    #endregion

    public override void Begin()
    {
        #region Assigning BOOLEANS
        useDebug = GameStateMachine.instance.useDebug; //whether or not we want to have debugs from this script appear in the console
        isFirstRound = GameStateMachine.instance.isFirstRound; //whether or not this is the first round
        isFirstTimePlaying = GameStateMachine.instance.isFirstTimePlaying;
        #endregion

        if (isFirstTimePlaying == true)
        {
            Debug.Log("First time playing. Should show tutorial...");
            GameStateMachine.instance.tutorialScreenGO.SetActive(true);
            GameStateMachine.instance.tutorialRollButtonsGroupGO.SetActive(true);
            GameStateMachine.instance.tutorialOpenHandbookButtonGO.SetActive(true);
            GameStateMachine.instance.tutorialHandbookOxygenButtonGO.SetActive(true);
            GameStateMachine.instance.tutorialGoToFightButtonGO.SetActive(true);


            #region Resetting TEAM and SHOP
            if (TeamControl.instance.CheckHasTeam())
            {
                TeamControl.instance.ResetTeam();
            }

            ShopControl.instance.numberOfCC = 20;
            ShopControl.instance.ResetShop();
            #endregion

            #region Setting new PLAYER INFORMATION
            PlayerInformation.instance.ResetStats(); //reset stats (hearts, rounds, wins, team, and shop)
            PlayerInformation.instance.teamName = PlayerInformation.instance.GenerateTeamName(); //creating a new team name
            #endregion

            UnitCreator.instance.UpdateActiveElements(1);

            isFirstTimePlaying = false;
            isFirstRound = false;
            GameStateMachine.instance.isFirstTimePlaying = false;
            GameStateMachine.instance.isFirstRound = false;
        }
        else
        {
            if (isFirstRound == true) //this is first round of the playthrough
            {
                Debug.Log("This is the first round of the playthrough");

                #region Resetting BOOLEANS
                isFirstRound = false;
                GameStateMachine.instance.isFirstRound = false;
                #endregion

                #region Resetting TEAM and SHOP
                if (TeamControl.instance.CheckHasTeam())
                {
                    TeamControl.instance.ResetTeam();
                }
                ShopControl.instance.ResetShop();
                #endregion

                #region Setting new PLAYER INFORMATION
                PlayerInformation.instance.ResetStats(); //reset stats (hearts, rounds, wins, team, and shop)
                PlayerInformation.instance.teamName = PlayerInformation.instance.GenerateTeamName(); //creating a new team name
                #endregion

                UnitCreator.instance.UpdateActiveElements(1);
            }
            else  //this is NOT the first round of the playthrough
            {
                PlayerInformation.instance.IncrementCurrentRounds(1); //so we increment the current number of rounds that we have played
                NudgeManager.instance.HandleHaventOpenedHandbookStreak();  // incrementing the handbook not opened streak
            }

            #region Setting SHOP properties
            ShopControl.instance.Roll(false); //we roll for new units in the shop, with 'requiresPayment' set to FALSE
            ShopControl.instance.ResetCoins(); //we reset the number of coins back to the number of coins that the player should start with
            #endregion
        }

        #region Setting PLAYER STATS GUI
        SetupControl.instance.SetPlayerStatsGUI();
        #endregion

        //TODO: move this somewhere else than PlayerInformation.cs!
        //PlayerInformation.instance.UpdatePlayStatsGUI(); //we update the gui for play stats (name, hearts, rounds, and wins)

        #region Playing/Stopping SOUND
        //AudioManager.instance.Stop("MainMenuMusic");
        AudioManager.instance.Stop("BattleMusic");

        AudioManager.instance.Play("MainMenuMusic");
        AudioManager.instance.Play("SetupMusic");
        #endregion

        if (useDebug) Echo("Begin()");
    }

    public override void End()
    {
        PlayerInformation.instance.SaveCurrentPlay();

        EnemyFinder.instance.FindEnemy(false);

        if (useDebug) Echo("End()");
    }

    public override void Update()
    {
        if (useDebug) Echo("Update()");
    }

    public override void Echo(string message)
    {
        Debug.Log("Setup State: " + message);
    }
}

public class BattleState : State
{
    bool useDebug;

    public override void Begin()
    {
        useDebug = GameStateMachine.instance.useDebug;

        if (useDebug) Echo("Begin()");

        BattleControl.instance.SetTeamsGUI();

        #region Sounds
        AudioManager.instance.Stop("SetupMusic");
        AudioManager.instance.Stop("MainMenuMusic");

        AudioManager.instance.Play("BattleMusic");
        #endregion
    }

    public override void End()
    {
        if (useDebug) Echo("End()");
        BattleControl.instance.ClearBattleGUI();
    }

    public override void Update()
    {
        if (useDebug) Echo("Update()");
    }

    public override void Echo(string message)
    {
        Debug.Log("Battle State: " + message);
    }
}