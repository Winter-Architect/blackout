using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    private TextMeshProUGUI textArea; // Pour le texte du message
    private TextMeshProUGUI actionText; // Pour le texte de l'action

    // La queue contient maintenant des listes de string : [message, action]
    private Queue<List<string>> messageQueue = new Queue<List<string>>();

    private void Start()
    {
        // Cherche les TextMeshPro à partir de ce GameObject (le prefab instancié)
        Transform panelTransform = transform.Find("Panel");

        if (panelTransform != null)
        {
            textArea = panelTransform.Find("Text").GetComponent<TextMeshProUGUI>();
            actionText = panelTransform.Find("Action").GetComponent<TextMeshProUGUI>();

            if (textArea == null || actionText == null)
            {
                Debug.LogError("Les TextMeshPro 'Text' ou 'Action' n'ont pas été trouvés dans le Panel.");
            }
        }
        else
        {
            Debug.LogError("Le Panel n'a pas été trouvé dans le DialogBox.");
        }
    }

    public void DisplayNextText()
    {
        if (messageQueue.Count > 0)
        {
            List<string> entry = messageQueue.Dequeue();
            string message = entry[0];
            string action = entry.Count > 1 ? entry[1] : null;

            if (textArea != null)
            {
                Debug.Log("Nouveau message affiché : " + message);
                textArea.text = message;
            }

            if (actionText != null)
            {
                actionText.text = string.IsNullOrEmpty(action) ? "" : action;
            }
        }
        else
        {
            CloseBox();
        }
    }

    // Ajoute un message et une action associée (action peut être null ou vide si pas d'action)
    public void EnqueueMessage(string message, string action = null)
    {
        messageQueue.Enqueue(new List<string> { message, action });
    }

    public void CloseBox()
    {
        messageQueue.Clear();
        Destroy(gameObject);
    }
}