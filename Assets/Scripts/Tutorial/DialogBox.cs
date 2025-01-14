using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogBox : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textArea;
    private Queue<string> messageQueue = new Queue<string>();

    public void DisplayNextText()
    {
        if(messageQueue.Count > 0)
        {
            textArea.text = messageQueue.Dequeue();
        }
        else
        {
            CloseBox();
        }
    }

    public void EnqueueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    public void CloseBox()
    {
        messageQueue.Clear();
        Destroy(gameObject);
    }

    
}
