using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ShopControl : MonoBehaviour
{
    public static ShopControl instance;

    public List<Slot> slots;
    public List<bool> frozenIndeces;
    [SerializeField] GameObject sellButtonGO, freezeButtonGO; //GO = GameObject
    [SerializeField] GameObject sellButtonNotificationGO;
    [SerializeField] TMP_Text sellButtonNotificationSellText;
    [SerializeField] TMP_Text sellButtonText;
    [SerializeField] GameObject freezeButtonNotificationGO;
    [SerializeField] TMP_Text freezeButtonNotificationFreezeText;
    [SerializeField]
    public byte highestAvailableTier;
    public byte roundToActivateTier1;
    public byte roundToActivateTier2;
    public byte roundToActivateTier3;
    public byte roundToActivateTier4;
     
    #region Chemistry Coins (CC)
    [Header("Chemistry Coins (CC)")]
    [Tooltip("The number of CC to start with when Setup begins.")]
    public int startCC;
    public int numberOfCC; //number of coins that can be spend
    public TMP_Text coinText; // UI Text element displaying the player's coin count
    public int rollCost;
    public int itemCost; // Cost of each item in the shop
    #endregion

    #region Tiers
    [Header("Tiers")]
    public GameObject tierUnlockOverlayGO;
    [SerializeField, TextArea] string tierString_upper, tierString_middle, tierString_lower;
    [SerializeField] TMP_Text tierText_upper, tierText_middle, tierText_lower;
    #endregion

    #region Roll Shake
    [Header("Roll Shake")]
    public float shakeMagnitude;
    public float shakeDuration;
    public float perlinScale;
    #endregion

    private void Awake()
    {
        instance = this;

        frozenIndeces = new List<bool>();
        for (int i = 0; i < slots.Count; i++)
        {
            frozenIndeces.Add(false);
        }
    }

    private void Update()
    {
        if      (Input.GetKeyDown(KeyCode.Alpha9)) { numberOfCC += 10; UpdateCoinUI(); }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) { if (numberOfCC > 0) { numberOfCC -= 1; UpdateCoinUI(); } }
    }

    public void RollSpecificElements(byte[] numberOfProtonsArray)
    {
        //numberOfCC -= rollCost; //decrement available CC
        //UpdateCoinUI(); //update the coin ui

        #region Sounds
        AudioManager.instance.Play("RollNewElements");
        #endregion

        EventControl.instance.rollEvent.Invoke();

        byte i = 0;
        foreach (Slot slot in slots) //for every slot in the shop
        {
            if (slot.GetIsFrozen() == true) { continue; } //if the slot is frozen, we want ignore it and go to the next iteration of the foreach loop
            if (slot.unitGO != null)  //if the slot has an unit in it
            {
                slot.DeleteUnitGO(); //delete the element gameobject that the slot is currently holding
            }

            //GameObject newUnitGO = UnitCreator.instance.CreateRandomElementUnit(slot.gameObject.GetComponent<RectTransform>().position, slot.transform);
            //GameObject newUnitGO = UnitCreator.instance.CreateElementUnitFromTier(highestAvailableTier, slot.unitSpot.position, slot.unitSpot.transform);
            GameObject newUnitGO = UnitCreator.instance.CreateNewElementUnit(numberOfProtonsArray[i], slot.unitSpot.position, slot.unitSpot.transform);
            slot.unitGO = newUnitGO; //assigning the newly generate element game object to the slot

            #region Shake
            //Debug.Log("Active? " + newUnitGO.activeInHierarchy);
            if (newUnitGO.activeInHierarchy)
            {
                newUnitGO.GetComponent<UnitGraphics>().Shake();
            }
            #endregion

            i++;
        }

        SelectionController.instance.ResetSelection();
    }

    /// <summary>
    /// Generates new element GameObjects in the shop. If requriesPayment is true, the roll will decrement the available amount of Chemistry Coin (CC) by rollCost.
    /// </summary>
    public void Roll(bool requiresPayment)
    {
        if(requiresPayment == true)
        {
            if(numberOfCC >= rollCost) //if we have CC (Chemistry Coin) enough
            {
                numberOfCC -= rollCost; //decrement available CC
                UpdateCoinUI(); //update the coin ui

                #region Sounds
                AudioManager.instance.Play("RollNewElements");
                #endregion

                EventControl.instance.rollEvent.Invoke();
            }
            else //you don't have money enough
            {
                Debug.LogWarning("Not enough coins to roll for elements!"); //debug an error message 

                //TODO: disable the roll button

                SelectionController.instance.ResetSelection(); //reset selection
                return; //DON'T RUN THE REST OF THE ROLL() CODE
            }
        }

        foreach (Slot slot in slots) //for every slot in the shop
        {
            if(slot.GetIsFrozen() == true) { continue; } //if the slot is frozen, we want ignore it and go to the next iteration of the foreach loop
            if (slot.unitGO != null)  //if the slot has an unit in it
            {
                slot.DeleteUnitGO(); //delete the element gameobject that the slot is currently holding
            }

            //GameObject newUnitGO = UnitCreator.instance.CreateRandomElementUnit(slot.gameObject.GetComponent<RectTransform>().position, slot.transform);
            GameObject newUnitGO = UnitCreator.instance.CreateElementUnitFromTier(highestAvailableTier, slot.unitSpot.position, slot.unitSpot.transform);
            slot.unitGO = newUnitGO; //assigning the newly generate element game object to the slot

            #region Shake
            //Debug.Log("Active? " + newUnitGO.activeInHierarchy);
            if(newUnitGO.activeInHierarchy)
            {
                newUnitGO.GetComponent<UnitGraphics>().Shake();
            }
            #endregion
        }

        SelectionController.instance.ResetSelection();
    }

    public void Buy(int a, int b)
    {
        bool successfullyBought = false;

        if (numberOfCC >= itemCost) //if player has enough coins enough
        {
            #region Temporary variables
            var changeType = TeamControl.instance.changeType;
            var unitGO_A = slots[a].unitGO;
            var unitGO_B = TeamControl.instance.slots[b].unitGO;
            var slotA = slots[a];
            var slotB = TeamControl.instance.slots[b];
            var teamSlotPosition = slotB.unitSpot.position;
            var teamSlotTransform = slotB.unitSpot.transform;
            #endregion

            if (unitGO_B != null && Combiner.instance.CanCombine(unitGO_A, unitGO_B) == true) //the units can combine
            {
                successfullyBought = true;

                numberOfCC -= itemCost; //decrement the cost from playerCoins
                UpdateCoinUI(); //update UI

                if(slots[a].isFrozen == true)
                {
                    slots[a].SetIsFrozen(false);
                    SetFreezeIndex(a, false);
                }

                Combiner.instance.Combine(unitGO_A, unitGO_B);
            }
            else if (TeamControl.instance.isTeamFull == true) //if the team is full
            {
                //> debug and exit the method
                Debug.LogWarning("Team is full!");
                return;
            }
            else //else, if the team is not full, BUYING
            {
                #region Sounds
                AudioManager.instance.Play("BoughtElement");
                #endregion

                if (changeType == TeamControl.ChangeType.Swap) //if the change type of the team is >>>'SWAP'<<<
                {
                    if(unitGO_B == null) //if the second slot is empty
                    {
                        successfullyBought = true; //the buy was successful
                        //
                        TeamControl.instance.slots[b].unitGO = unitGO_A; //move in the element from the first slot
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().isIon = true;
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().UpdateUnitProperties();
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(teamSlotPosition, teamSlotTransform); //update graphics of the bought element
                        slots[a].unitGO = null; //reset the slot that the bought element came from
                    }
                    else { Debug.LogWarning("Slot is occupied!"); } //the second slot is not empty, and so it is an illegal move
                }
                else if (TeamControl.instance.changeType == TeamControl.ChangeType.Requeue) //if the change type of the team is >>>'REQUEUE'<<<
                {
                    successfullyBought = true; //the buy was successful

                    if (unitGO_B == null) //if the second slot is empty
                    {
                        TeamControl.instance.slots[b].unitGO = unitGO_A; //move in the element from the first slot
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().isIon = true;
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().UpdateUnitProperties();
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(teamSlotPosition, teamSlotTransform); //update graphics of the bought element
                    }
                    else if (Combiner.instance.CanCombine(unitGO_A, unitGO_B) == true) //the units can combine
                    {
                        Combiner.instance.Combine(unitGO_A, unitGO_B);
                    }
                    else //buying
                    {
                        TeamControl.instance.BuyRequeue(b); //use TeamControl.cs's BuyRequeue()-method, which reorganizes the team, with respect to slot b
                        TeamControl.instance.slots[b].unitGO = unitGO_A; //move in the element from the first slot
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().isIon = true;
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitProperties>().UpdateUnitProperties();
                        TeamControl.instance.slots[b].unitGO.GetComponent<UnitGraphics>().UpdateGraphics(teamSlotPosition, teamSlotTransform); //update graphics of the bought element
                    }

                    slots[a].unitGO = null; //reset the slot that the bought element came from
                }

                if (successfullyBought == true)
                {
                    numberOfCC -= itemCost; //decrement the cost from playerCoins
                    UpdateCoinUI(); //update UI

                    TeamControl.instance.filledSlots++;

                    if (slots[a].isFrozen == true)
                    {
                        slots[a].SetIsFrozen(false);
                        SetFreezeIndex(a, false);
                    }

                    EventControl.instance.buyEvent.Invoke();
                }

                TeamControl.instance.UpdateFullCheck();
            }
        }
        else { Debug.LogWarning("Not enough coins to buy an element!"); } //the user does not have enought money
    }

    public void Sell()
    {
        var sellProfit = SelectionController.instance.slotA.unitGO.GetComponent<UnitProperties>().sellProfit;
        numberOfCC += sellProfit; //increase playerCoins by sellProfit
        UpdateCoinUI(); //update coin UI

        int index = SelectionController.instance.slotA.index;
        TeamControl.instance.slots[index].DeleteUnitGO();

        TeamControl.instance.filledSlots--;
        TeamControl.instance.UpdateFullCheck();
        SelectionController.instance.ResetSelection();

        #region Sounds
        AudioManager.instance.Play("SoldElement");
        #endregion

        EventControl.instance.sellEvent.Invoke();
    }

    public void ShowSellButton() { SetSellButtonActivation(true); }
    public void HideSellButton() { SetSellButtonActivation(false); }
    public void SetSellButtonActivation(bool newBool)
    {
        sellButtonGO.SetActive(newBool);

        if(SelectionController.instance.slotA == null)
        {
            sellButtonText.text = "SÆLG";
        }
        else
        {
            sellButtonText.text = "SÆLG (" + SelectionController.instance.slotA.unitGO.GetComponent<UnitProperties>().sellProfit + ")";
            sellButtonNotificationSellText.text = "+" + SelectionController.instance.slotA.unitGO.GetComponent<UnitProperties>().sellProfit;
        }
    }

    public void ShowFreezeButton()
    {
        SetFreezeButtonActivation(true);
        UpdateFreezeButtonText();
    }
    public void HideFreezeButton() { SetFreezeButtonActivation(false); }
    public void ToggleSlotFreezeState()
    {
        bool isFrozen = SelectionController.instance.slotA.GetIsFrozen();
        SelectionController.instance.slotA.SetIsFrozen(!isFrozen);
        SelectionController.instance.ResetSelection();

        #region Sounds
        AudioManager.instance.Play("FrozeElement");
        #endregion
    }
    public void SetFreezeButtonActivation(bool newBool)
    {
        freezeButtonGO.SetActive(newBool);
    }
    public void UpdateFreezeButtonText()
    {
        bool isFrozen = SelectionController.instance.slotA.GetIsFrozen();
        string newText = "LÅS";
        string newNotificationText = "BEHOLD ATOM EFTER RUL";

        if (isFrozen == true)
        {
            newText = "LÅS OP";
            newNotificationText = "FJERN LÅS";
        }

        SetFreezeButtonText(newText);
        freezeButtonNotificationFreezeText.text = newNotificationText;
    }
    public void SetFreezeButtonText(string newText) { freezeButtonGO.transform.GetChild(0).GetComponent<TMP_Text>().text = newText; }
    public void SetFreezeIndex(int index, bool newBool)
    {
        frozenIndeces[index] = newBool;
        SelectionController.instance.ResetSelection();
    }
    public List<string> GetShop()
    {
        List<string> shop = new List<string>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].unitGO == null)
            {
                shop.Add("");
            }
            else
            {
                shop.Add(slots[i].GetUnitChemicalName());
            }
        }

        return shop;
    }

    public List<bool> GetFrozenIndices()
    {
        return frozenIndeces;
    }

    public void UpdateCoinUI()
    {
        // Update the UI Text element to display the player's current coin count
        coinText.text = numberOfCC.ToString();

        // Update the coin UI for the MoreCoinsLeftPopup 
        GameStateMachine.instance.moreCoinsLeftPopupGO.transform.Find("CC Stat").Find("Text (TMP)").GetComponent<TMP_Text>().text = numberOfCC.ToString();
    }

    public void ResetCoins()
    {
        numberOfCC = startCC;
        UpdateCoinUI();
    }

    public void ToggleTierUnlockOverlay(bool toggle)
    {
        string modified = string.Format(tierString_upper, PlayerInformation.instance.currentRounds);
        tierText_upper.text = modified;

        modified = string.Format(tierString_middle, highestAvailableTier);
        tierText_middle.text = modified;

        modified = string.Format(tierString_lower, highestAvailableTier);
        tierText_lower.text = modified;

        tierUnlockOverlayGO.SetActive(toggle);
    }

    public void ResetShop()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].DeleteUnitGO();
        }
    }
}