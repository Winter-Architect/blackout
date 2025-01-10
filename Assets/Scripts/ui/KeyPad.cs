using System;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyPad : MonoBehaviour
{
    [SerializeField] private string CODE = "";
    private int CODE_LENGHT_LIMIT = 0;

    public UIDocument UIDocument;
    public VisualElement ui;
    
    public VisualElement Buttons;
    public Button deleteButton;
    public Button validateButton;
    
    public Label inputLabel;

    private string code;

    private void Awake()
    {
        ui = UIDocument.rootVisualElement.Q<VisualElement>("Container");
        CODE_LENGHT_LIMIT = CODE.Length;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIDocument.enabled = false;
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

    private void OnEnable()
    {
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

        deleteButton.clicked += () => { 
            code = "";
            inputLabel.text = ""; 
        };
        validateButton.clicked += ValidateCode;


    }

    private void ClickedNumber(int num)
    {
        if (code.Length + 1 > CODE_LENGHT_LIMIT) return;
        code += num.ToString();
        
        inputLabel.text = code;
    }

    private void ValidateCode()
    {
        if (code == CODE) Debug.Log("gooooood");
        else Debug.Log("pas goooood");
        
        code = "";
        inputLabel.text = "";
    }

    public void OpenKeePadui()
    {
        UIDocument.enabled = true;
    }
    
    
}
