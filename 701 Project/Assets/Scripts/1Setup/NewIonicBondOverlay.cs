using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewIonicBondOverlay : MonoBehaviour
{
    public TMP_Text headerText;
    public Image unitImage;
    public Image unitMaskImage;

    public void SetOverlay(string chemicalName, Sprite unitSprite, Sprite unitMaskSprite, Color maskColor)
    {
        string modified = string.Format(headerText.text, chemicalName);
        headerText.text = modified;

        unitImage.sprite = unitSprite;
        unitMaskImage.sprite = unitMaskSprite;
        unitMaskImage.color = maskColor;
    }

    public void DestroySelf() { Destroy(gameObject); }

    public void NextTutorialPopUp()
    {
        if(TutorialManager.instance != null)
        {
            TutorialManager.instance.NextPopUp();
        }
    }
}