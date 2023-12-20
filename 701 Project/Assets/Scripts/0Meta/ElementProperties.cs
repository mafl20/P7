using System;
using System.Globalization;
using UnityEngine;

public class ElementProperties : MonoBehaviour
{
    public string symbol;
    public string chemicalName = " "; 
    public string ionName;

    public float atomicWeightInAMU;

    public byte numberOfProtons;
    public byte numberOfNeutrons;
    public byte numberOfElectrons;
    public byte numberOfElectronShells;

    public int charge;
    public byte tier;

    public float meltingPointInCelsius;
    public float boilingPointInCelsius;

    public string stateAtRoomTemperature;

    public string hexColor;

    /// <summary>
    /// Updates the properties of this object using the number of protons.
    /// </summary>
    public void UpdatePropertiesFromProtons()
    {
        string[] properties = ElementDataReader.instance.GetElementPropertiesFromProtons(numberOfProtons);
        SetProperties(properties);
    }

    /// <summary>
    /// Updates the properties of this object using the chemical name.
    /// </summary>
    public void UpdatePropertiesFromChemicalName()
    {
        if(chemicalName[0] == ' ')
        {
            //Debug.Log("Chemical name is ' ', so no element to be found here...");
            return;
        }

        //Debug.Log("Chemical name of element: " + chemicalName);
        string[] properties = ElementDataReader.instance.GetElementPropertiesFromChemicalName(chemicalName);
        //Debug.Log("Symbol: " + properties[0]);
        SetProperties(properties);
    }

    /// <summary>
    /// Sets the properties of this object.
    /// </summary>
    void SetProperties(string[] properties)
    {
        symbol       = properties[0];
        chemicalName = properties[1];
        ionName      = properties[2];

        float temp = atomicWeightInAMU; //because simply doing '(half)properties[2]' does not work -_-
        float.TryParse(properties[3], NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
        atomicWeightInAMU = temp;

        numberOfProtons        = byte.Parse(properties[4]);
        numberOfNeutrons       = byte.Parse(properties[5]);
        numberOfElectrons      = byte.Parse(properties[6]);
        numberOfElectronShells = byte.Parse(properties[7]);

        charge = int.Parse(properties[8]);
        tier   = byte.Parse(properties[9]);

        float.TryParse(properties[10], NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
        meltingPointInCelsius = temp;
        float.TryParse(properties[11], NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
        boilingPointInCelsius = temp;
        stateAtRoomTemperature = properties[12];

        hexColor = properties[13];

        gameObject.name = GetComponent<ElementProperties>().GetChemicalName() + " Element";
    }

    public string[] GetElementPropertiesFromProtons()
    {
        return ElementDataReader.instance.GetElementPropertiesFromProtons(numberOfProtons);
    }

    public string[] GetElementPropertiesFromChemicalName()
    {
        return ElementDataReader.instance.GetElementPropertiesFromChemicalName(chemicalName);
    }

    public byte GetAtomDamage()
    {
        return (byte)GetElectrons();
    }

    public byte GetIonDamage()
    {
        return (byte)(GetElectrons() - charge);
    }

    public byte GetHealth()
    {
        float weightAsFloat = Mathf.Round((float)GetAtomicWeight());
        byte health = (byte)weightAsFloat;
        return health;
    }
    public string GetSymbol() { return symbol; }
    public void SetChemicalName(string newName) { chemicalName = newName; }
    public string GetChemicalName() { return chemicalName; }
    public string GetIonName() { return ionName; }
    public float GetAtomicWeight() { return atomicWeightInAMU; }
    public void SetProtons(byte numberOfProtons) { this.numberOfProtons = numberOfProtons; }
    public byte GetProtons() { return numberOfProtons; }
    public byte GetNeutrons() { return numberOfNeutrons; }
    public byte GetElectrons() { return numberOfElectrons; }
    public byte GetShells() { return numberOfElectronShells; }
    public int GetCharge() { return charge; }
    public byte GetTier() { return tier; }
    public float GetMeltingPoint() { return meltingPointInCelsius; }
    public float GetBoilingPoint() { return boilingPointInCelsius; }
    public string GetStateAtRoomTemperature() { return stateAtRoomTemperature; }
    public string GetHexColor() { return hexColor; }
}