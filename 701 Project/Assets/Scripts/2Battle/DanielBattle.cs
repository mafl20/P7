using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanielBattle : MonoBehaviour
{
    public TeamControl teamControl;     // To get the team slots
    public List<Slot> teamSlots;        // The slots in this scene
    public List<Slot> enemySlots;
    public GameObject elementGOPrefab;
    public int maxIndexElement;     // set to 20 for now

    public GameObject teamUI;   // Get the team

    void Awake()
    {
        //teamUI = GameObject.Find("TeamUI");
        StartCoroutine(GenerateEnemyTeamCoroutine());
        //GenerateEnemyTeam(); // Set enemy team at start (when this script is activated)
        //Debug.Log("Enemy was generated at start");
    }

    void Update()
    {
        // DEBUGGING
        if (Input.GetKeyDown(KeyCode.G)){
            Debug.Log("Enemy was generated with G");
            GenerateEnemyTeam();
        }   
    }

    // Have to wait a bit or the elements won't be in the slots, maybe because they need time to enable??
    IEnumerator GenerateEnemyTeamCoroutine(){
        Debug.Log("Called GenerateEnemyTeam()");
        yield return new WaitForSeconds(.5f);
        PlaceTeam();
        GenerateEnemyTeam();
        Debug.Log("Battle ended");
    }

    public void GenerateEnemyTeam(){
        Debug.Log("Enemy team was generated");

        foreach (Slot slot in enemySlots){
            GameObject newElementGO = Instantiate(elementGOPrefab, slot.unitSpot.position, Quaternion.identity, slot.unitSpot);

            int randomIndex = Random.Range(1, maxIndexElement); //
            //Element newElement = new Element(randomIndex);
            //newElementGO.GetComponent<ElementUI>().element = newElement;
            //newElementGO.GetComponent<ElementUI>().UpdateGraphics();
            //newElementGO.name = newElement.GetChemicalName();
        }
    }

    public void PlaceTeam(){
        teamUI.transform.SetParent(GameObject.Find("Battle").transform);    // Set the parent of the team to the battle UI
        teamUI.GetComponent<RectTransform>().localPosition = new Vector3(-450, 110, 0); // Transform the team ui so it fits both teams on screen
        Debug.Log("Team was placed");

        // VVVVVVV why am i even doing this, i can just move the team parent to the other UI??
        // gameobject.setup.getchild teamUI -> move it to battleUI ........
        // then, when in battle phase i cannont touch the team

/*         for (int i = 0; i < teamControl.slots.Count; i++){  // For every slot in the team
            if (teamControl.slots[i] != null){
                Debug.Log("Slot " + i + " is not null");

                GameObject newElementGO = Instantiate(elementGOPrefab, teamSlots[i].position, Quaternion.identity, teamSlots[i].transform); // Create new element game object
                Debug.Log("New element game object was created");

                newElementGO.GetComponent<ElementUI>().element = teamControl.slots[i].GetComponent<Slot>().elementGO.GetComponent<ElementUI>().element; // Assign the element from the team to the slot
                newElementGO.GetComponent<ElementUI>().UpdateGraphics();
                newElementGO.name = teamControl.slots[i].GetComponent<Slot>().elementGO.name;
                teamSlots[i].elementGO = newElementGO; // Assign the element game object to the slot
                Debug.Log("Element was assigned to slot");

                teamSlots[i].elementGO.GetComponent<RectTransform>().position = teamSlots[i].position; // Set the position of the element game object to the slot position
                teamSlots[i].elementGO.transform.SetParent(teamSlots[i].transform); // Set the parent of the element game object to the slot
                teamSlots[i].elementGO.transform.SetSiblingIndex(Slot.siblingIndex); // Set the sibling index of the element game object to the slot sibling index
                Debug.Log("Changed parent");
            }

            Debug.Log(teamControl.slots.Count + " " + teamSlots.Count);
        } */
    }

}
