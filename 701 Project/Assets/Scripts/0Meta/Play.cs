using System.Collections.Generic;
using UnityEngine;
using System;

public class Play
{
    string username, teamName;
    List<string> team;
    List<string> shop;
    List<bool> frozenIndices;
    int numLives, numWins, numRounds;
    DateTime recordedDateTime;

    bool sold;
    int tier;

    public static byte roundsDataIndex = 3;
    public Play() 
    {
        username      = "User";
        teamName      = "Team";
        numLives      = 5;
        numWins       = 0;
        numRounds     = 0;
        team          = new List<string>();
        shop          = new List<string>();
        frozenIndices = new List<bool>();
        recordedDateTime = DateTime.Now;
    }

    public Play(string username, string teamName, int numLives, int numWins, int numRounds, DateTime dateTime)
    {
        this.username  = username;
        this.teamName  = teamName;

        this.numLives  = numLives;
        this.numWins   = numWins;
        this.numRounds = numRounds;

        team          = new List<string>();
        shop          = new List<string>();
        frozenIndices = new List<bool>();

        recordedDateTime = dateTime;
    }

    public Play(string username, string teamName, int numLives, int numWins, int numRounds, List<string> team, List<string> shop, List<bool> frozenIndices, DateTime dateTime)
    {
        this.username      = username;
        this.teamName      = teamName;
        this.numLives      = numLives;
        this.numWins       = numWins;
        this.numRounds     = numRounds;
        this.team          = team;
        this.shop          = shop;
        this.frozenIndices = frozenIndices;
        recordedDateTime = dateTime;
    }

    /// <summary>
    /// Returns the team name, number of lives, wins, and rounds left.
    /// </summary>
    public string[] GetBasicPlayInfo()
    {
        string[] playInfo = { username, teamName, numLives.ToString(), numWins.ToString(), numRounds.ToString() };
        return playInfo;
    }

    /// <summary>
    /// Returns all information within the Play instance.
    /// </summary>
    public string[] GetAllPlayInfo()
    {
        string[] teamUnitNames = new string[team.Count]; //Debug.Log("team size: " + teamUnitNames.Length);
        for (int i = 0; i < teamUnitNames.Length; i++)
        {
            if(team[i] == null) { teamUnitNames[i] = ""; }
            else                { teamUnitNames[i] = team[i]; }
        }

        string[] shopUnitNames = new string[shop.Count]; //Debug.Log("shop size: " + shopUnitNames.Length);
        for (int i = 0; i < shopUnitNames.Length; i++)
        {
            if (shop[i] == null) { shopUnitNames[i] = ""; }
            else                 { shopUnitNames[i] = shop[i]; }
        }

        string[] playInfo = {
            username,
            teamName,

            numLives.ToString(),
            numRounds.ToString(),
            numWins.ToString(),

            teamUnitNames[0],
            teamUnitNames[1],
            teamUnitNames[2],
            teamUnitNames[3],
            teamUnitNames[4],

            shopUnitNames[0],
            shopUnitNames[1],
            shopUnitNames[2],

            frozenIndices[0].ToString(),
            frozenIndices[1].ToString(),
            frozenIndices[2].ToString(),

            recordedDateTime.ToString(),
            "\n"
        };
        return playInfo;
    }

    /// <summary>
    /// Returns information about what elements are in the team.
    /// </summary>
    public string[] GetPlaysTeamInfo()
    {
        string[] playInfo = new string[team.Count];
        for (int i = 0; i < playInfo.Length; i++)
        {
            playInfo[i] = team[i];
        }
        return playInfo;
    }

    /// <summary>
    /// Returns information about what elements are in the shop.
    /// </summary>
    public string[] GetPlaysShopInfo()
    {
        string[] playInfo = new string[shop.Count];
        for (int i = 0; i < playInfo.Length; i++)
        {
            playInfo[i] = shop[i];
        }
        return playInfo;
    }

    /// <summary>
    /// Returns information (in string format) about what slots in the shop are frozen.
    /// </summary>
    public string[] GetPlaysFrozenIndicesInfo_String()
    {
        string[] playInfo = new string[frozenIndices.Count];
        for (int i = 0; i < playInfo.Length; i++)
        {
            playInfo[i] = "Slot " + i + " of the shop is frozen";
        }
        return playInfo;
    }

    /// <summary>
    /// Returns information (in boolean format) about what slots in the shop are frozen.
    /// </summary>
    public bool[] GetPlaysFrozenIndicesInfo_Bool()
    {
        bool[] playInfo = new bool[frozenIndices.Count];
        for (int i = 0; i < playInfo.Length; i++)
        {
            playInfo[i] = frozenIndices[i];
        }
        return playInfo;
    }

    public void SaveTeam(List<string> team) { this.team = team; }
    public List<string> LoadTeam() { return team; }
    public void SaveShop(List<string> shop, List<bool> frozenIndices) { this.shop = shop; this.frozenIndices = frozenIndices; }
    public (List<string>, List<bool>) LoadShop() {  return (shop, frozenIndices); }

    //> EDIT: THIS IS MORE RELEVANT FOR THE SHOPCONTROL.cs script
    // FOR DEBUGGING STUFF
    //public void buttonSoldDebug()
    //{
    //    sold = true;
    //    if (int.TryParse(inputText.text, out int parsedTier))
    //    {
    //        tier = parsedTier;
    //    }
    //    else
    //    {
    //        // Handle the case where the text cannot be parsed as an integer
    //        Debug.LogError("Failed to parse soldTier as an integer");
    //    }

    //    ChangeGoldVariable(sold, tier);

    //    // Reset the values
    //    sold = false;
    //    tier = 0;
    //}

    //// FOR DEBUGGING STUFF
    //public void buttonBoughtDebug()
    //{
    //    sold = false;
    //    if (int.TryParse(inputText.text, out int parsedTier))
    //    {
    //        tier = parsedTier;
    //    }
    //    else
    //    {
    //        // Handle the case where the text cannot be parsed as an integer
    //        Debug.LogError("Failed to parse boughtTier as an integer");
    //    }
    //    ChangeGoldVariable(sold, tier);
    //}

    //// This method changes the value of the numGold variable based on whether the player has sold an item
    //void ChangeGoldVariable(bool sold, int tier)
    //{
    //    // Log the values of sold and tier to the console
    //    Debug.Log($"{sold},{tier}");

    //    // If the player has sold an item, add gold to the numGold variable
    //    if (sold)
    //    {
    //        int goldToAdd = tier;

    //        if (goldToAdd > 0 && numGold < maxGold)
    //        {
    //            numGold += goldToAdd;
    //        }
    //    }

    //    // If the player has not sold an item, remove gold from the numGold variable
    //    if (!sold)
    //    {
    //        int goldToRemove = tier;

    //        if (goldToRemove > 0 && numGold >= goldToRemove)
    //        {
    //            numGold -= goldToRemove;
    //        }
    //    }
    //}

    // This method returns an array of strings that contains information about the player's progress
}