using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    #region Properties
    [Header("Properties")]
    [SerializeField] float maxTimeSeconds;
    [SerializeField] float timeLeftSeconds;
    public bool isRunning;
    #endregion

    #region Graphics
    [Header("Graphics")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject timerFinishedGO;
    #endregion

    private void Start()
    {
        timeLeftSeconds = maxTimeSeconds;
        timerText.text = SecondsToClock();
    }

    private void Update()
    {
        if(isRunning)
        {
            timeLeftSeconds -= Time.deltaTime;

            if (timeLeftSeconds <= 0)
            {
                //TODO: End game session
                timeLeftSeconds = 0;
                isRunning = false;
                timerFinishedGO.SetActive(true);
            }

            timerText.text = SecondsToClock();
        }
    }

    public void UnpauseTimer() { isRunning = true; }

    public void PauseTimer() { isRunning = false; }

    public void ResetTimer()
    {
        timeLeftSeconds = maxTimeSeconds;
        isRunning = false;
        timerText.text = SecondsToClock();
        timerFinishedGO.SetActive(false);
    }

    string SecondsToClock()
    {
        var seconds = (int)(Mathf.Ceil(timeLeftSeconds) % 60);
        var minutes = (int)((Mathf.Ceil(timeLeftSeconds) / 60) % 60);

        return $"{minutes:00}:{seconds:00}";
    }
}