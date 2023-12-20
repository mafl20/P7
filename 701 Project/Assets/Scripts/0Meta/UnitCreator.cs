using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitCreator : MonoBehaviour
{
    public static UnitCreator instance;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject elementPrefab;

    public List<byte> allElements;
    public List<byte> activeElements;
    public byte[] tier1;
    public byte[] tier2;
    public byte[] tier3;
    public byte[] tier4;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.K)) { CreateNewUnit("1 Aluminum 1 Bromide 3", GetComponent<RectTransform>().position, transform); }
    }

    /*public GameObject CreateBattleUnit(string chemicalName, Vector2 newPosition, Transform newParent)
    {
        string[] compoundInfo = chemicalName.Split(' ');



        var newBattleUnit = Instantiate(battleUnitPrefab, transform);

        newBattleUnit.GetComponent<BattleUnit>().level = byte.Parse(compoundInfo[0]);
        newBattleUnit.GetComponent<BattleUnit>().damage = short.Parse(compoundInfo[])

        //newBattleUnit.GetComponent<BattleUnit>().elementACount = byte.Parse(compoundInfo[2]);
        //if (!compoundInfo[3].Equals(""))
        //    newBattleUnit.GetComponent<BattleUnit>().elementBCount = byte.Parse(compoundInfo[4]);

        //newUnit.GetComponent<UnitProperties>().UpdateUnitProperties();
        //newUnit.GetComponent<UnitGraphics>().UpdateGraphics(newPosition, newParent);

        //return newUnit;
    }*/

    string[] CalculateStats(string[] compoundInfo)
    {
        string[] stats = new string[compoundInfo.Length];

        var level = compoundInfo[0];
        var chemicalName_A  = compoundInfo[1];
        var chemicalCount_A = compoundInfo[2];
        var chemicalName_B  = compoundInfo[3];
        var chemicalCount_B = compoundInfo[4];

        #region SYMBOL
        var symbol = "";

        if(byte.Parse(level) > 1)
        {
            symbol += level;
        }

        symbol += chemicalName_A;

        if(byte.Parse(chemicalCount_A) > 1)
        {
            symbol += "<sub>" + chemicalCount_A + "</sub>";
        }

        if (chemicalName_B != " ")
        {
            symbol += chemicalName_B;

            if(byte.Parse(chemicalName_B) > 1)
            {
                symbol += "<sub>" + chemicalCount_B + "</sub>";
            }
        }
        #endregion



        stats[0] = level;
        stats[1] = symbol;

        return stats;
    }

    public GameObject CreateNewUnit(string chemicalName, Vector2 newPosition, Transform newParent)
    {
        //we split the chemical name by every space character
        string[] compoundInfo = chemicalName.Split(' ');

        //we create a new unit game object based on the unit prefab
        var newUnit = Instantiate(unitPrefab, transform);

        #region Element A
        //we create a new element game object based on the element prefab, with the newly create unit as parent
        var newElementA = Instantiate(elementPrefab, newUnit.transform);
        //we set the chemical name of the element based off of the second element in the compoundInfo array
        newElementA.GetComponent<ElementProperties>().SetChemicalName(compoundInfo[1]); //see Slot.cs -> GetUnitCompoundsChemicalName()-method
        //now that we have the name of the element, we set all of its properties based on the name
        newElementA.GetComponent<ElementProperties>().UpdatePropertiesFromChemicalName();
        //we set the element as the first child of the unit game object
        newElementA.transform.SetSiblingIndex(0);
        //we set the element as the element A game object of the unit game object
        newUnit.GetComponent<UnitProperties>().elementAGO = newElementA;
        #endregion

        #region Element B
        if (!compoundInfo[3].Equals("")) //if the unit is to be a compound, i.e., if the fourth element in the compoundInfo array is not " "
        {
            //we do the same as above, but for the second element
            var newElementB = Instantiate(elementPrefab, newUnit.transform);
            newElementB.GetComponent<ElementProperties>().SetChemicalName(compoundInfo[3]); //see Slot.cs -> GetUnitCompoundsChemicalName()-method
            newElementB.GetComponent<ElementProperties>().UpdatePropertiesFromChemicalName();
            newElementB.transform.SetSiblingIndex(1);
            newUnit.GetComponent<UnitProperties>().elementBGO = newElementB;
        }
        #endregion

        //we set the level of the unit based on the first element in the compoundInfo array
        newUnit.GetComponent<UnitProperties>().level = byte.Parse(compoundInfo[0]);
        //we set the amount of element A based on the third element in the compoundInfo array
        newUnit.GetComponent<UnitProperties>().elementACount = byte.Parse(compoundInfo[2]);
        if (!compoundInfo[3].Equals("")) //if the unit is to be a compound, i.e., if the fourth element in the compoundInfo array is not " "
        {
            //we set the amount of element B based on the third element in the compoundInfo array
            newUnit.GetComponent<UnitProperties>().elementBCount = byte.Parse(compoundInfo[4]);
        }

        //finally we update the properties and graphics of the unit
        newUnit.GetComponent<UnitProperties>().UpdateUnitProperties();
        newUnit.GetComponent<UnitGraphics>().UpdateGraphics(newPosition, newParent);

        return newUnit;
    }

    public GameObject CreateNewElementUnit(byte numberOfProtons, Vector2 newPosition, Transform newParent)
    {
        //we create a new unit game object based on the unit prefab
        var newUnit = Instantiate(unitPrefab, transform);
        //we create a new element game object based on the element prefab, with the newly create unit as parent
        var newElement = Instantiate(elementPrefab, newUnit.transform);

        //we set the number of protons of the element based on the specified number of protons
        newElement.GetComponent<ElementProperties>().SetProtons(numberOfProtons);
        //we update the properties of the element based on the number of protons
        newElement.GetComponent<ElementProperties>().UpdatePropertiesFromProtons();
        //we set the element as the first child of the unit game object
        newElement.transform.SetSiblingIndex(0);

        //we set the element as the element A game object of the unit game object
        newUnit.GetComponent<UnitProperties>().elementAGO = newElement;
        //we set the amount of element A to 1
        newUnit.GetComponent<UnitProperties>().elementACount = 1;
        //we set level of the unit to 1
        newUnit.GetComponent<UnitProperties>().level = 1;

        //finally we update the properties and graphics of the unit
        newUnit.GetComponent<UnitProperties>().UpdateUnitProperties();
        newUnit.GetComponent<UnitGraphics>().UpdateGraphics(newPosition, newParent);

        return newUnit;
    }

    /// <summary>
    /// 
    /// </summary>
    public GameObject CreateRandomElementUnit(Vector2 newPosition, Transform newParent)
    {
        var numberOfProtons = PickRandomNumberOfProtonsFromAllElements();

        return CreateNewElementUnit(numberOfProtons, newPosition, newParent);
    }

    /// <summary>
    /// 
    /// </summary>
    public GameObject CreateElementUnitFromTier(byte tier, Vector2 newPosition, Transform newParent)
    {
        var numberOfProtons = PickRandomNumberOfProtonsFromActiveElements();

        return CreateNewElementUnit(numberOfProtons, newPosition, newParent);
    }

    byte PickRandomNumberOfProtonsFromAllElements()
    {
        byte randomIndex = (byte)Random.Range(0, allElements.Count);
        byte randomNumberOfProtons = allElements[randomIndex];

        return randomNumberOfProtons;
    }

    public void UpdateActiveElements(byte tier)
    {
        switch (tier)
        {
            case 1: activeElements.AddRange(tier1); break;
            case 2: activeElements.AddRange(tier2); break;
            case 3: activeElements.AddRange(tier3); break;
            case 4: activeElements.AddRange(tier4); break;
            default: break;
        }
    }

    public void SetActiveElements(byte tier)
    {
        switch (tier)
        {
            case 1: activeElements = tier1.ToList(); break;
            case 2: activeElements = tier2.ToList(); break;
            case 3: activeElements = tier3.ToList(); break;
            case 4: activeElements = tier4.ToList(); break;
            default: break;
        }
    }

    public void ResetActiveElements()
    {
        activeElements.Clear();
    }

    byte PickRandomNumberOfProtonsFromActiveElements()
    {
        byte randomIndex = (byte)Random.Range(0, activeElements.Count);
        //Debug.Log("Random index: " + randomIndex);
        byte randomNumberOfProtons = activeElements[randomIndex];
        //Debug.Log("Random number of protons: " + randomNumberOfProtons); 

        return randomNumberOfProtons;
    }
}