using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<GameObject> itemPrefabs;
    private Dictionary<int, GameObject> itemPrefabDict = new Dictionary<int, GameObject>();

    private void Start()
    {
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            itemPrefabDict[i] = itemPrefabs[i];
        }
    }

    public GameObject GetPrefabById(int id)
    {
        return itemPrefabDict.ContainsKey(id) ? itemPrefabDict[id] : null;
    }
}
