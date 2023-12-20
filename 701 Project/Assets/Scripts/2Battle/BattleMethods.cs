using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMethods : MonoBehaviour
{
    public static BattleMethods instance;

    List<Slot> playerSlots;
    List<Slot> enemySlots;

    public bool playerTeamDied;
    public bool enemyTeamDied;

    public bool teamsAreSet;

    private void Awake()
    {
        instance = this;
    }

    public void StartDelayedMethod(float delay)
    {
        StartCoroutine(WaitBeforeContinue(delay));
    }

    IEnumerator WaitBeforeContinue(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public void Step()
    {
        if (teamsAreSet == false) { SetTeams(); } //if the teams are not setup, we set them up

        CheckEmptyTeam();

        byte result = Cycle();
        DetermineOutcome(result);
    }

    public byte Cycle()
    {
        Move();
        Attack();
        EvaluateFight();
        return EvaluateBattle();
    }

    public void Move()
    {
        #region Player Team
        for (int i = 1; i < playerSlots.Count; i++) //starting at index 1
        {
            //Debug.Log("i: " + i);
            if (playerSlots[i].unitGO == null) //if the slot has no unit
            {
                //Debug.Log("Slot " + i + " is empty. Continuing...");
                continue; //we skip it
            }
            else
            {
                //Debug.Log("Slot " + i + " is filled. Looking for furthest index...");
            }

            int furthestAheadEmptyIndex = 99;

            for (int j = i-1; j > -1; j--) //starting at the index before i
            {
                //Debug.Log("j: " + j);
                if(playerSlots[j].unitGO == null) //if the slot has no unit
                {
                    //Debug.Log("Slot " + j + " is empty. Setting furthest index...");
                    furthestAheadEmptyIndex = j;
                }
            }

            if (furthestAheadEmptyIndex < playerSlots.Count)
            {
                //Debug.Log("Inserting unit from slot " + i + " into slot " + furthestAheadEmptyIndex + "...");

                var furthestSlot = playerSlots[furthestAheadEmptyIndex];
                playerSlots[furthestAheadEmptyIndex].unitGO = playerSlots[i].unitGO;
                playerSlots[furthestAheadEmptyIndex].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(furthestSlot.unitSpot.position, furthestSlot.unitSpot);
                playerSlots[i].unitGO = null;
            }           
        }
        #endregion

        #region Enemy Team
        for (int i = 1; i < enemySlots.Count; i++) //starting at index 1
        {
            //Debug.Log("i: " + i);
            if (enemySlots[i].unitGO == null) //if the slot has no unit
            {
                //Debug.Log("Slot " + i + " is empty. Continuing...");
                continue; //we skip it
            }
            else
            {
                //Debug.Log("Slot " + i + " is filled. Looking for furthest index...");
            }

            int furthestAheadEmptyIndex = 99;

            for (int j = i - 1; j > -1; j--) //starting at the index before i
            {
                //Debug.Log("j: " + j);
                if (enemySlots[j].unitGO == null) //if the slot has no unit
                {
                    //Debug.Log("Slot " + j + " is empty. Setting furthest index...");
                    furthestAheadEmptyIndex = j;
                }
            }

            if (furthestAheadEmptyIndex < enemySlots.Count)
            {
                //Debug.Log("Inserting unit from slot " + i + " into slot " + furthestAheadEmptyIndex + "...");

                var furthestSlot = enemySlots[furthestAheadEmptyIndex];
                enemySlots[furthestAheadEmptyIndex].unitGO = enemySlots[i].unitGO;
                enemySlots[furthestAheadEmptyIndex].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(furthestSlot.unitSpot.position, furthestSlot.unitSpot);
                enemySlots[i].unitGO = null;
            }
        }
        #endregion
    }

    public void AttackAnimation()
    {
        if (playerSlots[0].unitGO != null) { playerSlots[0].unitGO.GetComponent<Animator>().SetTrigger("attacked_right"); }
        if (enemySlots[0].unitGO != null) { enemySlots[0].unitGO.GetComponent<Animator>().SetTrigger("attacked_left"); }
    }

    public void Attack()
    {
        if (playerSlots[0].unitGO != null)
        {
            playerSlots[0].unitGO.GetComponent<UnitProperties>().health -=
            enemySlots[0].unitGO.GetComponent<UnitProperties>().damage;

            playerSlots[0].unitGO.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();
        }

        if(enemySlots[0].unitGO != null)
        {
            enemySlots[0].unitGO.GetComponent<UnitProperties>().health -=
            playerSlots[0].unitGO.GetComponent<UnitProperties>().damage;
            enemySlots[0].unitGO.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();
        }

        AudioManager.instance.Play("BattleElementClash");
    }

    public void EvaluateFight()
    {
        if(playerSlots[0].unitGO != null)
        {
            if (playerSlots[0].unitGO.GetComponent<UnitProperties>().health <= 0)
            {
                playerSlots[0].DeleteUnitGO();
            }
        }

        if(enemySlots[0].unitGO != null)
        {
            if (enemySlots[0].unitGO.GetComponent<UnitProperties>().health <= 0)
            {
                enemySlots[0].DeleteUnitGO();
            }
        }

        #region Old attempt
        //for (int i = 0; i < playerSlots.Count; i++)
        //{
        //    Debug.Log("Looking at player team's " + i + ". member...");
        //    if (playerSlots[i].unitGO == null) { continue; }

        //    if (playerSlots[i].unitGO.GetComponent<UnitProperties>().health <= 0)
        //    {
        //        Debug.Log("Player team's " + playerSlots[i].unitGO.GetComponent<UnitProperties>().chemicalName + " in slot " + i + " has died!");
        //        playerSlots[i].DeleteUnitGO();
        //    }
        //}

        //for (int i = 0; i < enemySlots.Count; i++)
        //{
        //    Debug.Log("Looking at enemy team's " + i + ". member...");
        //    if (enemySlots[i].unitGO == null) { continue; }

        //    if (enemySlots[i].unitGO.GetComponent<UnitProperties>().health <= 0)
        //    {
        //        Debug.Log("Enemy team's " + enemySlots[i].unitGO.GetComponent<UnitProperties>().chemicalName + " in slot " + i + " has died!");
        //        enemySlots[i].DeleteUnitGO();
        //    }
        //}
        #endregion
    }

    public byte EvaluateBattle()
    {
        byte died = 0;

        #region Checking if PLAYER team died
        byte deadUnits = 0;
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if(playerSlots[i].unitGO == null) { deadUnits++; }
        }
        if(playerSlots.Count - deadUnits <= 0)
        {
            Debug.Log("Team A has no units left!");
            died += 1;
        }
        #endregion

        #region Checking if ENEMY team died
        deadUnits = 0;
        for (int i = 0; i < enemySlots.Count; i++)
        {
            if (enemySlots[i].unitGO == null) { deadUnits++; }
        }
        if (enemySlots.Count - deadUnits <= 0)
        {
            Debug.Log("Team B has no units left!");
            died += 2;
        }
        #endregion

        return died;
    }

    public void DetermineOutcome(byte result)
    {
        switch (result)
        {
            case 0: //teams are still alive
                //Debug.Log("Both teams are still alive");
                break;
            case 1: //team A is dead
                //Debug.Log("Team A are is dead");
                Lose();
                break;
            case 2: //team B is dead
                //Debug.Log("Team B is dead");
                Win();
                break;
            case 3: //both teams are dead
                //Debug.Log("Both teams are has died");
                Tie();
                break;
            default: //default
                break;
        }
    }

    void Win()
    {
        teamsAreSet = false;
        BattleControl.instance.ButtonWinRound();
    }

    void Lose()
    {
        teamsAreSet = false;
        BattleControl.instance.ButtonLoseRound();
    }
    
    void Tie()
    {
        BattleControl.instance.ButtonTieRound();
        teamsAreSet = false;
    }

    public void SetTeams()
    {
        #region Player Team
        playerSlots = BattleControl.instance.playerSlots;
        //List<Slot> playerSlots = BattleControl.instance.playerSlots;
        //foreach (Slot slot in playerSlots)
        //{
        //    if (slot.unitGO != null)
        //    {
        //        teamA.Add(slot.unitGO.GetComponent<UnitProperties>());
        //    }
        //}
        #endregion

        #region Enemy Team
        enemySlots = BattleControl.instance.enemySlots;
        //List<Slot> enemySlots = BattleControl.instance.enemySlots;
        //foreach (Slot slot in enemySlots)
        //{
        //    if (slot.unitGO != null)
        //    {
        //        teamB.Add(slot.unitGO.GetComponent<UnitProperties>());
        //    }
        //}
        #endregion

        //Debug.Log("Teams are set!");
        teamsAreSet = true;
    }

    public void CheckEmptyTeam()
    {
        if (EvaluateBattle() != 0) { DetermineOutcome(EvaluateBattle()); return; }
    }
}