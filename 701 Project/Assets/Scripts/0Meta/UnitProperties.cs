using UnityEngine;

public class UnitProperties : MonoBehaviour
{
    #region Constituents
    [Header("Constituents")]
    public GameObject elementAGO;
    public byte elementACount;
    public GameObject elementBGO;
    public byte elementBCount;
    #endregion

    #region Stats
    [Header("Stats")]
    public string symbol;
    public string chemicalName;
    public string chemicalNameForSprite;
    public short damage;
    public short health;
    public byte level;
    public int ionCharge;
    public int charge;
    public bool isIon;
    public bool isCompound;
    public byte tier;
    public string hexColor;
    public byte sellProfit;
    #endregion

    public void UpdateUnitProperties()
    {
        if(elementBGO != null)
        {
            isIon = false;
            isCompound = true;
            //Debug.Log("This is a compound");
        }

        #region SYMBOL
        symbol = "";

        if(level > 1)
        {
            //Debug.Log("Adding level in front...");
            symbol += level + " "; } //if the level is higher than 1, we add the level in front

        symbol += elementAGO.GetComponent<ElementProperties>().GetSymbol(); //we add the symbol of the first element

        if (elementBGO != null) //if we also have a second element
        {
            if (elementACount > 1) { symbol += "<sub>" + elementACount + "</sub>"; } //if we have more than 1 of the first element, add its count to the symbol string

            symbol += elementBGO.GetComponent<ElementProperties>().GetSymbol(); //also, add the symbol of the second element

            if (elementBCount > 1) { symbol += "<sub>" + elementBCount + "</sub>"; } //if we have more than 1 of the second element, add its count to the symbol string
        }
        #endregion

        #region CHEMICAL NAME
        chemicalName = elementAGO.GetComponent<ElementProperties>().GetChemicalName();
        chemicalNameForSprite = chemicalName;
        if (elementBGO != null) { chemicalName += elementBGO.GetComponent<ElementProperties>().GetIonName(); }

        #endregion

        #region DAMAGE
        damage = (short)(level * elementAGO.GetComponent<ElementProperties>().GetAtomDamage());
        if(isIon == true) //if this is an ion
        {
            damage = (short)(level * elementAGO.GetComponent<ElementProperties>().GetIonDamage());
        }
        else if (elementBGO != null) //if this is a compound
        {
            var damageA = elementACount * elementAGO.GetComponent<ElementProperties>().GetIonDamage();
            var damageB = elementBCount * elementBGO.GetComponent<ElementProperties>().GetIonDamage();

            //Debug.Log("Damage A: " + damageA);
            //Debug.Log("Damage B: " + damageB);

            var combinedDamage = damageA + damageB;
            //Debug.Log("Combined Damage: " + combinedDamage);

            damage = (short)(level * combinedDamage);
            //Debug.Log("Damage: " + damage);
        }
        #endregion

        #region HEALTH
        health = (short)(level * elementAGO.GetComponent<ElementProperties>().GetHealth());
        if (elementBGO != null)
        {
            var healthA = elementACount * elementAGO.GetComponent<ElementProperties>().GetHealth();
            var healthB = elementBCount * elementBGO.GetComponent<ElementProperties>().GetHealth();

            //Debug.Log("Health A: " + healthA);
            //Debug.Log("Health B: " + healthB);

            var combinedHealth = healthA + healthB;
            //Debug.Log("Combined Health: " + combinedHealth);

            health = (short)(level * combinedHealth);
            //Debug.Log("Health: " + health);
        }
        #endregion

        #region CHARGE
        if (elementBGO != null) //if we have a compound, the ion charge is zero, resulting in the charge also being zero
        {
            ionCharge = 0;
            charge = 0;
        } 
        else
        {
            ionCharge = elementAGO.GetComponent<ElementProperties>().GetCharge();
            charge = level * ionCharge;
        }
        #endregion

        #region TIER
        tier = elementAGO.GetComponent<ElementProperties>().GetTier();
        if(elementBGO != null)
        {
            if(elementAGO.GetComponent<ElementProperties>().GetTier() <
                elementBGO.GetComponent<ElementProperties>().GetTier())
            {
                tier = elementBGO.GetComponent<ElementProperties>().GetTier();
            }
        }
        #endregion

        #region SELL PROFIT
        sellProfit = level;
        if(isCompound)
        {
            sellProfit = (byte)(level * (elementACount + elementBCount));
        }
        //Debug.Log(gameObject.name + " sell profit: " + sellProfit);
        #endregion

        #region COLOR
        hexColor = elementAGO.GetComponent<ElementProperties>().GetHexColor();
        if (elementBGO != null)
        {
            hexColor = elementBGO.GetComponent<ElementProperties>().GetHexColor();
            //Debug.Log("HexColor: " + elementBGO.GetComponent<ElementProperties>().GetHexColor());
        }
        #endregion
    }

    public string GetChemicalName() { return chemicalName; }
    public string GetSymbol() { return symbol; }
    public short GetStrength() { return damage; }
    public short GetHealth() { return health; }
    public string GetHexColor() { return hexColor; }
    public byte GetSellProfit() { return sellProfit; }

    public void Attack()
    {
        BattleMethods.instance.Attack();
    }
}