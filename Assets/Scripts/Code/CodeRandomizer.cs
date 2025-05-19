using UnityEngine;
using TMPro;

public class CodeRandomizer : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string code = "";

        for (int i = 0; i < 4; i++)
        {
            int value = Random.Range(0, 9);
            code += value.ToString();
        }

        textMeshPro.text = code;
    }
}
