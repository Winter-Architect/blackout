using UnityEngine;
using UnityEngine.UIElements;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public UIDocument UIDocument;
    public VisualElement ui;
    public VisualElement BarsContainer;
    public VisualElement HealthBar;
    public VisualElement EnergyBar;

    public int HealthBarValue = 100;
    public int EnergyBarValue = 100;

    
    void Start()
    {
         ui = UIDocument.rootVisualElement.Q<VisualElement>("Container");
            BarsContainer = ui.Q<VisualElement>("BarsContainer");
            HealthBar = BarsContainer.Q<VisualElement>("HealthBar").Q<VisualElement>("BarBG").Q<VisualElement>("BarFill");
            EnergyBar = BarsContainer.Q<VisualElement>("EnergyBar").Q<VisualElement>("BarBG").Q<VisualElement>("BarFill");
    }

    // Update is called once per frame
    void Update()
    {
        // HealthBar.style.width = Length.Percent(HealthBarValue);
        // EnergyBar.style.width = Length.Percent(EnergyBarValue);
    }
}
