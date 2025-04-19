using UnityEngine;

public class lockerDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string PlayerTag;
    public bool Condition = true;
    void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag(PlayerTag) && Condition == true)
        {
            Debug.Log("enter a locker");
            
            Agent agent = other.gameObject.GetComponent<Agent>();
            agent.isInLocker = true;
        }
    }
    void OnTriggerExit(Collider other)
    {        
        if (other.CompareTag(PlayerTag) && Condition == true)
        {
            Debug.Log("leave a locker");
            
            Agent agent = other.gameObject.GetComponent<Agent>();
            agent.isInLocker = false;
        }
    }
}
