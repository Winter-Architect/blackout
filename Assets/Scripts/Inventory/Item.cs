using UnityEngine;



[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public int Id;


}