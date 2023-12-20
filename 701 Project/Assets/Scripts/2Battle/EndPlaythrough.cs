using UnityEngine;

public class EndPlaythrough : MonoBehaviour
{
    [SerializeField] GameObject playBattleButtonGO;
    [SerializeField] GameObject continueButtonGO;
    [SerializeField] GameObject battleOutcomeGO;

    public void EndPlaythroughButtonClick()
    {
        playBattleButtonGO.SetActive(true);
        continueButtonGO.SetActive(true);
        battleOutcomeGO.SetActive(false);

        GameStateMachine.instance.ChangeToMainMenuState();
    }
}