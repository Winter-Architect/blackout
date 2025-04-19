using UnityEngine;

public class Flashlight : MonoBehaviour , IActionItem
{
    public Transform followTarget; // à assigner lors de l'équipement
    public Vector3 rotationOffset = new Vector3(30, 50, 30);

    public void PrimaryAction(Agent agent)
    {
        var light = gameObject.GetComponentInChildren<Light>();
        if (light == null)
        {
            Debug.LogWarning("No light component found in children.");
            return;
        }  
        light.enabled = !light.enabled;
    }

    void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position;
            transform.rotation = followTarget.rotation * Quaternion.Euler(rotationOffset);
        }
    }
}
