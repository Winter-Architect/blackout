using UnityEngine;
using TMPro;

public class ChangeText2 : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("Not found bruh");
        }
        else
        {
            Debug.LogError("Here");
        }
    }
}
