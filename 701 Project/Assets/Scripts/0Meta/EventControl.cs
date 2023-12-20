using UnityEngine;
using UnityEngine.Events;

public class EventControl : MonoBehaviour
{
    public static EventControl instance;

    [SerializeField] GameObject tutorialManagerGO;

    public UnityEvent selectEvent;
    public UnityEvent deselectEvent;

    public UnityEvent rollEvent;
    public UnityEvent buyEvent;
    public UnityEvent sellEvent;
    public UnityEvent freezeEvent;
    public UnityEvent unfreezeEvent;
    public UnityEvent combineEvent;
    public UnityEvent moveWithinTeamEvent;

    #region Bools
    bool isFirstTimePLaying;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isFirstTimePLaying = GameStateMachine.instance.isFirstTimePlaying;

        selectEvent.AddListener(SelectHappened);
        deselectEvent.AddListener(DeselectHappened);

        rollEvent.AddListener(RollHappened);
        buyEvent.AddListener(BuyHappened);
        sellEvent.AddListener(SellHappened);
        freezeEvent.AddListener(FreezeHappened);
        unfreezeEvent.AddListener(UnfreezeHappened);

        combineEvent.AddListener(CombineHappened);
        moveWithinTeamEvent.AddListener(MoveWithinTeamHappened);
    }

    void SelectHappened()
    {
        //Debug.Log("Select happened...");
        if(isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            Debug.Log("Will do tutorial stuff...");
            TutorialManager.instance.hasSelected = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }
    }

    void DeselectHappened()
    {
        //Debug.Log("Deselect happened...");
    }

    void RollHappened()
    {
        //Debug.Log("Roll happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasRolled = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }

    void BuyHappened()
    {
        //Debug.Log("Buy happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasBought = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }

    void SellHappened()
    {
        //Debug.Log("Sell happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasSold = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }
    
    void FreezeHappened()
    {
        //Debug.Log("Freeze happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasFrozen = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }
    
    void UnfreezeHappened()
    {
        //Debug.Log("Unfreeze happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasUnfrozen = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }

    void CombineHappened()
    {
        //Debug.Log("Combine happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasCombined = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }
    
    void MoveWithinTeamHappened()
    {
        //Debug.Log("Move within team happened...");
        if (isFirstTimePLaying && tutorialManagerGO.activeInHierarchy)
        {
            TutorialManager.instance.hasMovedWithinTeam = true; TutorialManager.instance.CheckIfConditionForNextIsMet();
        }

        NudgeManager.instance.ResetNudge();
    }

}