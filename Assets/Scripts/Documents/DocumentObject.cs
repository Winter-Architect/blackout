using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Document", menuName = "Scriptable Objects/Document")]
public class DocumentObject : ScriptableObject
{
    public string Name;
    public Sprite Image;
    public int Id;
}
