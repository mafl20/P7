using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TutorialPopUp
{
    public enum Type
    {
        Button,
        Select,
        Buy,
        Sell,
        Freeze,
        Unfreeze,
        Roll,
        Combine,
        MoveWithinTeam
    }

    public Type type;
    public GameObject popUpGO;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public List<GameObject> annoyingStaticStuff;

    public GameObject popUpGroup;
    [SerializeField] GameObject shop;
    public byte popUpIndex;
    public List<TutorialPopUp> popUps;

    #region Bools
    [Header("Bools")]
    public bool isTutorialActive;
    public bool hasSelected;
    public bool hasBought;
    public bool hasSold;
    public bool hasFrozen;
    public bool hasUnfrozen;
    public bool hasRolled;
    public bool hasCombined;
    public bool hasMovedWithinTeam;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OnlyActivateFirstPopUp();
    }

    public void SpawnFirstShop() { SpawnInShopSpecificSlot(3, 2); } //lithium in the last slot of the shop (furthest to the left)
    public void SpawnSecondShop() { SpawnInShopSpecificSlot(9, 2); } //fluor in the last slot of the shop (furthest to the left)
    public void SpawnThirdShop()
    {
        byte[] numberOfProtonsArray = new byte[3] { 9, 9, 8 }; //1 oxygen, 2 fluor
        SpawnInShopForAll(numberOfProtonsArray);
    }
    public void SpawnFourthShop()
    {
        byte[] numberOfProtonsArray = new byte[3] { 3, 3, 0 }; //nothing, and 2 lithium
        SpawnInShopForAll(numberOfProtonsArray);
    }

    public void SpawnNitrogenOnTeam()
    {
        byte numberOfProtons = 7;
        byte index = 2;
        SpawnElementAtSlotInTeam(numberOfProtons, index);
    }

    public void SpawnFifthShop()
    {
        byte[] numberOfProtonsArray = new byte[3] { 9, 8, 7 }; //1 nitrogen, 1 oxygen and 1 lithium in the shop
        SpawnInShopForAll(numberOfProtonsArray);
    }

    public void SpawnSixthShop()
    {
        byte[] numberOfProtonsArray = new byte[3] { 7, 3, 0 }; //nothing, lithium and nitrogen in the shop
        SpawnInShopForAll(numberOfProtonsArray);
    }

    public void SpawnInShopSpecificSlot(byte numberOfProtons, byte index)
    {
        var parent = ShopControl.instance.slots[index].GetComponent<Slot>().unitSpot.transform;
        var position = parent.gameObject.GetComponent<RectTransform>().position;
        GameObject newUnit = UnitCreator.instance.CreateNewElementUnit(numberOfProtons, position, parent);
        ShopControl.instance.slots[index].unitGO = newUnit;
    }
    
    public void SpawnInShopForAll(byte[] numberOfProtonsArray)
    {
        //for (int i = 0; i < numberOfProtonsArray.Length; i++)
        //{
        //    SpawnInShopSpecificSlot(numberOfProtonsArray[i], (byte)(2 - i));
        //}
        ShopControl.instance.RollSpecificElements(numberOfProtonsArray);
    }

    public void SpawnElementAtSlotInTeam(byte numberOfProtons, byte index)
    {
        TeamControl.instance.SpawnSpecificElementAtIndex(numberOfProtons, index);
    }

    public void NextPopUp()
    {
        #region Resetting bools
        hasSelected = false;
        hasBought = false;
        hasSold = false;
        hasFrozen = false;
        hasUnfrozen = false;
        hasRolled = false;
        hasCombined = false;
        hasMovedWithinTeam = false;
        #endregion

        #region Incrementing
        popUpIndex++; //we increment the index of tutorial pop ups
        byte i = 0; //used as an index in the foreach-loop

        foreach(TutorialPopUp popUp in popUps) //for every pop up that we have
        {
            if(popUp.popUpGO == null) { i++; continue; }
            
            if(i == popUpIndex) //only activate the one that is associated with the current pop up index
            {
                //Debug.Log("Active popup: " + popUp.popUpGO.name);
                popUp.popUpGO.SetActive(true);
            }
            else
            {
                popUp.popUpGO.SetActive(false);
            }
            i++; //increment the index value
        }
        #endregion
    }

    public void CheckIfConditionForNextIsMet()
    {
        if (popUps[popUpIndex].type == TutorialPopUp.Type.Select && hasSelected == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Buy && hasBought == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Sell && hasSold == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Freeze && hasFrozen == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Unfreeze && hasUnfrozen == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Roll && hasRolled == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.Combine && hasCombined == true) { NextPopUp(); }
        else if (popUps[popUpIndex].type == TutorialPopUp.Type.MoveWithinTeam && hasMovedWithinTeam == true) { NextPopUp(); }
    }

    public void OnlyActivateFirstPopUp()
    {
        isTutorialActive = true; // Set isTutorialActive to true

        popUpIndex = 0; // Set popUpIndex to 0
        
        //Debug.Log("Only activating first pop up...");
        var childCount = transform.childCount;
        //popUpGroup.SetActive(true); // Show tutorial if it is the first time playing
        for (int i = 0; i < childCount; i++)
        {
            if(transform.GetChild(i).gameObject == null) { continue; }
            transform.GetChild(i).gameObject.SetActive(false);
            //Debug.Log("Deactivated " + transform.GetChild(i).gameObject.name);
        }

        popUps[popUpIndex].popUpGO.SetActive(true);
    }

    public void DestroyAnnoyingStaticStuff()
    {
        for (int i = 0; i < annoyingStaticStuff.Count; i++)
        {
            Destroy(annoyingStaticStuff[i]);
        }
    }

    public void DestroyIonicBondOverlay()
    {
        //New Ionic Bond Overlay(Clone)
        Destroy(FindObjectOfType<NewIonicBondOverlay>().gameObject);
    }

    public void SetTutorialActive(bool value)
    {
        isTutorialActive = value;
    }
}