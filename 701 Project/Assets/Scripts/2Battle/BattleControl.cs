using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleControl : MonoBehaviour
{
    public static BattleControl instance;

    [SerializeField] GameObject battleOutcomeGUIGO;
    [SerializeField] GameObject nextButton, returnToMainMenuButton;
    [SerializeField] TMP_Text outcomeText;
    [SerializeField] GameObject winButtonGO, loseButtonGO, tieButtonGO;


    #region User Stats
    [Header("Player Stats")]
    [SerializeField] TMP_Text playerUsernameText;
    [SerializeField] TMP_Text playerTeamNameText;
    [SerializeField] TMP_Text playerHeartsText;
    [SerializeField] TMP_Text playerRoundsText;
    [SerializeField] TMP_Text playerWinsText;
    [Header("Enemy Stats")]
    [SerializeField] TMP_Text enemyUsernameText;
    [SerializeField] TMP_Text enemyTeamNameText;
    [SerializeField] TMP_Text enemyHeartsText;
    [SerializeField] TMP_Text enemyRoundsText;
    [SerializeField] TMP_Text enemyWinsText;
    #endregion

    #region Slots
    [Header("Player Team Slots")]
    public List<Slot> playerSlots;
    [Header("Enemy Team Slots")]
    public List<Slot> enemySlots;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        winButtonGO.SetActive(GameStateMachine.instance.allowBattleCheatEnd);
        loseButtonGO.SetActive(GameStateMachine.instance.allowBattleCheatEnd);
        tieButtonGO.SetActive(GameStateMachine.instance.allowBattleCheatEnd);
    }

    public void SetTeamsGUI()
    {
        var player = PlayerInformation.instance;

        playerUsernameText.text = player.username;
        playerTeamNameText.text = player.teamName;
        playerHeartsText.text   = player.currentLives.ToString();
        playerRoundsText.text   = player.currentRounds.ToString();
        playerWinsText.text     = player.currentWins.ToString();

        byte i = 0;
        foreach (Slot slot in playerSlots) //for every slot in the player team slots
        {
            if(player.currentPlay.GetAllPlayInfo()[5 + i][0] == ' ') //if there is no unit at this spot in the team, i.e., the chemical name at 5 + i == " "
            {
                //Debug.Log("No unit here...");
                i++;
                continue;
            }

            //Debug.Log("Unit with name " + player.currentPlaythrough.LoadPlay(0).GetAllPlayInfo()[5 + i] + " found");

            GameObject newUnitGO = UnitCreator.instance.CreateNewUnit(player.currentPlay.GetAllPlayInfo()[5 + i], slot.unitSpot.position, slot.unitSpot);
            if(newUnitGO.GetComponent<UnitProperties>().elementBGO == null)
            { newUnitGO.GetComponent<UnitProperties>().isIon = true; }
            newUnitGO.GetComponent<UnitProperties>().UpdateUnitProperties();
            newUnitGO.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();
            slot.unitGO = newUnitGO; //assigning the newly generate element game object to the slot
            i++;
        }



        var enemy = EnemyFinder.instance.foundEnemy;

        enemyUsernameText.text = enemy[0]; //enemy USERNAME
        enemyTeamNameText.text = enemy[1]; //enemy TEAM NAME
        enemyHeartsText.text   = enemy[2]; //enemy HEARTS
        enemyRoundsText.text   = enemy[3]; //enemy ROUNDS
        enemyWinsText.text     = enemy[4]; //enemy WINS

        i = 0;
        foreach (Slot slot in enemySlots) //for every slot in the enemy team slots
        {
            if (enemy[5 + i][0] == ' ') //if there is no unit at this spot in the team, i.e., the chemical name at 5 + i == " "
            {
                //Debug.Log("No unit here...");
                i++;
                continue;
            }

            //Debug.Log("Unit with name " + enemy[5 + i] + " found");

            GameObject newUnitGO = UnitCreator.instance.CreateNewUnit(enemy[5 + i], slot.unitSpot.position, slot.unitSpot);
            if (newUnitGO.GetComponent<UnitProperties>().elementBGO == null)
            { newUnitGO.GetComponent<UnitProperties>().isIon = true; }

            newUnitGO.GetComponent<UnitGraphics>().isEnemy = true;
            newUnitGO.GetComponent<UnitProperties>().UpdateUnitProperties();
            newUnitGO.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();

            #region Flipping the image

            //var localScaleX = -1;
            //var localScaleY = newUnitGO.transform.GetChild(0).GetComponent<RectTransform>().localScale.y;
            //var localScaleZ = newUnitGO.transform.GetChild(0).GetComponent<RectTransform>().localScale.z;
            //newUnitGO.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(localScaleX, localScaleY, localScaleZ);

            //localScaleY = newUnitGO.transform.GetChild(1).GetComponent<RectTransform>().localScale.y;
            //localScaleZ = newUnitGO.transform.GetChild(1).GetComponent<RectTransform>().localScale.z;
            //newUnitGO.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
            #endregion

            slot.unitGO = newUnitGO; //assigning the newly generate element game object to the slot
            i++;
        }
    }

    public void ClearBattleGUI()
    {
        foreach (Slot slot in playerSlots) //for every slot in the enemy team slots
        {
            slot.DeleteUnitGO();
        }

        foreach (Slot slot in enemySlots) //for every slot in the enemy team slots
        {
            slot.DeleteUnitGO();
        }
    }

    public void ButtonWinRound()
    {
        PlayerInformation.instance.IncrementCurrentWins(1);

        outcomeText.text = "DU VANDT!";

        #region Playing SOUND
        AudioManager.instance.Play("PlayerWonRound");
        #endregion

        if (PlayerInformation.instance.currentWins >= PlayerInformation.instance.winsToEnd)
        {
            //TODO: hide next button
            nextButton.SetActive(false);
            //TODO: display that the playthrough has been won
            outcomeText.text = "DU VANDT DET HELE!!!";
            //TODO: show return to main menu button
            returnToMainMenuButton.SetActive(true);

            #region Playing SOUND
            AudioManager.instance.Play("PlayerWonRound");
            AudioManager.instance.Play("PlayerWonGameMusic");
            #endregion
        }

        battleOutcomeGUIGO.SetActive(true);
    }

    public void ButtonLoseRound()
    {
        PlayerInformation.instance.DecrementCurrentLives(1);

        //TODO: edit outcome text to "du tabte..."
        outcomeText.text = "du tabte...";

        #region Sounds
        AudioManager.instance.Play("PlayerLostRound");
        #endregion

        //NudgeManager.instance.HandleLossStreak();

        if (PlayerInformation.instance.currentLives <= 0)
        {
            //TODO: hide next button
            nextButton.SetActive(false);
            //TODO: display that the playthrough has been lost
            outcomeText.text = "du løb tør for liv...";
            //TODO: show return to main menu button
            returnToMainMenuButton.SetActive(true);

            #region Sounds
            AudioManager.instance.Play("PlayerLostRound");
            AudioManager.instance.Play("PlayerLostGameMusic");
            #endregion
        }

        battleOutcomeGUIGO.SetActive(true);
    }

    public void ButtonTieRound()
    {
        //PlayerInformation.instance.currentPlaythrough.DecrementCurrentLives(0);

        //TODO: edit outcome text to "du tabte..."
        outcomeText.text = "Det endte uafgjort";

        battleOutcomeGUIGO.SetActive(true);
    }
}