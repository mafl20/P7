using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using System.Collections;
using System;

public class UnitGraphics : MonoBehaviour
{
    public Sprite unitSprite, maskSprite;
    public Image compoundImage, compoundMaskImage;
    public Color maskColor;
    [SerializeField] GameObject maskImageGO;
    public TMP_Text symbolText;
    public TMP_Text damageText;
    public TMP_Text healthText;
    public Image damageImage;

    Vector2 currentPosition;
    Transform currentParent;

    public GameObject infoPanelGO;

    public bool isEnemy;

    [Header("Shake Properties")]
    [Tooltip("Shake intensity. Higher value = more intense shake.")] 
    public float shakeMagnitude;
    [Tooltip("Shake duration in seconds. How long the shake should last.")]
    public float shakeDuration;
    [Tooltip("Scale of the perlin noise. Changes the speed of shaking.")]
    public float perlinScale;

    [Header("Pulse Properties")]
    [Tooltip("Pulse Size. Higher value = more intense pulse.")]
    public float pulseSize;
    [Tooltip("Pulse duration in seconds. How long the pulse should last.")]
    public float pulseDuration;
    [Tooltip("Pulse speed. How fast the pulse should be.")]
    public float pulseSpeed;

    /// <summary>
    /// 
    /// </summary>
    public void UpdateGraphics(Vector2 newPosition, Transform newParent)
    {
        SetPosition(newPosition);
        SetParent(newParent);
        SetSiblingIndex();
        UpdatePropertiesGraphics();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdatePropertiesGraphics()
    {
        var unitProperties = GetComponent<UnitProperties>();

        if(unitProperties.isCompound)
        {
            //Debug.Log("Compound images set");
            unitSprite = UnitSpriteHolder.instance.GetSpriteFromName(unitProperties.chemicalNameForSprite + " Compound");
            maskSprite = UnitSpriteHolder.instance.GetSpriteFromName(unitProperties.chemicalNameForSprite + " Compound Mask");
            
            compoundMaskImage.sprite = maskSprite;
            maskColor = HexToColor("#" + unitProperties.GetHexColor());
            compoundMaskImage.color = maskColor;

            maskImageGO.SetActive(true);
        }
        else
        {
            //Debug.Log("Ion image set");
            unitSprite = UnitSpriteHolder.instance.GetSpriteFromName(unitProperties.chemicalName);
            maskImageGO.SetActive(false);
        }

        compoundImage.sprite = unitSprite;

        symbolText.text   = unitProperties.GetSymbol();
        damageText.text   = unitProperties.GetStrength().ToString();
        healthText.text   = unitProperties.GetHealth().ToString();

        if (unitProperties.isIon == true) //if this is an ion
        {
            symbolText.text   += "<sup>" + ChargeToString(unitProperties.ionCharge) + "</sup>";
            //damageImage.color = HexToColor("#5782BE"); //blue
            //damageImage.color = HexToColor("#808080"); //dark grey
            damageImage.color = Color.white;
            damageText.text = "<u>" + unitProperties.GetStrength().ToString() + "</u>";
        }
        else if (unitProperties.isCompound == true) //if this is a compound
        {
            //damageImage.color = HexToColor("#FFBE08"); //yellow color
            //damageImage.color = HexToColor("#" + unitProperties.GetHexColor()); //color of unit
            damageImage.color = Color.white;
            damageText.text = "<u>" + unitProperties.GetStrength().ToString() + "</u>";
        }
        else //else, this is an atom
        {
            //damageImage.color = HexToColor("#808080"); //dark grey
            damageImage.color = Color.white;
        }

        gameObject.name = unitProperties.GetChemicalName() + " Unit";

        if(isEnemy)
        {
            var localScaleX = -1;
            var localScaleY = compoundImage.gameObject.GetComponent<RectTransform>().localScale.y;
            var localScaleZ = compoundImage.gameObject.GetComponent<RectTransform>().localScale.z;
            compoundImage.gameObject.GetComponent<RectTransform>().localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
            compoundMaskImage.gameObject.GetComponent<RectTransform>().localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
        }

        SetInfoPanelText();
    }

    string ChargeToString(int charge)
    {
        string sign = "+";
        if(charge < 0) { sign = "-"; }

        string newString = Mathf.Abs(charge).ToString() + sign;

        return newString;
    }

    public void SetPosition(Vector2 newPosition)
    {
        Vector3 position = new Vector3(newPosition.x, newPosition.y, 0);
        GetComponent<RectTransform>().position = newPosition;
    }
    public void SetParent(Transform newParent) { GetComponent<RectTransform>().SetParent(newParent); }
    public void SetSiblingIndex() { transform.SetSiblingIndex(Slot.siblingIndex); }
    

    void SetInfoPanelText()
    {
        var chemicalName = GetComponent<UnitProperties>().level + " " + GetComponent<UnitProperties>().chemicalName;

        var sellProfit = GetComponent<UnitProperties>().sellProfit;

        var sign = "+"; if (GetComponent<UnitProperties>().charge < 0) { sign = "-"; } if(GetComponent<UnitProperties>().charge == 0) { sign = ""; }

        int charge = GetComponent<UnitProperties>().charge;

        var unitType = "";

        if (GetComponent<UnitProperties>().isIon == true)
        {
            //chemicalName += " " + Mathf.Abs(GetComponent<UnitProperties>().ionCharge) + sign;
            unitType += "Ion";
            if(charge > 0) { unitType += " (Kation)"; }
            else           { unitType += " (Anion)"; }
        }
        else if (GetComponent<UnitProperties>().isCompound == true)
        {
            unitType += "Ionforbindelse (Salt)";
        }
        else
        {
            unitType += "Atom";
        }

        byte worth = 3;
        if(GetComponent<UnitProperties>().isIon == true || GetComponent<UnitProperties>().isCompound == true)
        {
            worth = sellProfit;
        }

        bool isCompound = GetComponent<UnitProperties>().isCompound;
        Color hexColor = HexToColor("#" + GetComponent<UnitProperties>().GetHexColor());

        #region TIER
        byte tier = GetComponent<UnitProperties>().tier;
        #endregion

        infoPanelGO.GetComponent<InfoPanelControl>().SetPanel(chemicalName, tier, unitType, charge, worth, unitSprite, maskSprite, hexColor, isCompound);
    }

    public Color HexToColor(string hex)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public void Shake(){
        StartCoroutine(ShakeCoroutine(shakeMagnitude, shakeDuration, perlinScale));
    }

    IEnumerator ShakeCoroutine(float shakeMagnitide, float shakeDuration, float perlinScale){
        float elapsed = 0f;     // time elapsed since starting coroutine

        Vector2 originalPos = GetComponent<RectTransform>().position;   // save original pos of object

        while(elapsed < shakeDuration){
            #region RANDOM RANGE (OLD)
            //float x = transform.position.x + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitide;
            //float y = transform.position.y + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitide;

            //GetComponent<RectTransform>().position = new Vector2(x, y);
            #endregion

            #region PERLIN NOISE (OLD)
            //float x = originalPos.x + (Mathf.PerlinNoise(Time.time * perlinScale, 0) * 2 - 1) * shakeMagnitide;
            //float y = originalPos.y + (Mathf.PerlinNoise(0, Time.time * perlinScale) * 2 - 1) * shakeMagnitide;

            //GetComponent<RectTransform>().position = new Vector2(x, y);
            #endregion

            #region PERLIN NOISE WITH RANDOM SEED
            System.Random random = new System.Random();

            float x = originalPos.x + (Mathf.PerlinNoise((float)random.NextDouble() * perlinScale, 0) * 2 - 1) * shakeMagnitide;
            float y = originalPos.y + (Mathf.PerlinNoise(0, (float)random.NextDouble() * perlinScale) * 2 - 1) * shakeMagnitide;

            GetComponent<RectTransform>().position = new Vector2(x, y);
            #endregion

            elapsed += Time.deltaTime;

            yield return null;
        }

        GetComponent<RectTransform>().position = originalPos;

        //Debug.Log("Shake coroutine finished!");
    }

    public void Pulse(){
        StartCoroutine(PulseCoroutine(pulseSize, pulseDuration, pulseSpeed));
    }

    IEnumerator PulseCoroutine(float pulseSize, float pulseDuration, float pulseSpeed){
        float elapsedTime = 0f;

        // save the original scale of the object
        Vector2 originalScale = GetComponent<RectTransform>().localScale;

        while(elapsedTime < pulseDuration){
            // calculate the scale of the pulse based on the time and speed
            float pulseScale = 1 + Mathf.Sin(elapsedTime * pulseSpeed) * pulseSize;
            
            // clamp the scale to be between 0 and 1 so it doesn't go become smaller
            pulseScale = Mathf.Max(pulseScale, 0, 1);

            // set the scale of the object
            GetComponent<RectTransform>().localScale = originalScale * pulseScale;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // reset the scale of the object
        GetComponent<RectTransform>().localScale = originalScale;

        //Debug.Log("Pulse coroutine finished!");
    }
}