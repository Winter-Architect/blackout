using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blackout.Inventory
{
    public class InventoryController : MonoBehaviour
    {

        public Animator anim;
        private bool InvWheelSelected = false;
        public Image selectedItem;
        public Sprite noImage;
        public static int selectedItemId = 0;
        public Dictionary<int, Item> inventory = new Dictionary<int, Item>();
        // public List<Image> inventorySlots = new List<Image>();
        public List<InventoryButtonController> inventorySlots = new List<InventoryButtonController>();
        public List<int> freeSlots = new List<int>(){0,1,2,3,4,5};
        public static int activeInventorySlotId = 0;

        public static InventoryController Instance;

        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            for (int i = 0; i < inventory.Count; i++) {
                inventorySlots[i].icon = inventory[i].Icon;
                freeSlots.Remove(i);
            }
            //inventorySlots1[0].icon = null; //= inventory[0].Icon
        }

        void Update()
        {
            
        }

        public void toggleInventory() {
            if (Input.GetKeyDown(KeyCode.Tab)) InvWheelSelected = !InvWheelSelected;
            anim.SetBool("OpenInv", InvWheelSelected);
        }

        public void AddItemToInventory(Item obj) {
            if (freeSlots.Count == 0) {
                Debug.Log("Inventory is full");
                return;
            }
            inventory.Add(freeSlots[0], obj);
            selectedItemId = obj.Id;
            inventorySlots[freeSlots[0]].icon = obj.Icon;
            inventorySlots[freeSlots[0]].itemName = obj.Name;
            freeSlots.RemoveAt(0);
            Agent.AddItemToAgentInventory(obj);
        }

        public void RemoveItemFromInv(Item obj) {
            foreach (var objec in inventory) {
                if (objec.Value == obj) {
                    inventory.Remove(objec.Key);
                    selectedItemId = 0;

                    freeSlots.Add(objec.Key);
                    inventorySlots[objec.Key].icon = noImage;
                    inventorySlots[objec.Key].itemName = "";
                    break;
                }
            }
        }
    }
}
