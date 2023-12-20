using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInformation : MonoBehaviour
{
    public static PlayerInformation instance;
    //public List<Playthrough> playthroughs = new List<Playthrough>();
    //public Playthrough currentPlaythrough;
    public Play currentPlay;

    public string username;
    public string teamName;
    public byte startLives;
    public byte currentLives;
    public byte currentRounds;
    public byte winsToEnd;
    public byte currentWins;
    public byte highestAvailableTier;

    public List<string> currentTeam;
    public List<string> currentShop;
    public List<bool> currentFrozenIndices;

    #region Graphics
    [SerializeField] TMP_Text testIDResult;
    [SerializeField] TMP_Text usernameText, teamNameText;
    [SerializeField] TMP_Text heartsText, roundsText, winsText;
    #endregion

    public List<string> ionicBondsMade;

    private void Awake() { instance = this; }

    void StartNewPlaythrough()
    {
        string teamName = GenerateTeamName();
        Debug.Log("User " + username + " started new playthrough with team name " + teamName);
    }

    void SetTeam() { currentTeam = TeamControl.instance.GetTeam(); }

    void SetShop() { currentShop = ShopControl.instance.GetShop(); }

    void SetFrozenIndices() { currentFrozenIndices = ShopControl.instance.GetFrozenIndices(); }

    public void SaveCurrentPlay()
    {
        SetTeam();
        SetShop();
        SetFrozenIndices();

        currentPlay = new Play(username, teamName, currentLives, currentWins, currentRounds, currentTeam, currentShop, currentFrozenIndices, DateTime.Now);

        string path = "";
        if (ElementDataReader.instance.isBuild) { path = Application.dataPath + "/Resources/" + currentLives + "Hearts" + currentWins + "Wins.txt"; }
        else { path = Path.Combine(Application.streamingAssetsPath, "../Data/Plays/" + currentLives + "Hearts" + currentWins + "Wins.txt"); }

        string[] content = currentPlay.GetAllPlayInfo();

        if (File.Exists(path)) { AddContentToPlayLog(path, content); }
        else { CreateAndAddContentToPlayLog(path, content); }

        Debug.Log("Saved current play...");
    }

    void AddContentToPlayLog(string path, string[] content)
    {
        string pastContent = File.ReadAllText(path);
        string formattedContent = string.Join(",", content);
        File.WriteAllText(path, formattedContent + pastContent);
    }

    void CreateAndAddContentToPlayLog(string path, string[] content)
    {
        string formattedContent = string.Join(",", content);
        File.AppendAllText(path, formattedContent);
    }

    public string GenerateTeamName()
    {
        string path = "";
        if (ElementDataReader.instance.isBuild) { path = Application.dataPath + "/Resources/"; }
        else { path = Path.Combine(Application.streamingAssetsPath, "../Data/"); }

        string adjective = GetRandomWord(path + "adjectives.txt");
        string noun      = GetRandomWord(path + "nouns.txt");
        string teamName  = "The " + adjective + " " + noun;

        return teamName;
    }

    string GetRandomWord(string path)
    {
        string[] lines = File.ReadAllLines(path);
        int index = UnityEngine.Random.Range(0, lines.Length);

        string word = lines[index];
        return word;
    }

    public void SetUsernameFromTMPText() //called from button beneath input text field in main menu
    {
        username = testIDResult.text;
        if(username == "")
        {
            username = GenerateUsername();
        }
    }

    public string GenerateUsername()
    {
        string path = "";
        if(ElementDataReader.instance.isBuild) { path = Application.dataPath + "/Resources/"; }
        else { path = Path.Combine(Application.streamingAssetsPath, "../Data/"); }


        string username = "";

        var randomChance = UnityEngine.Random.Range(0, 2);
        //Debug.Log("Random chance for nametype: " + randomChance);
        if (randomChance == 0)
        {
            string firstName = GetRandomWord(path + "first_names.txt");
            username += firstName;
        }
        else
        {
            string gamerTag = GetRandomWord(path + "gamer_tags.txt");
            username += gamerTag;
        }

        randomChance = UnityEngine.Random.Range(0, 2);
        //Debug.Log("Random chance for suffix number: " + randomChance);
        if (randomChance == 1)
        {
            string number = UnityEngine.Random.Range(0, 2025).ToString();
            username += number;
        }

        return username;
    }

    public void DecrementCurrentLives(byte decrease) { currentLives -= decrease; }

    public void IncrementCurrentRounds(byte increase)
    {
        currentRounds += increase;

        //TODO: the cases of this switch statement are HARDCODED!!!

        switch (currentRounds)
        {
            case 1: SetTier(1); break;
            case 3: SetTier(2); break;
            case 5: SetTier(3); break;
            case 7: SetTier(4); break;
            default: break;
        }
    }

    public void SetTier(byte tier)
    {
        SetHighestAvailableTier(tier);
        UnitCreator.instance.UpdateActiveElements(tier);

        #region Tier Unlock Overlay
        /*
         * this code takes care of finding the appropriate sprites for the 'new tier unlocked'-overlaying GUI
        */
        ShopControl.instance.ToggleTierUnlockOverlay(true);
        TierUnlockOverlayControl.instance.DeleteImages();

        byte numberOfImages = 0;
        byte[] protons = new byte[numberOfImages];

        if      (tier == 2) { numberOfImages = 6; protons = UnitCreator.instance.tier2; Handbook.instance.__EnablePeriod(3); } //HARDCODED!!!
        else if (tier == 3) { numberOfImages = 4; protons = UnitCreator.instance.tier3; Handbook.instance.__EnablePeriod(4); }
        else if (tier == 4) { numberOfImages = 2; protons = UnitCreator.instance.tier4; Handbook.instance.__EnablePeriod(5); }

        Sprite[] sprites = new Sprite[numberOfImages];

        for (int i = 0; i < numberOfImages; i++)
        {
            sprites[i] = UnitSpriteHolder.instance.GetElementSpriteFromNumberOfProtons(protons[i]);
        }

        TierUnlockOverlayControl.instance.SpawnImages(numberOfImages, sprites);
        TierUnlockOverlayControl.instance.SetIsPlaying(true);
        #endregion
    }

    public void IncrementCurrentWins(byte increase) { currentWins += increase; }

    public void SetHighestAvailableTier(byte newTier)
    {
        highestAvailableTier = newTier;
        ShopControl.instance.highestAvailableTier = newTier;
    }

    public void ResetAllInformation()
    {
        username = "";
        teamName = "";
        
        ResetStats();
    }

    public void ResetStats()
    {
        currentLives = startLives;
        currentRounds = 1;
        currentWins = 0;

        SetHighestAvailableTier(1);
        UnitCreator.instance.ResetActiveElements();

        currentTeam = new List<string>();
        currentShop = new List<string>();
        currentFrozenIndices = new List<bool>();

        Handbook.instance.__DisableAll();
    }
}