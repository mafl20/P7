using System;
using System.Collections;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public static BattleStateMachine instance;

    #region States
    public enum BattleStateEnum
    {
        StartBattle,
        SettingTeams,
        Move,
        Attack,
        EvaluateFight,
        EvaluateBattle
    }
    
    [Header("States")]
    [SerializeField] BattleStateEnum battleStateEnum;

    public State currentBattleState;
    public State startBattleState = new StartBattleState();
    State settingTeamsState = new SettingTeamsState();
    State moveState = new MoveState();
    State attackState = new AttackState();
    State evaluateFightState = new EvaluateFightState();
    State evaluateBattleState = new EvaluateBattleState();
    public byte currentStateIndex;
    #endregion

    #region Other
    [Header("Other")]
    public bool useDebug;
    public bool isPaused;
    public float timeBetweenStates;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentStateIndex = 0;
    }

    public void NextStep()
    {
        StartCoroutine(WaitBeforeNextStep(timeBetweenStates));
    }

    IEnumerator WaitBeforeNextStep(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        NextState();
    }


    public void NextState()
    {
        currentStateIndex++; if(currentStateIndex > 5) { currentStateIndex = 2; }

        string currentStateString = "";

        if      (currentStateIndex == 0) { currentBattleState = startBattleState;    currentStateString = "Start Battle State";    battleStateEnum = BattleStateEnum.StartBattle; }
        else if (currentStateIndex == 1) { currentBattleState = settingTeamsState;   currentStateString = "Setting Teams State";   battleStateEnum = BattleStateEnum.SettingTeams; }
        else if (currentStateIndex == 2) { currentBattleState = moveState;           currentStateString = "Move State";            battleStateEnum = BattleStateEnum.Move; }
        else if (currentStateIndex == 3) { currentBattleState = attackState;         currentStateString = "Attack State";          battleStateEnum = BattleStateEnum.Attack; }
        else if (currentStateIndex == 4) { currentBattleState = evaluateFightState;  currentStateString = "Evaluate Fight State";  battleStateEnum = BattleStateEnum.EvaluateFight; }
        else if (currentStateIndex == 5) { currentBattleState = evaluateBattleState; currentStateString = "Evaluate Battle State"; battleStateEnum = BattleStateEnum.EvaluateBattle; }

        Debug.Log("Current Battle State: " + currentStateString);

        currentBattleState.Begin();
    }

    public void ResetBattleStateMachine()
    {
        currentStateIndex = 0;
        currentBattleState = settingTeamsState;
    }

    private void Update()
    {
        if (currentBattleState != null)
        {
            currentBattleState.Update();
        }
    }
}


public class StartBattleState : State
{
    public override void Begin()
    {
        //TODO: transition (zoom in and wipe or w/e it's called)
        //      > when transition done, call End()

        BattleStateMachine.instance.NextStep();
    }

    public override void Echo(string message)
    {
    }

    public override void End()
    {
        //TODO: wait before trigger switch to SettingTeamsState
    }

    public override void Update()
    {
    }
}

public class SettingTeamsState : State
{
    public override void Begin()
    {
        BattleMethods.instance.SetTeams();
        BattleMethods.instance.CheckEmptyTeam();
        BattleStateMachine.instance.NextStep();
    }

    public override void Echo(string message)
    {
    }

    public override void End()
    {
        //TODO: wait before trigger switch to MoveState
    }

    public override void Update()
    {
        //TODO: animation of units sliding into the slots
        //      > when done, call End()
    }
}

public class MoveState : State
{
    bool useDebug, isPaused;

    public override void Begin()
    {
        useDebug = BattleStateMachine.instance.useDebug;
        isPaused = BattleStateMachine.instance.isPaused;

        if (useDebug) Echo("Begin()");

        BattleMethods.instance.Move();

        BattleStateMachine.instance.NextStep();
    }

    public override void Update()
    {
        //TODO: animation of units moving from original slot to furthest ahead empty slot
        //      > when animation done, call End()
    }

    public override void End()
    {
        //TODO: wait before trigger switch to AttackState
    }

    public override void Echo(string message)
    {
        
    }
}

public class AttackState : State
{
    bool useDebug, isPaused;

    public override void Begin()
    {
        useDebug = BattleStateMachine.instance.useDebug;
        isPaused = BattleStateMachine.instance.isPaused;

        if (useDebug) Echo("Begin()");

        BattleMethods.instance.AttackAnimation();
        //BattleMethods.instance.Attack(); //moved to Attack()-method in UnitProperties

        //TODO: animation of units dashing against one another
        //      > when animation done, call End()

        BattleStateMachine.instance.NextStep();
    }

    public override void Update()
    {
    }

    public override void End()
    {
        //TODO: wait before trigger switch to EvaluateFightState
    }

    public override void Echo(string message)
    {
    }
}

public class EvaluateFightState : State
{
    bool useDebug, isPaused;

    public override void Begin()
    {
        useDebug = BattleStateMachine.instance.useDebug;
        isPaused = BattleStateMachine.instance.isPaused;

        if (useDebug) Echo("Begin()");

        BattleMethods.instance.EvaluateFight();

        //TODO: animation of dead units flying out
        //      > when animation done, call End()

        BattleStateMachine.instance.NextStep();
    }

    public override void Update()
    {
    }

    public override void End()
    {
        //TODO: wait before trigger switch to EvaluateBattleState
    }

    public override void Echo(string message)
    {
    }
}

public class EvaluateBattleState : State
{
    bool useDebug, isPaused;

    public override void Begin()
    {
        useDebug = BattleStateMachine.instance.useDebug;
        isPaused = BattleStateMachine.instance.isPaused;

        if (useDebug) Echo("Begin()");

        byte result = BattleMethods.instance.EvaluateBattle();
        BattleMethods.instance.DetermineOutcome(result);
        if (result != 0)
        {
            BattleStateMachine.instance.currentStateIndex = 0;
            BattleStateMachine.instance.currentBattleState = BattleStateMachine.instance.startBattleState;
            return;
        }
        BattleStateMachine.instance.NextStep();
    }

    public override void Update()
    {
    }

    public override void End()
    {
    }

    public override void Echo(string message)
    {
    }
}