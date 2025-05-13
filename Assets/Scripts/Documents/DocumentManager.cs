using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DocumentManager : MonoBehaviour
{
    // Liste de tous les documents disponibles dans le jeu
   
    // Liste des IDs des documents collectés
    private List<int> collectedDocumentIds = new List<int>();

    public static DocumentManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Collecte un document par son ID
    public void CollectDocument(int documentId)
    {
        Debug.Log("collectdoc");
        if (!collectedDocumentIds.Contains(documentId))
        {
            collectedDocumentIds.Add(documentId);
            SaveCollectedDocuments();
        Debug.Log("collectdoc2");
            
            // DocumentObject doc = GetDocumentById(documentId);
            // if (doc != null)
            // {
            //     Debug.Log($"Document collecté: {doc.Name}");
            // }
        }
    }
    
    private void SaveCollectedDocuments()
    {
        // Convertir la liste en string séparée par des virgules
        string idList = string.Join(",", collectedDocumentIds);
        PlayerPrefs.SetString("CollectedDocumentIds", idList);
        PlayerPrefs.Save();
    }
    

    public void CollectDocumentObject(DocumentObject doc)
    {
        CollectDocument(doc.Id);
    }
}