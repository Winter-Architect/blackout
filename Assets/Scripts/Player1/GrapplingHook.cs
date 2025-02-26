using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class GrapplingHook : MonoBehaviour, IActionItem
{
    private bool loaded = true;
    [SerializeField] private GameObject projectilePrefab;


    public void PrimaryAction(Agent agent)
    {
        if (!loaded) return;

        Debug.Log("Tried to shoot");
    }

    


}