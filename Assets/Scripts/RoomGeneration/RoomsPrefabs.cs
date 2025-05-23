using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Rooms", menuName = "Scriptable Objects/Rooms")]
public class Rooms : ScriptableObject
{
 public NetworkObject[] roomPrefabs;  
}
