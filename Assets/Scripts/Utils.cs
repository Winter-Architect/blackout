using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class Utils : MonoBehaviour
    {
        
        void Start()
        {
            string generatedCode = GenerateCode();

            Debug.Log($"Code généré : {generatedCode}");
        }
        public string GenerateCode()
        {
            char[] charsForTheCode = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 
                'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z', '1', 
                '2', '3', '4', '5', '6', '7', '8', '9' };

            string code = "";
            
            System.Random random = new System.Random();


            for (int i = 0; i < 4; i++)
            {
                code += charsForTheCode[random.Next(0, charsForTheCode.Length)];
            }

            return code;

        }
    }
}