using UnityEngine;

public class Flashlight : MonoBehaviour , IActionItem
{
    public Transform followTarget; // à assigner lors de l'équipement
    public Vector3 rotationOffset = new Vector3(30, 50, 30);
    private Agent agent;
    
    private float energyTimer = 0f; // Ajout du timer

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
        if (agent == null)
        {
            agent = GetComponentInParent<Agent>();
            if (agent == null)
            {
                Debug.LogWarning("No Agent component found in parent.");
                return;
            }
        }
        if (agent.Energy <= 0) return;

        var light = gameObject.GetComponentInChildren<Light>();
        if (light != null && light.enabled)
        {
            energyTimer += Time.deltaTime;
            if (energyTimer >= 1f)
            {
                agent.Energy -= 1;
                energyTimer = 0f;
            }
        }
        else
        {
            energyTimer = 0f; 
        }

        if (followTarget != null)
        {
            transform.position = followTarget.position;
            transform.rotation = followTarget.rotation * Quaternion.Euler(rotationOffset);
        }
    }
}
