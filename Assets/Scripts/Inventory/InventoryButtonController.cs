using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Blackout.Inventory
{
    public class InventoryButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int Id;
        private Animator anim;
        public string itemName;
        public TextMeshProUGUI itemText;
        public Image selectedItem;
        public Image ItemSlot;
        private bool selected = false;
        public Sprite icon;
        [SerializeField] private int slotId;

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
            Debug.LogWarning("Selected");
            selected = true;
            InventoryController.activeInventorySlotId = slotId;
            InventoryController.selectedItemId = Id;
        }

        public void Deselected() {
            Debug.LogWarning("Deselected");
            selected = false; 
            if (InventoryController.selectedItemId == Id)
           InventoryController.selectedItemId = -1;
        }

        public void HoverEnter() {
            Debug.LogWarning("HoverEnter");
            anim.SetBool("Hover", true);
            itemText.text = itemName;
        }

        public void HoverExit() {
            Debug.LogWarning("HoverExit");
            anim.SetBool("Hover", false);
            itemText.text = "";
        }

        public void OnPointerClick(PointerEventData eventData) => Debug.Log("Click!");
        public void OnPointerEnter(PointerEventData eventData) => Debug.Log("Enter!");
        public void OnPointerExit(PointerEventData eventData) => Debug.Log("Exit!");
    }
}
