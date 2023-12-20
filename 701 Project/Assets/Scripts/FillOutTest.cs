using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillOutTest : MonoBehaviour
{
    #region Station
    [Header("Station")]
    public string activeStation_string = "";
    public string testNumber_string = "";
    [SerializeField] List<Button> stationButtons;
    Color[] originalButtonColors;
    [SerializeField] Color notSelectedButtonColor;
    #endregion

    #region Test Number
    [Header("Test Number")]
    [SerializeField] TMP_Text testNumberTitle;
    [SerializeField] TMP_InputField testNumberInputField;
    [SerializeField] GameObject randomUsernameButtonGO;
    [SerializeField] TMP_Text testIDTitle;
    public TMP_Text testIDResult;
    Vector3 originalTestIDResultPosition;
    [SerializeField] RectTransform usernameSpot;
    #endregion

    #region Properties
    [Header("Properties")]
    public bool isDev;
    #endregion

    #region External References
    [Header("External References")]
    [SerializeField] Button beginButton;
    [SerializeField] GameObject startTestButtonGO;
    [SerializeField] GameObject startDevButtonGO;
    [SerializeField] Toggle tutorialToggle;
    [SerializeField] GameObject timerGO;
    #endregion

    #region Font Scaling
    [Header("Font Scaling")]
    [SerializeField, Tooltip("1 is a good value.")] float minFontSize;
    [SerializeField, Tooltip("120 is a good value.")] float maxFontSize;
    #endregion

    private void Start()
    {
        testNumberInputField.onValueChanged.AddListener(SetTestNumber);

        originalButtonColors = new Color[stationButtons.Count];
        for (int i = 0; i < originalButtonColors.Length; i++)
        {
            originalButtonColors[i] = stationButtons[i].gameObject.GetComponent<Image>().color;
        }

        originalTestIDResultPosition = testIDResult.GetComponent<RectTransform>().position;

        isDev = false;
        UpdateTestIDResult();
    }

    public void SetActiveStation(string newString)
    {
        var buttonToHighlightIndex = 0;

        activeStation_string = newString + "_test_";

        #region Setting index for active station button
        testNumberTitle.text = "Test Number";
        if(isDev) { testNumberInputField.text = ""; }

        randomUsernameButtonGO.SetActive(false);
        testIDTitle.text = "Test ID Result";
        testIDResult.GetComponent<RectTransform>().position = originalTestIDResultPosition;

        isDev = false;

        beginButton.interactable = true;

        startTestButtonGO.SetActive(true);
        startDevButtonGO.SetActive(false);
        timerGO.SetActive(true);

        tutorialToggle.isOn = true;

        GameStateMachine.instance.allowBattleCheatEnd = false;

        switch (newString)
        {
            case "dev":
                buttonToHighlightIndex = 0;
                activeStation_string = "";
                testNumberTitle.text = "Username";
                testNumberInputField.text = "";
                randomUsernameButtonGO.SetActive(true);
                testIDTitle.text = "";
                testIDResult.GetComponent<RectTransform>().position = usernameSpot.position;
                isDev = true;
                startTestButtonGO.SetActive(false);
                startDevButtonGO.SetActive(true);
                timerGO.SetActive(false);
                tutorialToggle.isOn = false;
                GameStateMachine.instance.allowBattleCheatEnd = true;
                break;
            case "a":
                buttonToHighlightIndex = 1;
                break;
            case "b":
                buttonToHighlightIndex = 2;
                break;
            case "c":
                buttonToHighlightIndex = 3;
                break;
            case "d":
                buttonToHighlightIndex = 4;
                break;
            case "pilot":
                buttonToHighlightIndex = 5;
                break;
            default:
                break;
        }
        #endregion

        #region Going through and activating/deactivating buttons
        for (int i = 0; i < stationButtons.Count; i++)
        {
            if(i == buttonToHighlightIndex)
            {
                stationButtons[i].gameObject.GetComponent<Image>().color = originalButtonColors[i];
            }
            else
            {
                stationButtons[i].gameObject.GetComponent<Image>().color = notSelectedButtonColor;
            }
        }
        #endregion

        UpdateTestIDResult();
    }

    public void SetTestNumber(string newString)
    {
        testNumber_string = newString;
        UpdateTestIDResult();
    }

    public void UpdateTestIDResult()
    {
        testIDResult.text = activeStation_string + testNumber_string;
        ScaleText(testIDResult);

        if(isDev)
        {
            beginButton.interactable = true;
        }
        else
        {
            if (testNumber_string == "") { beginButton.interactable = false; }
            else { beginButton.interactable = true; }
        }
    }

    public void GenerateUsername()
    {
        var newUsername = PlayerInformation.instance.GenerateUsername();
        testNumberInputField.text = newUsername;
        SetTestNumber(newUsername);
    }

    void ScaleText(TMP_Text textComponent)
    {
        if (textComponent != null)
        {
            //> we set the text to a large font size temporarily to measure its preferred width
            textComponent.fontSize = maxFontSize;
            float preferredWidth = textComponent.preferredWidth;

            //> we adjust the font size based on the ratio of the current width to the desired width
            float maxWidth = textComponent.gameObject.GetComponent<RectTransform>().rect.width;
            float scaleFactor = maxWidth / preferredWidth;
            float newFontSize = Mathf.Clamp(textComponent.fontSize * scaleFactor, minFontSize, maxFontSize);

            //> w epply the new font size to the TMP_Text component
            textComponent.fontSize = newFontSize;
        }
    }

    public void SetIsFirstTimePlayingToToggle()
    {
        GameStateMachine.instance.isFirstTimePlaying = tutorialToggle.isOn;
    }
}