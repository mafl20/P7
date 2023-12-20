using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EnemyFinder : MonoBehaviour
{
    public static EnemyFinder instance;
    public string[] foundEnemy;
    [SerializeField] GameObject tutorialManagerGO;

    private void Awake()
    {
        instance = this;
    }

    public void FindEnemy(bool isRandom)
    {
        // If this is the first time playing, then we want to battle against a fixed enemy
        if (tutorialManagerGO.activeInHierarchy)
        {
            Debug.Log("First time playing, so this battle is fixed...");
            foundEnemy = ("Brian,Brian's Hold,5,1,0,          ,          ,1 Lithium 1 Fluor 1 ,1 Nitrogen 1   ,          ,,Nitrogen,Nitrogen,False,False,False,23-11-2023 21:34:11,").Split(',');
            return;
        }

        if (isRandom == true)
        {
            foundEnemy = FindRandomEnemy();
        }
        else
        {
            byte hearts = PlayerInformation.instance.currentLives;
            byte rounds = PlayerInformation.instance.currentRounds;
            byte wins   = PlayerInformation.instance.currentWins;

            foundEnemy = FindEnemy(hearts, rounds, wins);
        }
    }

    string[] FindEnemy(byte hearts, byte rounds, byte wins)
    {
        //We define the path that we want to look in

        string path = "";
        if (ElementDataReader.instance.isBuild) { path = Application.dataPath + "/Resources/" + hearts + "Hearts" + wins + "Wins.txt"; }
        else { path = Path.Combine(Application.streamingAssetsPath, "../Data/Plays/" + hearts + "Hearts" + wins + "Wins.txt"); }

        //path = GetPathWithLessHeartsAndWins(path, hearts, wins); //we want to return an enemy play that has the same amount of hearts and wins as us
        if (TryFindPath(path) == false) //if the path does not exist
        {
            Debug.Log("Path does not exist, so cannot find a play with similar or less hearts (<=" + hearts + "), and similar or less wins (<=" + wins + ").");
            //path = GetPathWithLessHeartsAndWins(path, hearts, wins);
            //return FindRandomEnemy(); //we want to find a random enemy

            Debug.Log("Battle against yourself...");
            return PlayerInformation.instance.currentPlay.GetAllPlayInfo();
        }

        List<int> similarPlays = GetPlaysWithSimilarRounds(path, rounds);
        if (similarPlays.Count <= 0) { similarPlays = GetPlaysWithLessRounds(path, rounds); }
        //if (similarPlays.Count <= 0) { similarPlays = GetPlaysWithLessHearts(path, hearts, rounds, wins); }
        //if (similarPlays.Count <= 0) { similarPlays = GetPlaysWithLessWins(path, hearts, rounds, wins); }

        int randomIndex = Random.Range(0, similarPlays.Count);
        //int index = GetPlayIndexWithSimilarRounds(path, rounds); //find a play with similar 
        int chosenIndex = similarPlays[randomIndex];

        string[] chosenEnemy = GetPlay(path, chosenIndex);
        //Debug.Log("Found Play with similar amount of played rounds " + i + " in " + path + ".");
        Debug.Log("Chose Play " + chosenIndex + " with hearts " + chosenEnemy[2] + ", rounds " + chosenEnemy[3] + ", and wins " + chosenEnemy[4] + ".");
        return chosenEnemy;
    }

    //string[] FindEnemyWithLowerStats(byte hearts, byte rounds, byte wins)
    //{

    //}

    string[] FindRandomEnemy()
    {
        byte randomHearts = (byte)Random.Range(1, PlayerInformation.instance.startLives);
        byte randomWins = (byte)Random.Range(0, PlayerInformation.instance.winsToEnd);

        string path = "";
        if (ElementDataReader.instance.isBuild) { path = Application.dataPath + "/Resources/" + randomHearts + "Hearts" + randomWins + "Wins.txt"; }
        else { path = Path.Combine(Application.streamingAssetsPath, "../Data/Plays/" + randomHearts + "Hearts" + randomWins + "Wins.txt"); }

        path = GetPathWithLessHeartsAndWins(path, randomHearts, randomWins);
        if (TryFindPath(path) == false)
        {
            Debug.Log("Could not find a play with similar or less hearts (<=" + randomHearts + "), and similar or less wins (<=" + randomWins + ").");
            Debug.Log("Battle against yourself...");
            return PlayerInformation.instance.currentPlay.GetAllPlayInfo();
        }

        int randomIndex = Random.Range(0, GetPlays(path).Length);

        string[] chosenEnemy = GetPlay(path, randomIndex);
        Debug.Log("Chose Play with hearts " + chosenEnemy[2] + ", rounds " + chosenEnemy[3] + ", and wins " + chosenEnemy[4] + ".");
        return chosenEnemy;
    }

    string[] GetPlays(string path)
    {
        return File.ReadAllLines(path);
    }

    string[] GetPlay(string path, int index)
    {
        return GetPlays(path)[index].Split(',');
    }

    List<int> GetPlaysWithSimilarRounds(string path, byte rounds)
    {
        List<int> indices = new List<int>();
        string[] data = GetPlays(path);
        string[] play;
        byte foundRounds = 0;

        for (int i = 1; i < data.Length; i++) //we start at 1, so that we don't meet ourselves
        {
            Debug.Log("Searching index " + i + " for enemy...");
            play = data[i].Split(',');

            if(play.Equals(PlayerInformation.instance.currentPlay.GetAllPlayInfo()))
            {
                Debug.Log("This Play is the current Play of the player.");
                continue;
            }

            byte.TryParse(play[Play.roundsDataIndex], out foundRounds);

            if(foundRounds == rounds)
            {
                Debug.Log("Found Play " + i + " with similar amount of played rounds " + rounds + " in " + path + ".");
                indices.Add(i);
            }
            else
            {
                Debug.Log("Did NOT find enemy at index " + i);
            }
        }

        return indices;
    }

    List<int> GetPlaysWithLessRounds(string path, byte rounds)
    {
        var list = new List<int>();

        for (byte i = (byte)(rounds - 1); i > rounds; i--)
        {
            list = GetPlaysWithSimilarRounds(path, i);
            if(list.Count != 0) { return list; }
        }

        return list;
    }

    int GetPlayIndexWithSimilarRounds(string path, byte rounds)
    {
        string[] data = GetPlays(path); //storing all plays in a file
        string[] play; //for storing a single play
        byte playedRounds = 0; //for storing number of rounds played in that play

        for (int i = 1; i < data.Length; i++) //iterating through all data, except the newest, since that might be the players current Play
        {
            play = data[i].Split(','); //store a single play
            byte.TryParse(play[Play.roundsDataIndex], out playedRounds); //convert number of rounds played from string to byte

            if(play.Equals(PlayerInformation.instance.currentPlay.GetAllPlayInfo())) //if the current play is just this players current play
            {
                Debug.Log("This Play is the current Play of the player.");
                continue; //go to the next iteration in the for loop
            }

            if(playedRounds == rounds) //check if these two are the same
            {
                Debug.Log("Found Play with similar amount of played rounds " + i + " in " + path + ".");
                return i; //if so, this is the play we want to battle against
            }
        }

        Debug.Log("Did not find a Play with similar amount of played rounds. Returned index 0");
        return 0;
    }

    public bool TryFindPath(string path)
    {
        return File.Exists(path);
    }

    public string GetPathWithLessHeartsAndWins(string path, byte hearts, byte wins)
    {
        string newPath = path;

        if (TryFindPath(newPath) == false) //if the path does not exist
        {
            for (int i = hearts; i > 0; i--) //we enumerate from hearts to 1
            {
                for (int j = wins; j > 0; j--) //we enumerate from wins to 1
                {
                    newPath = "";

                    if (ElementDataReader.instance.isBuild) { newPath = Application.dataPath + "/Resources/" + i + "Hearts" + j + "Wins.txt"; }
                    else { newPath = Path.Combine(Application.streamingAssetsPath, "../Data/Plays/" + i + "Hearts" + j + "Wins.txt"); }

                    if (TryFindPath(newPath) == true) //if a path with less hearts and/or wins is found
                    {
                        Debug.Log("Succeeded find at " + i + "Hearts" + j + "Wins.txt");
                        return newPath; //we return that path
                    }

                    Debug.Log("Failed find at " + i + "Hearts" + j + "Wins.txt");
                }
            }
        }

        return path; //if we failed at finding a path with lower stats than our current hearts and wins, we return the original path
    }
}