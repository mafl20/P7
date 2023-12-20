using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanelControl : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] Image compoundMaskImage;
    [SerializeField] GameObject compoundMaskGO;
    [SerializeField] TMP_Text chemicalNameText;
    [SerializeField] TMP_Text tierText;
    [SerializeField] TMP_Text unitTypeText;
    [SerializeField] TMP_Text worthText;
    [SerializeField] TMP_Text chargeText;
    [SerializeField] Image typeImage;
    [SerializeField] Sprite cationSprite, anionSprite, ionicBondSprite;

    public void SetPanel(string chemicalName, byte tier, string unitType, int charge, byte worth, Sprite unitSprite, Sprite maskSprite, Color hexColor, bool isCompound)
    {
        if (isCompound)
        {
            compoundMaskGO.SetActive(true);
        }
        else
        {
            compoundMaskGO.SetActive(false);
        }

        chemicalNameText.text    = chemicalName;

        #region TIER
        string tierString = "";
        if (tier == 1) { tierString = "I"; }
        else if (tier == 2) { tierString = "II"; }
        else if (tier == 3) { tierString = "III"; }
        else if (tier == 4) { tierString = "IV"; }
        tierText.text = tierString;
        #endregion

        #region CHARGE
        var absoluteCharge = Mathf.Abs(charge);
        chargeText.text = absoluteCharge.ToString();
        if      (charge == 0) { typeImage.sprite = ionicBondSprite; }
        else if (charge > 0)  { typeImage.sprite = cationSprite; }
        else if (charge < 0)  { typeImage.sprite = anionSprite; }
        #endregion

        unitTypeText.text = unitType;

        worthText.text           = worth.ToString();

        unitImage.sprite         = unitSprite;
        compoundMaskImage.sprite = maskSprite;
        compoundMaskImage.color  = hexColor;
    }
}