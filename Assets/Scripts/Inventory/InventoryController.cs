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

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) InvWheelSelected = !InvWheelSelected;
            anim.SetBool("OpenInv", InvWheelSelected);

            switch (selectedItemId) {
                case 0:
                    selectedItem.sprite = noImage;
                    break;  
                case 1:
                    Debug.Log("Selected Item 1");
                    break;
                case 2:
                    Debug.Log("Selected Item 2");
                    break;
                case 3:
                    Debug.Log("Selected Item 3");
                    break;
                case 4:
                    Debug.Log("Selected Item 4");
                    break;
                case 5:
                    Debug.Log("Selected Item 5");
                    break;
                case 6:
                    Debug.Log("Selected Item 6");
                    break;
                case 7:
                    Debug.Log("Selected Item 7");
                    break;
            }
        }
    }
}
