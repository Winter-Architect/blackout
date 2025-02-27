using UnityEngine;
using TMPro;

public class ChangeText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    int digits = 4;
    string code = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("Not Found");
        }
        else
        {
            Debug.LogError("Found !");

        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < digits; i++)
        {
            int value = Random.Range(0, 9);
            code += value.ToString();
        }

        Debug.Log(code);
        code = "";
    }
}
