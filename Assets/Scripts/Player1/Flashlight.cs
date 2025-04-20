using UnityEngine;

public class Flashlight : MonoBehaviour, IActionItem
{
    public Transform followTarget; // assign this when equipping
    public Vector3 rotationOffset = new Vector3(30, 50, 30);
    private Agent agent;

    private float energyTimer = 0f; // Ajout du timer
    public float batteryLifeSeconds = 120f; // current charge
    public float maxBatteryLife = 120f;
    
    private Light flashlightLight;
    private bool isOn = false;

    void Start()
    {
        flashlightLight = GetComponentInChildren<Light>();
        if (flashlightLight == null)
        {
            Debug.LogWarning("No light component found in children.");
        }
    }

    public void PrimaryAction(Agent agent)
    {
        if (flashlightLight == null) return;

        // If battery is empty, try to refill
        if (batteryLifeSeconds <= 0f)
        {
            if (agent.batteryCount > 0)
            {
                agent.batteryCount--;
                batteryLifeSeconds = maxBatteryLife;
                Debug.Log("Battery replaced. Batteries left: " + agent.batteryCount);
            }
            else
            {
                Debug.Log("No batteries left!");
                return;
            }
        }

        // Toggle flashlight
        isOn = !isOn;
        flashlightLight.enabled = isOn;
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

        if (isOn && batteryLifeSeconds > 0f)
        {
            batteryLifeSeconds -= Time.deltaTime;

            if (batteryLifeSeconds <= 0f)
            {
                batteryLifeSeconds = 0f;
                isOn = false;
                flashlightLight.enabled = false;
                Debug.Log("Battery depleted.");
            }
        }
    }
}