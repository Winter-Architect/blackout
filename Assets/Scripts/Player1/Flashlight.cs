using UnityEngine;

public class Flashlight : MonoBehaviour, IActionItem
{
    public Transform followTarget;
    public Vector3 rotationOffset = new Vector3(30, 50, 30);

    private Agent agent;
    private float energyTimer = 0f;
    private Light flashlight;

    void Awake()
    {
        flashlight = GetComponentInChildren<Light>();

        if (flashlight == null)
        {
            Debug.LogWarning("No Light component found in children.");
        }
    }

    public void PrimaryAction(Agent agent)
    {
        this.agent = agent;

        if (flashlight != null && agent.playerCamera != null && flashlight.transform.parent != agent.playerCamera)
        {
            flashlight.transform.SetParent(agent.playerCamera);
            flashlight.transform.localPosition = Vector3.zero;
            flashlight.transform.localRotation = Quaternion.identity;
            flashlight.intensity *= 1.1f;
            flashlight.spotAngle = 30f;
            flashlight.range = 28f;
        } if (flashlight == null) return;
        
        flashlight.enabled = !flashlight.enabled;
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

        if (agent.Energy <= 0)
        {
            if (flashlight != null) TurnOff();
            return;
        }

        if (flashlight != null && flashlight.enabled)
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
    
    public void TurnOff()
    {
        if (flashlight != null) flashlight.enabled = false;
    }
}
