using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    #region Slot Properties
    public enum SlotType
    {
        Team,
        Shop,
        Battle
    }
    [Header("Slot Properties")]
    [Tooltip("The type of slot, determines its behaviour...")]
    public SlotType type;
    public byte index;
    [Tooltip("The associated element")]
    public GameObject unitGO; //GO = GameObject
    public static int siblingIndex = 0;
    #endregion

    #region Graphics
    [Header("Graphics")]
    public bool isHovered;
    public bool isSelected;
    public bool isFrozen;
    [Tooltip("A Vector2 represnting the position of the RectTransform component")]
    public RectTransform unitSpot;
    [SerializeField] GameObject hoverHighlightImageGO; //GO = GameObject
    [SerializeField] GameObject selectionHighlightImageGO;
    [SerializeField] GameObject frozenHighlightImageGO;
    //[SerializeField] GameObject infoPanelGO; //TODO: add this here instead of on unit
    #endregion

    public void OnClick()
    {
        if(unitGO != null)
        {
            unitGO.GetComponent<Animator>().SetTrigger("clicked");
        }

        if(type != SlotType.Battle)
        {
            //Debug.Log(gameObject.name + " was clicked!");
            if (SelectionController.instance.slotA == null && unitGO == null)
            {
                Debug.LogWarning("Can not start by selecting empty slot first!");
            }
            else
            {
                SelectionController.instance.RegisterSlot(this);
            }
        }
    }

    /// <summary>
    /// Set the hovered state of the slot.
    /// </summary>
    public void SetIsHovered(bool newBool)
    {
        isHovered = newBool;
        ControlHoverHighlight();
    }

    public void SetIsSelected(bool newBool)
    {
        isSelected = newBool;
        ControlSelectionHighlight();

        #region Sounds
        AudioManager.instance.Play("Click");
        #endregion
    }

    public void SetIsFrozen(bool newBool)
    {
        if (isFrozen == true) { EventControl.instance.unfreezeEvent.Invoke(); }
        else                  { EventControl.instance.freezeEvent.Invoke(); }
        isFrozen = newBool;

        ControlFrozenHighlight();
    }
    public bool GetIsFrozen() { return isFrozen; }

    /// <summary>
    /// Control the toggling of the hover highlight object.
    /// </summary>
    public void ControlHoverHighlight()
    {
        if (type != SlotType.Battle) { hoverHighlightImageGO.SetActive(isHovered); }

        if(unitGO != null)
        {
            unitGO.GetComponent<UnitGraphics>().infoPanelGO.SetActive(isHovered);

            //if(type == SlotType.Shop)
            //{
            //    unitGO.GetComponent<UnitGraphics>().buyInfoPanelGO.SetActive(isHovered);
            //    unitGO.GetComponent<UnitGraphics>().coinIconGO.SetActive(isHovered);
            //}

            //else if (type == SlotType.Team)
            //{
            //    unitGO.GetComponent<UnitGraphics>().sellProfitGO.SetActive(isHovered);
            //    unitGO.GetComponent<UnitGraphics>().emptyCoinGO.SetActive(isHovered);
            //}
        }
    }

    /// <summary>
    /// Control the toggling of the selection highlight object.
    /// </summary>
    public void ControlSelectionHighlight()
    {
        if (type != SlotType.Battle)
        {
            selectionHighlightImageGO.SetActive(isSelected);
        }
        
    }

    /// <summary>
    /// Control the toggling of the frozen highlight object.
    /// </summary>
    public void ControlFrozenHighlight()
    {
        if (type != SlotType.Battle)
        {
            frozenHighlightImageGO.SetActive(isFrozen); 
        }
    }

    public void DeleteUnitGO()
    {
        if(unitGO != null) Destroy(unitGO);
        Clear();
    }

    public void Clear()
    {
        unitGO = null;

        if (isFrozen == true)
        {
            SetIsFrozen(false);
        }

        if (type == SlotType.Team)
        {
            transform.parent.GetComponent<TeamControl>().UpdateFullCheck();
        }
    }

    public string GetUnitChemicalName()
    {
        if(unitGO == null)
        {
            return null;
        }
        else
        {
            return unitGO.GetComponent<UnitProperties>().GetChemicalName();
        }
    }

    public string[] GetUnitCompoundsChemicalName()
    {
        if (unitGO == null)
        {
            return null;
        }
        else
        {
            string[] names = new string[5];

            names[0] = unitGO.GetComponent<UnitProperties>().level.ToString();
            names[1] = unitGO.GetComponent<UnitProperties>().elementAGO.GetComponent<ElementProperties>().GetChemicalName();
            names[2] = unitGO.GetComponent<UnitProperties>().elementACount.ToString();

            if (unitGO.GetComponent<UnitProperties>().elementBGO != null)
            {
                names[3] = unitGO.GetComponent<UnitProperties>().elementBGO.GetComponent<ElementProperties>().GetChemicalName();
                names[4] = unitGO.GetComponent<UnitProperties>().elementBCount.ToString();
            }
            else
            {
                names[4] = "";
            }

            return names;
        }
    }
}