using UnityEngine;

public class Combiner : MonoBehaviour
{
    public static Combiner instance;
    [SerializeField] GameObject setupGO;
    [SerializeField] GameObject tutorialGO;
    [SerializeField] GameObject newIonicBondOverlayPF; //PF = prefab
    private void Awake()
    {
        instance = this;
    }

    public bool CanCombine(GameObject a, GameObject b)
    {
        var unitA = a.GetComponent<UnitProperties>();
        var unitB = b.GetComponent<UnitProperties>();

        //> check if they are the same type of unit
        if(unitA.GetChemicalName() == unitB.GetChemicalName() && unitA.level + unitB.level <= 3) //HARD CODED NUMBER!!! - lau :)
        {
            return true;
        }

        //> check if their charge sum to 0
        int chargeA = unitA.charge;
        int chargeB = unitB.charge;

        if(chargeA + chargeB == 0 && chargeA != 0 && chargeB != 0)
        {
            return true;
        }

        return false;
    }

    public void Combine(GameObject a, GameObject b)
    {
        var unitA = a.GetComponent<UnitProperties>(); //cation unit
        var unitB = b.GetComponent<UnitProperties>(); //anion unit
        var higherLevel = 0;

        int chargeA = unitA.charge; //charge of cation
        int chargeB = unitB.charge; //charge of anion

        var cation = a; //cation gameobject
        var anion  = b; //anion gameobject

        if (chargeA < 0)
        {
            cation = b; //cation gameobject
            anion  = a; //anion gameobject
        }

        //> Combine two of the same units
        if (unitA.GetChemicalName().Equals(unitB.GetChemicalName()))
        {
            //Debug.Log("Stacking the same unit...");

            //> settle on what the new level will be
            if (unitA.level >= unitB.level) { higherLevel = unitA.level; }
            else { higherLevel = unitB.level; }
            higherLevel++;

            //> update the unit
            b.GetComponent<UnitProperties>().level = (byte)higherLevel;
            
            if(b.GetComponent<UnitProperties>().isIon == true)
            {
                b.GetComponent<UnitProperties>().elementACount = (byte)higherLevel;
            }

            b.GetComponent<UnitProperties>().UpdateUnitProperties();
            b.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();

            a.transform.parent.parent.GetComponent<Slot>().DeleteUnitGO();
            //Destroy(a);

            #region Sounds
            AudioManager.instance.Play("StackedElements");
            #endregion
        }
        else if(chargeA + chargeB == 0 && chargeA != 0 && chargeB != 0) //> Combine two units with opposite proportional charge
        {
            //Debug.Log("Making ionic bond...");

            //TODO: change this to take into account the level of the compound unit with the lowest level
            b.GetComponent<UnitProperties>().level = 1;

            b.GetComponent<UnitProperties>().elementBGO = anion.GetComponent<UnitProperties>().elementAGO; //oxygen gameobject
            b.GetComponent<UnitProperties>().elementAGO = cation.GetComponent<UnitProperties>().elementAGO; //natrium gameobject

            //> repositioning and reparenting the first selected unit
            var unitAElement = a.transform.GetChild(0);
            unitAElement.SetParent(b.transform);
            unitAElement.transform.SetSiblingIndex(0);
            unitAElement.GetComponent<RectTransform>().position = b.GetComponent<RectTransform>().position;

            var elementACount = cation.GetComponent<UnitProperties>().elementACount;
            var elementBCount = anion.GetComponent<UnitProperties>().elementACount;
            if(elementACount == elementBCount) { b.GetComponent<UnitProperties>().level = elementACount; elementACount = 1; elementBCount = 1; }

            b.GetComponent<UnitProperties>().elementACount = elementACount;
            b.GetComponent<UnitProperties>().elementBCount = elementBCount;

            b.GetComponent<UnitProperties>().UpdateUnitProperties();
            b.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();

            a.transform.parent.parent.GetComponent<Slot>().DeleteUnitGO();
            //Destroy(a);

            //Debug.Log("Ionic bond made: " + b.GetComponent<UnitProperties>().GetChemicalName());
            if(!PlayerInformation.instance.ionicBondsMade.Contains(b.GetComponent<UnitProperties>().GetChemicalName())) //if the player has not made this ionic bond before
            {
                GameObject newIonicBondOverlay;

                if (GameStateMachine.instance.isFirstTimePlaying)
                {
                    newIonicBondOverlay = Instantiate(newIonicBondOverlayPF, tutorialGO.transform);
                }
                else
                {
                    newIonicBondOverlay = Instantiate(newIonicBondOverlayPF, setupGO.transform);
                }

                string chemicalName = b.GetComponent<UnitProperties>().GetChemicalName();
                Sprite unitSprite = b.GetComponent<UnitGraphics>().unitSprite;
                Sprite unitMaskSprite = b.GetComponent<UnitGraphics>().maskSprite;
                Color maskColor = b.GetComponent<UnitGraphics>().maskColor;

                newIonicBondOverlay.GetComponent<NewIonicBondOverlay>().SetOverlay(chemicalName, unitSprite, unitMaskSprite, maskColor);
                PlayerInformation.instance.ionicBondsMade.Add(b.GetComponent<UnitProperties>().GetChemicalName());
            }

            #region Sounds
            AudioManager.instance.Play("CombinedElements");
            #endregion
        }
        else
        {
            Debug.Log("Making else...");
        }

        EventControl.instance.combineEvent.Invoke();
    }
}