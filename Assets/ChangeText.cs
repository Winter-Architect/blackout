using TMPro;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textMeshPro != null)
        {
            Debug.Log("Found TMP");
        }
    }

    // Update is called once per frame
    void Update(string newText)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = newText;
        }
    }
}
