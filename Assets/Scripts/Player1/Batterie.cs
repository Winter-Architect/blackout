using Blackout.Inventory;
using UnityEngine;

public class Batterie : MonoBehaviour, IActionItem
{
    public void PrimaryAction(Agent agent)
    {
        Debug.Log("Battery used");
        agent.Energy = 100;

        InventoryController.Instance.RemoveItemFromInvByName("Battery");
    }
}