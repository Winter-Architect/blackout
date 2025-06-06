using System;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
public class KeyPad : MonoBehaviour
{
    private static string CODE = "1234";
    public bool Split; 
    public int Digits; //HAS TO BE EITHER 4 OR 5
    private int CODE_LENGHT_LIMIT { get => CODE.Length; }

    public TextMeshProUGUI tmp1;
    public TextMeshProUGUI tmp2;
    public TextMeshProUGUI tmp3;
    public TextMeshProUGUI tmp4;
    public TextMeshProUGUI tmp5;

    public UIDocument UIDocument;
    public VisualElement ui;
    
    public VisualElement Buttons;
    public Button deleteButton;
    public Button validateButton;
    
    public Label inputLabel;

    private string code;
    public static bool IsAnyKeyPadOpen = false; 
    public Door door;

    void Start()
    {
        /*for (int i = 0; i < Digits; i++)
        {
            int value = UnityEngine.Random.Range(0, 10);
            CODE += value.ToString();
        }*/

        if (Digits == 5)
        {
            CODE = "12345";
        }

        if (Split)
        {
            tmp1.text = CODE[0].ToString();
            tmp2.text = CODE[1].ToString();
            tmp3.text = CODE[2].ToString();
            tmp4.text = CODE[3].ToString();

            if (tmp5 != null)
            {
                tmp5.text = CODE[4].ToString();
            }
        }
        else
        {
            tmp1.text = CODE;
        }
    }

    private void Awake()
    {
        ui = UIDocument.rootVisualElement.Q<VisualElement>("Container");

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
            // if (!CompareTag("Player")) // ou autre condition pour reconnaître l'agent
            // {
            //     Destroy(this);
            //     return;
            // }

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
        if (other.CompareTag("Player") && other.GetComponent<Agent>() != null)
        {
            Debug.Log("player entrer");
            ui.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            IsAnyKeyPadOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Agent>() != null)
        {
            Debug.Log("player exit");
            ui.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            IsAnyKeyPadOpen = false;
        }
    }

    private void ClickedNumber(int num)
    {
        Debug.Log("clicked number " + num);
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
