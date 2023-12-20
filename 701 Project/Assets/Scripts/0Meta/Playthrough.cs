using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Playthrough
{
    public List<Play> plays = new List<Play>();

    string username, teamName;
    List<string> currentTeam;
    List<string> currentShop;
    List<bool> currentFrozenIndices;
    byte currentNumLives, currentNumRounds, currentNumWins;

    public Playthrough()
    {
        username             = "User without name";
        teamName             = "Team without name";
        currentTeam          = new List<string>();
        currentShop          = new List<string>();
        currentFrozenIndices = new List<bool>();
        currentNumLives      = PlayerInformation.instance.startLives;
        currentNumRounds     = 0;
        currentNumWins       = 0;
    }

    public Playthrough(string username, string teamName)
    {
        this.username        = username;
        this.teamName        = teamName;
        currentTeam          = new List<string>();
        currentShop          = new List<string>();
        currentFrozenIndices = new List<bool>();
        currentNumLives      = PlayerInformation.instance.startLives;
        currentNumRounds     = 1;
        currentNumWins       = 0;
    }

    public Playthrough(string username, string teamName, List<string> team, List<string> shop, List<bool> frozenIndices, byte numLives, byte numRounds, byte numWins)
    {
        this.username        = username;
        this.teamName        = teamName;
        currentTeam          = team;
        currentShop          = shop;
        currentFrozenIndices = frozenIndices;
        currentNumLives      = numLives;
        currentNumRounds     = numRounds;
        currentNumWins       = numWins;
    }

    public void SetCurrentStats(List<string> newTeam, List<string> newShop, List<bool> newFrozenIndices, byte newNumLives, byte newNumRounds, byte newNumWins)
    {
        currentTeam          = newTeam;
        currentShop          = newShop;
        currentFrozenIndices = newFrozenIndices;
        currentNumLives      = newNumLives;
        currentNumRounds     = newNumRounds;
        currentNumWins       = newNumWins;

        Debug.Log("Set current play stats");
    }

    public void SetCurrentStats(string newName, List<string> newTeam, List<string> newShop, List<bool> newFrozenIndices, byte newNumLives, byte newNumRounds, byte newNumWins)
    {
        teamName = newName;
        currentTeam = newTeam;
        currentShop = newShop;
        currentFrozenIndices = newFrozenIndices;
        currentNumLives = newNumLives;
        currentNumRounds = newNumRounds;
        currentNumWins = newNumWins;

        Debug.Log("Set current play stats");
    }
    public void SetCurrentStats(string newUsername, string newName, List<string> newTeam, List<string> newShop, List<bool> newFrozenIndices, byte newNumLives, byte newNumRounds, byte newNumWins)
    {
        username = newUsername;
        teamName = newName;
        currentTeam = newTeam;
        currentShop = newShop;
        currentFrozenIndices = newFrozenIndices;
        currentNumLives = newNumLives;
        currentNumRounds = newNumRounds;
        currentNumWins = newNumWins;

        Debug.Log("Set current play stats");
    }

    /// <summary>
    /// Creates a new Play object with the current play information and stores in the list of plays.
    /// </summary>
    public void SaveCurrentStatsAsPlay()
    {
        Play play = new Play(username, teamName, currentNumLives, currentNumWins, currentNumRounds, currentTeam, currentShop, currentFrozenIndices, DateTime.Now);
        plays.Insert(0, play);
        LogPlay(play);

        Debug.Log("Saved current play...");
    }

    public void LogPlay(Play play)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "../Data/Plays/" + currentNumLives + "Hearts" + currentNumWins + "Wins.txt");
        string[] content = play.GetAllPlayInfo();

        if (File.Exists(path)) { AddContentToPlayLog(path, content); }
        else                   { CreateAndAddContentToPlayLog(path, content); }
    }

    void AddContentToPlayLog(string path, string[] content)
    {
        //File.WriteAllLines(path, content);
        string pastContent = File.ReadAllText(path);
        string formattedContent = string.Join(",", content);
        File.WriteAllText(path, formattedContent + pastContent);
    }

    void CreateAndAddContentToPlayLog(string path, string[] content)
    {
        //File.AppendAllLines(path, content);
        string formattedContent = string.Join(",", content);
        File.AppendAllText(path, formattedContent);
    }

    /// <summary>
    /// Creates a new Play object and stores in the list of plays.
    /// </summary>
    public Play LoadPlay(int index)
    {
        return plays[index];
    }

    public void DeletePlay(int index) { plays.RemoveAt(index); }

    public Play GetPlayByIndex(int index)
    {
        return plays[index];
    }

    public void SetTeamName(string newName) { teamName = newName; }
    public void SetCurrentLives(byte newLives) { currentNumLives = newLives; }
    public void SetCurrentRounds(byte newRounds) {  currentNumRounds = newRounds; }
    public void SetCurrentWins(byte newWins) {  currentNumWins = newWins; }

    public string GetTeamName() { return teamName; }
    public byte GetCurrentLives() { return currentNumLives; }
    public byte GetCurrentRounds() { return currentNumRounds; }
    public byte GetCurrentWins() { return currentNumWins; }

    public void IncrementCurrentLives(byte increase)  { currentNumLives  += increase; }
    public void IncrementCurrentRounds(byte increase) { currentNumRounds += increase; }
    public void IncrementCurrentWins(byte increase)   { currentNumWins   += increase; }

    public void DecrementCurrentLives(byte decrease)  { currentNumLives  -= decrease; }
    public void DecrementCurrentRounds(byte decrease) { currentNumRounds -= decrease; }
    public void DecrementCurrentWins(byte decrease)   { currentNumWins   -= decrease; }
}