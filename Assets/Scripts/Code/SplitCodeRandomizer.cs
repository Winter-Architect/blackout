using UnityEngine;
using TMPro;

public class SplitCodeRandomizer : MonoBehaviour
{
    public TextMeshProUGUI tmp1;
    public TextMeshProUGUI tmp2;
    public TextMeshProUGUI tmp3;
    public TextMeshProUGUI tmp4;
    public TextMeshProUGUI tmp5;
    public int digits; //represents how many numbers are in the code. Has to be either 4 or 5. Why would you need a 1-digit code anyway. Or 2 digits. Or even 3.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string code = "";

        for (int i = 0; i < digits; i++)
        {
            int value = UnityEngine.Random.Range(0, 9);
            code += value.ToString();
        }

        //Write the different digits on the TMPs. .ToString() is necessary due to each digit being a char.
        tmp1.text = code[0].ToString();
        tmp2.text = code[1].ToString();
        tmp3.text = code[2].ToString();
        tmp4.text = code[3].ToString();

        if (tmp5 != null)
        {
            tmp5.text = code[4].ToString();
        }
    }
}
