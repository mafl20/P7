using UnityEngine;
using TMPro;

public class SetupControl : MonoBehaviour
{
    public static SetupControl instance;

    #region Player Stats
    [Header("Player Stats")]
    [SerializeField] TMP_Text playerTeamNameText;
    [SerializeField] TMP_Text playerHeartsText;
    [SerializeField] TMP_Text playerRoundsText;
    [SerializeField] TMP_Text playerWinsText;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    public void SetPlayerStatsGUI()
    {
        var player = PlayerInformation.instance;

        playerTeamNameText.text = player.teamName;
        playerHeartsText.text = player.currentLives.ToString();
        playerRoundsText.text = player.currentRounds.ToString();
        playerWinsText.text = player.currentWins + "/" + player.winsToEnd;
    }
}