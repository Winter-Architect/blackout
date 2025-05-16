using System;
using UnityEngine;
using UnityEngine.UIElements;
public class KeyPad : MonoBehaviour
{
    [SerializeField] public string CODE = "";
    private int CODE_LENGHT_LIMIT = 0;

    public UIDocument UIDocument;
    public VisualElement ui;
    
    public VisualElement Buttons;
    public Button deleteButton;
    public Button validateButton;
    
    public Label inputLabel;

    private string code;
    private bool inside = false;
    public static bool IsAnyKeyPadOpen = false; 
    public Door door;

    private void Awake()
    {
        ui = UIDocument.rootVisualElement.Q<VisualElement>("Container");
        CODE_LENGHT_LIMIT = CODE.Length;

        // On ne désactive plus UIDocument ici
        ui.style.display = DisplayStyle.None;

        Buttons = ui.Q<VisualElement>("Numbers");
        code = "";
        for (int i = 0; i < 10; i++)
        {
            Button button = Buttons.Q<Button>(i.ToString());
            var i1 = i;
            button.clicked += () => ClickedNumber(i1);
        }

        inputLabel = ui.Q<VisualElement>("Input").Q<Label>("CodeInput");
        deleteButton = ui.Q<Button>("delete");
        validateButton = ui.Q<Button>("validate");

        deleteButton.clicked += () =>
        {
            code = "";
            inputLabel.text = "";
        };
        validateButton.clicked += ValidateCode;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ui.style.display = DisplayStyle.None;
            IsAnyKeyPadOpen = false; 
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
        
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                ClickedNumber(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            code = "";
            inputLabel.text = "";
        }
        
        if (Input.GetKeyDown(KeyCode.Return)) ValidateCode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player entrer");
            ui.style.display = DisplayStyle.Flex; // Affiche l'UI
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            inside = true;
            IsAnyKeyPadOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player exit");
            ui.style.display = DisplayStyle.None; // Cache l'UI
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            inside = false;
            IsAnyKeyPadOpen = false;
        }
    }

    private void ClickedNumber(int num)
    {
        if (code.Length + 1 > CODE_LENGHT_LIMIT) return;
        code += num.ToString();
        
        inputLabel.text = code;
    }

    private void ValidateCode()
    {
        if (code == CODE)
        {
            Debug.Log("gooooood");
            door.Condition = true;
        }
        else Debug.Log("pas goooood");
        
        code = "";
        inputLabel.text = "";
    }

    public void OpenKeePadui()
    {
        UIDocument.enabled = true;
    }
    
    
}
