using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Blackout.Inventory
{
    public class InventoryButtonController : MonoBehaviour
    {
        private Animator anim;
        public string itemName;
        public TextMeshProUGUI itemText;
        public Image selectedItem;
        public Image ItemSlot;
        private bool selected = false;
        public Sprite icon;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (selected) {
                selectedItem.sprite = icon;
                itemText.text = itemName;
            }
            ItemSlot.sprite = icon;

        }

        public void Selected() {
            selected = true;
            // InventoryController.selectedItemId = Id;
        }
        
        public void Deselected() {
            selected = false; 
           // InventoryController.selectedItemId = 0;
        }

        public void HoverEnter() {
            anim.SetBool("Hover", true);
            itemText.text = itemName;
        }

        public void HoverExit() {
            anim.SetBool("Hover", false);
            itemText.text = "";
        }
    }
}
