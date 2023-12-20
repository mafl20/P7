using TMPro;
using UnityEngine;

public class SelectionController: MonoBehaviour
{
    public static SelectionController instance;

    public Slot slotA, slotB;


    private void Awake()
    {
        instance = this;
    }

    public void RegisterSlot(Slot slot)
    {
        if (slotA == null && slot.unitGO != null) //if we don't have a firstly selected slot
        {
            slot.SetIsSelected(true);
            EventControl.instance.selectEvent.Invoke();
            slotA = slot;

            //> showing the Sell or Freeze button
            if (slotA.type == Slot.SlotType.Team) { ShopControl.instance.ShowSellButton(); }
            else if (slotA.type == Slot.SlotType.Shop) { ShopControl.instance.ShowFreezeButton(); }

            //Debug.Log("Now holding " + slot.name + " as slot A...");
        }
        else if (slotA != null && slotB == null && slot == slotA) //if the first and second selected slot are the same
        { //QUESTION: couldnt this just be "slotA == slotB"? - lau :)
            ResetSelection();
        }
        else if (slotA.type == Slot.SlotType.Shop && slot.type == Slot.SlotType.Shop) //if both slots are of the shop
        {
            //> swapping out the old selected shop slot with the newly selected shop slot
            slotA.SetIsSelected(false);
            EventControl.instance.deselectEvent.Invoke();
            slot.SetIsSelected(true);
            EventControl.instance.selectEvent.Invoke();
            slotA = slot;
        }
        else if (slotA != null && slotB == null && slot != slotA) //if we have a firstly selected slot, missing a second selected and the new slot is not the first slot
        {
            slot.SetIsSelected(true);
            EventControl.instance.selectEvent.Invoke();
            slotB = slot;
            //Debug.Log("Now holding " + slot.name + " as slot B...");

            if (slotA.type == Slot.SlotType.Shop && slotB.type == Slot.SlotType.Team) //buying
            {
                //TODO: check if the bought element can combine with a potential element in the slot of the team
                //TODO: add GUI that gives the player the choice between reorganizing elements and combining them

                ShopControl.instance.Buy(slotA.index, slotB.index);
                ResetSelection();
            }
            else if (slotA.type == Slot.SlotType.Team && slotB.type == Slot.SlotType.Team) //editing team
            {
                //TODO: check if the two elements can be combined
                //TODO: add GUI that gives the player the choice between reorganizing elements and combining them

                if (TeamControl.instance.changeType == TeamControl.ChangeType.Swap) { TeamControl.instance.Swap(slotA.index, slotB.index); }
                else if (TeamControl.instance.changeType == TeamControl.ChangeType.Requeue) { TeamControl.instance.Requeue(slotA.index, slotB.index); }
                ResetSelection();
            }
            else
            {
                Debug.LogWarning("Cannot move element from " + slotA.type.ToString() + " to " + slotB.type.ToString());
                ResetSelection();
            }
        }
        else
        {
            Debug.LogWarning("Already holding 2 slots!");
            ResetSelection();
        }
    }

    public void ResetSelection()
    {
        if(slotA != null) { slotA.SetIsSelected(false); }
        if(slotB != null) { slotB.SetIsSelected(false); }

        slotA = null;
        slotB = null;

        ShopControl.instance.SetSellButtonActivation(false);
        ShopControl.instance.SetFreezeButtonActivation(false);
    }
}