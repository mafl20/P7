using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Takes in 2 Teams. In steps, compare and perform actions in order. Either per press, or recursive
/// </summary>
public class BattleSteps : MonoBehaviour
{
    
    [SerializeField]
    private Play player;
    [SerializeField]
    private Play enemy;

    public Play Player { get => player; set => player = value; }
    public Play Enemy { get => enemy; set => enemy = value; }

    //private List<Element> tempP;
    //private List<Element> tempE;

    public struct BattleElement {
        public int health, dmg; 
        public BattleElement(int h, int d) { 
            health = h; dmg = d; 
        }
    };

    private List<BattleElement> playerTeam;
    private List<BattleElement> enemyTeam;


    void Start()
    {
        //tempP = new List<Element>(new Element[5]);
        //for (int i = 0; i < 4; i++)
        //{
        //    tempP[i] = new Element(UnityEngine.Random.Range(1, 20));
        //}
        //tempE = new List<Element>(new Element[5]);
        //for (int i = 0; i < 4; i++)
        //{
        //    tempE[i] = new Element(UnityEngine.Random.Range(1, 20));
        //}

        player = new Play("Player", "Partypoopers",5,0,1, DateTime.Now);
        enemy = new Play("Enemy", "Fishmongers", 5, 0, 1, DateTime.Now);

        if (player == null || enemy == null)
        {
            Debug.LogWarning("player or enemy missing");
            // Pseudo: Change gamestate back to settup, count as outcome = draw
            Application.Quit(); //for now I close the game enstead
        }

        //tempP = player.LoadTeam();
        //tempE = enemy.LoadTeam();

        //foreach (var e in tempP)
        //{
        //    playerTeam.Add(new BattleElement(e.GetElectrons(), e.GetShells()));
        //}
        //foreach (var e in tempE)
        //{
        //    enemyTeam.Add(new BattleElement(e.GetElectrons(), e.GetShells()));
        //}
    }

    public void PerformStep()
    {
        playerTeam.Sort(); Debug.Log(playerTeam.Count);
        enemyTeam.Sort(); Debug.Log(enemyTeam.Count);

        CalculateOutcome(playerTeam[0], enemyTeam[0]);

        Debug.Log("Player team: " + playerTeam);
        Debug.Log("Enemy team: " + enemyTeam);
        Debug.Log("____________________________________");
    }


    public void CalculateOutcome(BattleElement playerContester, BattleElement enemyContester)
    {
        playerContester.health -= enemyContester.dmg;
        enemyContester.health -= playerContester.dmg;

        if (playerContester.health <= 0) playerTeam.RemoveAt(0);
        if (enemyContester.health <= 0) enemyTeam.RemoveAt(0);
    }

    public void EnableAutoStep()
    {

    }
}