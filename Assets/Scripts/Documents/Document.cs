using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Document : MonoBehaviour
{
    // Liste de tous les documents disponibles dans le jeu
    public List<DocumentObject> AllDocuments;    
    [SerializeField] protected UIDocument UIDocument;
    private VisualElement ui;
    private ScrollView scrollView;
    private Button ButtonTemplate;
    private VisualElement ImageContainer;
    public Sprite TempImage;
    public Label ImageName;
    private Button MenuButton;

    // Liste des IDs des documents collectés
    private List<int> collectedDocumentIds = new List<int>();

    public static Document Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCollectedDocuments();
        }
        else
        {
            Destroy(gameObject);
        }

        VisualElement root = UIDocument.rootVisualElement;
        
        ui = root.Q<VisualElement>("Container");
        scrollView = ui.Q<ScrollView>("scrollDocs"); 
        ButtonTemplate = scrollView.Q<Button>("Template");
        ImageContainer = ui.Q<VisualElement>("Visu").Q<VisualElement>("Image");
        ImageName = ui.Q<VisualElement>("Visu").Q<Label>("Name");
        MenuButton = ui.Q<Button>("MenuButton");
    }

    void ExitView() {
        UIDocument.sortingOrder = 0;
    }

    // Collecte un document par son ID
    public void CollectDocument(int documentId)
    {
        if (!collectedDocumentIds.Contains(documentId))
        {
            collectedDocumentIds.Add(documentId);
            SaveCollectedDocuments();
            
            DocumentObject doc = GetDocumentById(documentId);
            if (doc != null)
            {
                Debug.Log($"Document collecté: {doc.Name}");
            }
        }
    }
    
    private void SaveCollectedDocuments()
    {
        // Convertir la liste en string séparée par des virgules
        string idList = string.Join(",", collectedDocumentIds);
        PlayerPrefs.SetString("CollectedDocumentIds", idList);
        PlayerPrefs.Save();
    }
    
    private void LoadCollectedDocuments()
    {
        collectedDocumentIds.Clear();
        
        if (PlayerPrefs.HasKey("CollectedDocumentIds"))
        {
            string idList = PlayerPrefs.GetString("CollectedDocumentIds");
            if (!string.IsNullOrEmpty(idList))
            {
                string[] idStrings = idList.Split(',');
                foreach (string idStr in idStrings)
                {
                    if (int.TryParse(idStr, out int id))
                    {
                        collectedDocumentIds.Add(id);
                    }
                }
            }
        }
    }

    private DocumentObject GetDocumentById(int id)
    {
        return AllDocuments.Find(doc => doc.Id == id);
    }

    void Start()
    {
        foreach (int id in collectedDocumentIds)
        {
            DocumentObject doc = GetDocumentById(id);
            if (doc != null)
            {
                CreateButton(doc.Name, doc.Id, doc.Image);
            }
        }
        Debug.Log("cc");
        MenuButton.clicked += ExitView;
        UIDocument.sortingOrder = 0;
    }


    public void CollectDocumentObject(DocumentObject doc)
    {
        CollectDocument(doc.Id);
    }

    public void CreateButton(string name, int id, Sprite Image)
    {
        Button newButton = new Button();
        
        CopyVisualElementStyle(ButtonTemplate, newButton);
        
        newButton.style.display = DisplayStyle.Flex;
        
        Label nameLabel = new Label(name);
        nameLabel.name = "Name";
        nameLabel.style.width = Length.Percent(80);
        nameLabel.style.marginRight = 5;
        
        Label idLabel = new Label(id+1 < 10 ? $"#00{id+1}" : $"#0{id+1}");
        idLabel.name = "Id";
        idLabel.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
        idLabel.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 0.69f));
        
        newButton.Add(nameLabel);
        newButton.Add(idLabel);
        
        newButton.clicked += () => OnButtonClicked(name, id, Image);
        
        scrollView.Add(newButton);
    }

    private void OnButtonClicked(string name, int id, Sprite image) {
        ImageContainer.style.backgroundImage = new StyleBackground(image);
        ImageName.text = name;
    }
    
    private void CopyVisualElementStyle(VisualElement source, VisualElement target)
    {
        target.name = source.name;
        
        target.style.height = new StyleLength(100);
        target.style.marginTop = 20;
        target.style.marginRight = 20;
        target.style.marginBottom = 20;
        target.style.marginLeft = 20;
        target.style.unityFontStyleAndWeight = FontStyle.Bold;
        target.style.fontSize = 30;
        target.style.backgroundColor = new StyleColor(new Color(198/255f, 172/255f, 143/255f));
        target.style.flexDirection = FlexDirection.Row;
        
        foreach (string className in source.GetClasses())
        {
            target.AddToClassList(className);
        }
    }
}