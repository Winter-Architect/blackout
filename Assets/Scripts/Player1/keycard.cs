using UnityEngine;

public class Keycard : MonoBehaviour, IActionItem
{
    public Transform followTarget; // assign this when equipping
    public Vector3 rotationOffset = new Vector3(30, 50, 30);

    void Start()
    {
        
    }

    public void PrimaryAction(Agent agent)
    {
        
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