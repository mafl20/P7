using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamControl : MonoBehaviour
{
    public static TeamControl instance;

    // put elements in inspector in array, swap them around and make this script print the index names to see if it has swapped
    public List<Slot> slots;
    public bool isTeamFull;
    public int filledSlots;

    public enum ChangeType
    {
        Swap,
        Requeue
    }

    public ChangeType changeType;

    [SerializeField] Image[] waterImages;
    [SerializeField] Color defaultWaterColor;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateWaterColor()
    {
        int i = 0;
        foreach (Slot slot in slots)
        {
            if (slot.type == Slot.SlotType.Team)
            {
                string hexColor;
                Color waterColor;

                if (slot.unitGO == null)
                {
                    waterColor = defaultWaterColor;
                }
                else
                {
                    hexColor = slot.unitGO.GetComponent<UnitProperties>().GetHexColor();
                    //Debug.Log("Hex Color of Slot " + i + ": " + hexColor);
                    waterColor = slot.unitGO.GetComponent<UnitGraphics>().HexToColor("#" + hexColor);
                    //Debug.Log("Water color: " + waterColor);
                }

                waterColor.a = 0.8f;
                waterImages[i].color = waterColor;
            }

            i++;
        }
    }

    public void Swap(int a, int b)
    {
        #region Temporary variables
        var slotA          = slots[a];
        var slotB          = slots[b];
        var unitGO_A       = slots[a].unitGO;
        var unitGO_B       = slots[b].unitGO;
        var slotPositionA  = slotA.unitSpot.position;
        var slotTransformA = slotA.unitSpot;
        var slotPositionB  = slotB.unitSpot.position;
        var slotTransformB = slotB.unitSpot;
        #endregion

        if (unitGO_B != null && Combiner.instance.CanCombine(unitGO_A, unitGO_B) == true) //the units can combine
        {
            Combiner.instance.Combine(unitGO_A, unitGO_B);
            return;
        }

        Debug.Log("Will just swap these two...");
        slots[a].unitGO = unitGO_B; //replacing the 1st element with the 2nd element
        slots[b].unitGO = unitGO_A; //replacing the 2nd element with the 1st element

        #region Updating unit graphics
        if (slots[a].unitGO != null)
        {
            slots[a].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slotPositionA, slotTransformA); //update graphics of the element
        }

        if (slots[b].unitGO != null)
        {
            slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slotPositionB, slotTransformB); //update graphics of the element
        }
        #endregion

        #region Sounds
        AudioManager.instance.Play("SwappedElements");
        #endregion

        EventControl.instance.moveWithinTeamEvent.Invoke();
    }

    public void Requeue(int a, int b)
    {
        #region Temporary variables
        var slotA = slots[a];
        var slotB = slots[b];
        var unitGO_A = slots[a].unitGO;
        var unitGO_B = slots[b].unitGO;
        var slotPositionA = slotA.unitSpot.position;
        var slotTransformA = slotA.unitSpot.transform;
        var slotPositionB = slotB.unitSpot.position;
        var slotTransformB = slotB.unitSpot.transform;
        #endregion

        if (unitGO_B != null && Combiner.instance.CanCombine(unitGO_A, unitGO_B) == true) //the units can combine
        {
            //Debug.Log("These can combine!");
            Combiner.instance.Combine(unitGO_A, unitGO_B);
            return;
        }
        else if(unitGO_B == null || Mathf.Abs(a - b) == 1) //if the new slot for the element is already empty OR if the two slots are next to each other (difference in their index is 1)
        {
            Swap(a, b); //this is just a simple swap
            return;
        }
        else //else (the new slot is occupied)
        {
            //Debug.Log("The newly selected slot is occupied. Will try and requeue them...");
            slots[a].unitGO = null; //remove the first element (don't worry, its stored in 'elementGO_A' ;) )

            //> find where there is room in the team
            #region Advanced requeuing (only moves from hole to second selected slot)
            
            for (int i = b; i < slots.Count; i++) //iterate through all the slots
            {
                //Debug.Log("Looking to requeue slot " + i);
                if (slots[i].unitGO == null) //if the current slot does not have an element, there is a "hole" in the team that can be filled
                {
                    //Debug.Log("Slot " + i + ": is seen as being empty");
                    for (int j = i; j > b; j--) //iterate through all slots from the current slot (i) to the new slot (b)
                    {
                        slots[j].unitGO = slots[j - 1].unitGO; //move elements from its slot to the slot left of it (moving further back)
                        #region Updating element graphics
                        if (slots[j].unitGO != null)
                        {
                            slots[j].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[j].unitSpot.position, slots[j].unitSpot.transform); //update graphics of the element
                        }
                        #endregion
                    }

                    slots[b].unitGO = unitGO_A; //we are done
                    #region Updating element graphics
                    if (slots[b].unitGO != null)
                    {
                        slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[b].unitSpot.position, slots[b].unitSpot.transform); //update graphics of the element
                    }
                    #endregion

                    SelectionController.instance.ResetSelection();
                    return; //and we return, and don't run the code below
                }
            }
            #endregion
        }

        #region Standard requeuing (moves all element from a to b)
        //Debug.Log("A: " + a + ", B: " + b + " | A > B = " + (a > b));

        if (a > b) //if the selected element is further to the left (further back) than the new slot
        {
            //Debug.Log("Firstly selected slot is further BACK the the lastly selected slot...");
            for (int i = a; i > b; i--) //iterate from the old slot, down to the new slot
            {
                slots[i].unitGO = slots[i - 1].unitGO; //move element from right to left (move further back)
                if (slots[i].unitGO != null)
                {
                    slots[i].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[i].unitSpot.position, slots[i].unitSpot); //update graphics of the element
                }//
            }
        }
        else //else, if the selected element is ahead (to the right) of the new slot
        {
            //Debug.Log("Firstly selected slot is further AHEAD the the lastly selected slot...");
            for (int i = a; i < b; i++) //iterate from the old slot, up to the new slot
            {
                slots[i].unitGO = slots[i + 1].unitGO; //move element from left to right (move further ahead)
                if (slots[i].unitGO != null)
                {
                    slots[i].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[i].unitSpot.position, slots[i].unitSpot); //update graphics of the element
                }
            }
        }
        #endregion

        slots[b].unitGO = unitGO_A;

        slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[b].unitSpot.position, slots[b].unitSpot); //update graphics of the element

        EventControl.instance.moveWithinTeamEvent.Invoke();
        SelectionController.instance.ResetSelection();
    }

    public void BuyRequeue(int b)
    {
        int a = GetClosestEmptySlot(b); //a = closest empty index

        if (a > b)
        {
            for (int j = a; j > b; j--) //iterate through all slots from the current slot (i) to the new slot (b)
            {
                slots[j].unitGO = slots[j - 1].unitGO; //move elements from its slot to the slot left of it (moving further back)
                if (slots[j].unitGO != null)
                {
                    slots[j].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[j].unitSpot.position, slots[j].unitSpot); //update graphics of the element
                }
            }
        }
        else
        {
            for (int j = a; j < b; j++) //iterate through all slots from the current slot (i) to the new slot (b)
            {
                slots[j].unitGO = slots[j + 1].unitGO; //move elements from its slot to the slot left of it (moving further back)
                if (slots[j].unitGO != null)
                {
                    slots[j].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(slots[j].unitSpot.position, slots[j].unitSpot); //update graphics of the element
                }
            }
        }

        SelectionController.instance.ResetSelection();
    }

    public void SpawnSpecificElementAtIndex(byte numberOfProtons, byte index)
    {
        GameObject newUnitGO = UnitCreator.instance.CreateNewElementUnit(numberOfProtons, slots[index].unitSpot.position, slots[index].unitSpot.transform);
        newUnitGO.GetComponent<UnitProperties>().isIon = true;
        newUnitGO.GetComponent<UnitProperties>().sellProfit = 1;
        newUnitGO.GetComponent<UnitGraphics>().UpdatePropertiesGraphics();
        slots[index].unitGO = newUnitGO; //assigning the newly generate element game object to the slot
    }

    public List<int> GetEmptySlotIndices()
    {
        List<int> emptySlots = new List<int>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].unitGO == null)
            {
                emptySlots.Add(i);
                //Debug.Log("index " + i + " in the team is empty");
            }
        }

        return emptySlots;
    }

    public int GetClosestEmptySlot(int index)
    {
        int closestIndex = index;
        int distance = slots.Count;

        List<int> emptySlots = GetEmptySlotIndices();

        for (int i = 0; i < emptySlots.Count; i++)
        {
            if (distance > Mathf.Abs(emptySlots[i] - index))
            {
                //Debug.Log("Distance was updated since " + distance + " > " + Mathf.Abs(emptySlots[i] - index));
                distance = Mathf.Abs(emptySlots[i] - index);
                closestIndex = emptySlots[i];
            }
            //Debug.Log("Distance: " + distance);

            //eventually implement bias towards distances to the left(lower in queue)
        }

        return closestIndex;
    }

    public void UpdateFullCheck()
    {
        UpdateFilledSlots();
        EvaluateIsFull();
    }

    public void UpdateFilledSlots()
    {
        filledSlots = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].unitGO != null)
            {
                filledSlots++;
            }
        }
    }

    public void EvaluateIsFull()
    {
        isTeamFull = filledSlots == slots.Count;
    }

    public List<string> GetTeam()
    {
        List<string> team = new List<string>();

        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].unitGO == null)
            {
                team.Add("          "); //10 spaces = 5 data points + 5 spaces inbetween (see below)
            }
            else
            {
                //team.Add(slots[i].GetUnitChemicalName());
                var unitName = slots[i].GetUnitCompoundsChemicalName()[0] + " " +
                               slots[i].GetUnitCompoundsChemicalName()[1] + " " +
                               slots[i].GetUnitCompoundsChemicalName()[2] + " " +
                               slots[i].GetUnitCompoundsChemicalName()[3] + " " +
                               slots[i].GetUnitCompoundsChemicalName()[4] + " ";
                team.Add(unitName);
            }
        }

        return team;
    }

    public bool CheckHasTeam()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].unitGO != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Resets the team so that the slots hold no elements.
    /// </summary>
    public void ResetTeam()
    {
        for (int i = 0;i < slots.Count;i++)
        {
            slots[i].DeleteUnitGO();
        }

        UpdateFullCheck();
    }
}