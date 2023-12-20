using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// ------------------
//      TODO LIST     
// > Add shop elements to the combination list
// > UnitGraphics: shake needs to have random seeds for perlin noise CHECK
// > change so the elements do not shake, but "pulse" instead CHECK
// > Remeber to reset EVERYTHING when the player does something CHECK
// > Fix so the unitGO is not null when shaking
// ------------------ 

public struct SlotPair{
    public Slot firstSlot;
    public Slot secondSlot;
}

public class NudgeManager : MonoBehaviour
{
    public static NudgeManager instance;    

    #region Shake Nudge
    [Header("Shake Nudge")]
    public bool hasStartedNudgeCoroutine = false;
    public float checkDelay;
    //public bool hasListChanged = false;
    public float elapsedTime = 0f;
    public float delayBetweenCombinationShake;
    public bool isShakingCombinations = false;
    #endregion

    #region Handbook Nudge
    [Header("Handbook Nudge")]
    public byte numberOfTimesBeforeTheHandbookPopsUp;
    public bool hasPlayerOpenedHandbookAfterNudge = false;
    public bool isHandbookPopupActive = false;
    public GameObject handbookPopupGO;
    public byte handbookNotOpenedStreak;

    [Header("Pulse Properties")]
    [Tooltip("Pulse Size. Higher value = more intense pulse.")]
    public float pulseSize;
    [Tooltip("Pulse duration in seconds. How long the pulse should last.")]
    public float pulseDuration;
    [Tooltip("Pulse speed. How fast the pulse should be.")]
    public float pulseSpeed;
    #endregion

    List<SlotPair> combinations = new List<SlotPair>();
    SlotPair slotPair;
    int combinationIndex = 0;

    void Awake(){
        instance = this;

        slotPair.firstSlot = null;
        slotPair.secondSlot = null;
    }

    void Update(){
        if (!hasStartedNudgeCoroutine && GameStateMachine.instance.currentState == GameStateMachine.instance.setupState){
            StartCoroutine(CheckIfPlayerIsPlaying());
            hasStartedNudgeCoroutine = true;
        }

        else if (GameStateMachine.instance.currentState != GameStateMachine.instance.setupState){
            hasStartedNudgeCoroutine = false;
            //ResetNudge();
        }
    }

    #region Shake Elements Nudge
    IEnumerator CheckIfPlayerIsPlaying(){
        while (true){
            if (elapsedTime <= checkDelay){
                //Debug.Log("Player is currently playing, do nothing..." + " Elapsed time: " + elapsedTime);
            }

            if (elapsedTime > checkDelay){
                //Debug.Log("Player has not done anything, shaking elements..." + " Elapsed time: " + elapsedTime);
                
                if (!isShakingCombinations) {
                    if (combinations.Count == 0) { 
                        GetCombinationsAvailable(TeamControl.instance.slots, TeamControl.instance.slots, true);   // get team/team combos
                        GetCombinationsAvailable(TeamControl.instance.slots, ShopControl.instance.slots, false);   // get team/shop combos
                        GetCombinationsAvailable(ShopControl.instance.slots, ShopControl.instance.slots, true);   // get shop/shop combos

                        //if (combinations.Count > 0) Debug.Log("FINISHED FINDING COMBINATIONS");
                    }
                    
                    ShakeCombinations();      // shake combos
                }
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    public void ResetNudge(){
        elapsedTime = 0f;                                           // reset elapsed time
        if (combinations.Count > 0) combinations.Clear();           // clear combinations list
        if (isShakingCombinations) isShakingCombinations = false;   // reset isShakingCombinations
        combinationIndex = 0;                                       // reset combination index

        StopCoroutine(ShakeCombinationsCoroutine());                // stop coroutine
    }

    /* public void GetCombinationsAvailable(List<Slot> firstSlots, List<Slot> secondSlots, bool sameSlotHolder = true){
        for (int i = 0; i < firstSlots.Count; i++){
            if (firstSlots[i].unitGO == null) continue;

            for (int j = 0; j < secondSlots.Count; j++){
                if (secondSlots[j].unitGO == null) continue;

                #region Switch i and j if i is greater than j
                var small = i;  // for readability
                var big = j;

                if (small > big){
                    var temp = small;
                    small = big;
                    big = temp;
                }

                slotPair.firstSlot = firstSlots[small];
                slotPair.secondSlot = secondSlots[big];
                #endregion
                
                if (!combinations.Contains(slotPair) && Combiner.instance.CanCombine(firstSlots[i].unitGO, secondSlots[j].unitGO)){
                    if (sameSlotHolder) {
                        if (i != j) {

                            Debug.Log("Combination available at " + i + " and " + j + "! " + small + " " + big);
                            combinations.Add(slotPair);
                        }
                    }

                    else {
                        Debug.Log("Combination available at " + i + " and " + j + "! " + small + " " + big);
                        combinations.Add(slotPair);
                    }
                }

            }
        }

        if (combinations.Count > 0) Debug.Log("Available combinations: " + combinations.Count);
    } */

    public void GetCombinationsAvailable(List<Slot> firstSlots, List<Slot> secondSlots, bool sameSlotHolder = true){
        //int smallerSizeFirst = firstSlots.Count;    // get the size of the first list
        //int smallerSizeSecond = secondSlots.Count;  // get the size of the second list
        

        for (int i = 0; i < firstSlots.Count; i++){
            if (firstSlots[i].unitGO == null) continue;   // if the slot is empty, skip to the next one

            // check if we are using the same slot holder 
            // i + 1 will prevent duplications if the same list is used
            for (int j = sameSlotHolder ? i + 1 : 0; j < secondSlots.Count; j++){
                if (secondSlots[j].unitGO == null) continue;

                slotPair.firstSlot = firstSlots[i];
                slotPair.secondSlot = secondSlots[j];

                if (!combinations.Contains(slotPair) && Combiner.instance.CanCombine(firstSlots[i].unitGO, secondSlots[j].unitGO)){
                    //Debug.Log("Combination available at " + i + " and " + j + "!");
                    combinations.Add(slotPair);
                }
            }
        }

        //if (combinations.Count > 0) Debug.Log("Available combinations: " + combinations.Count);
    }

    public void ShakeCombinations(){
        if (combinations.Count == 0) return;

        StartCoroutine(ShakeCombinationsCoroutine());
    }

    IEnumerator ShakeCombinationsCoroutine(){
        isShakingCombinations = true;

        combinationIndex = 0;   // reset index
        //Debug.Log("Combination index: " + combinationIndex);

        while (combinationIndex < combinations.Count){

            if (combinationIndex >= 0 && combinationIndex < combinations.Count){
            
            // get the current combination of elements
            //var slotX = combinations[(int)combinations[combinationIndex].x];
            //var slotY = combinations[(int)combinations[combinationIndex].y];

            var slotX = combinations[combinationIndex].firstSlot;
            var slotY = combinations[combinationIndex].secondSlot;

            //Debug.Log("slotx: " + slotX.unitGO.GetComponent<UnitProperties>().chemicalName + " sloty: " + slotY.unitGO.GetComponent<UnitProperties>().chemicalName);

            slotX.unitGO.GetComponent<UnitGraphics>().Pulse();  // change accordingly to what animation we want
            slotY.unitGO.GetComponent<UnitGraphics>().Pulse();

            yield return new WaitForSeconds(slotX.unitGO.GetComponent<UnitGraphics>().shakeDuration + delayBetweenCombinationShake);

            }

            combinationIndex++;
        }
        
        isShakingCombinations = false;

        if (combinationIndex >= combinations.Count){
            combinations.Clear();
            ResetNudge();
        }

    }
    #endregion

    #region Handbook Nudge
    public void HandleHaventOpenedHandbookStreak(){
        handbookNotOpenedStreak++; // increment loss streak
        Debug.Log("Player have not opened handbook " + handbookNotOpenedStreak + " times in a row");

        if (handbookNotOpenedStreak >= numberOfTimesBeforeTheHandbookPopsUp && !hasPlayerOpenedHandbookAfterNudge){
            if (!isHandbookPopupActive) {
                handbookPopupGO.SetActive(true);    // show the popup
                isHandbookPopupActive = true;

                hasPlayerOpenedHandbookAfterNudge = true;   // set to true so the popup doesn't show up again

                Pulse();    // pulse the popup
            }

            ResetHandbookStreak();
        }

        else {
            handbookPopupGO.SetActive(false);
            isHandbookPopupActive = false;
        }
    }

    public void ResetHandbookStreak(){
        handbookNotOpenedStreak = 0;
    }

    public void Pulse(){
        StartCoroutine(PulseCoroutine(pulseSize, pulseDuration, pulseSpeed));
    }

    IEnumerator PulseCoroutine(float pulseSize, float pulseDuration, float pulseSpeed){
        Debug.Log("Pulse coroutine started!");

        while (GameStateMachine.instance.currentState != GameStateMachine.instance.setupState){
            yield return null;
        }

        Debug.Log("Wating for setup state...");

        float elapsedTime = 0f;

        // save the original scale of the object
        Vector2 originalScale = handbookPopupGO.GetComponent<RectTransform>().localScale;

        while(elapsedTime < pulseDuration){
            // calculate the scale of the pulse based on the time and speed
            float pulseScale = 1 + Mathf.Sin(elapsedTime * pulseSpeed) * pulseSize;
            
            // clamp the scale to be between 0 and 1 so it doesn't go become smaller
            pulseScale = Mathf.Max(pulseScale, 0, 1);

            // set the scale of the object
            handbookPopupGO.GetComponent<RectTransform>().localScale = originalScale * pulseScale;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // reset the scale of the object
        handbookPopupGO.GetComponent<RectTransform>().localScale = originalScale;

        //Debug.Log("Pulse coroutine finished!");
    }
    
    #endregion
}
