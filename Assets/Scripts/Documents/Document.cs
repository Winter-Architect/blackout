using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;

public class Document : MonoBehaviour
{
    public List<DocumentObject> Documents;    
    [SerializeField] protected UIDocument UIDocument;
    private VisualElement ui;
    private ScrollView scrollView;
    private Button ButtonTemplate;
    private VisualElement ImageContainer;
    public Sprite TempImage;

 List<DocumentObject> DocumentsList = new List<DocumentObject>();

   
    private void Awake()
    {
       VisualElement root = UIDocument.rootVisualElement;
        
        ui = root.Q<VisualElement>("Container");
        scrollView = ui.Q<ScrollView>("scrollDocs"); 
        ButtonTemplate = scrollView.Q<Button>("Template");
        ImageContainer = ui.Q<VisualElement>("Visu").Q<VisualElement>("Image");

        ImageContainer.style.backgroundImage = new StyleBackground(TempImage);
        

         for (int i = 0; i < 15; i++)
        {
            var doc = ScriptableObject.CreateInstance<DocumentObject>();
            doc.Name = $"Document {i+1}";
            doc.Id = i;
            doc.Image = TempImage; 
            DocumentsList.Add(doc);
        }

       
    }

    void Start()
    {
         foreach (var doc in DocumentsList)
        {
            CreateButton(doc.Name, doc.Id);
        }
    }

    public void CreateButton(string name, int id)
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
        
        //newButton.clicked += () => OnButtonClicked(name, id);
        
        scrollView.Add(newButton);
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