using System;
using UnityEngine;
using UnityEngine.Video;
public class rushScript : MonoBehaviour
{
    public string PlayerTag;
    public bool Condition = false;
    
    public GameObject ScreamerPanel;         // The UI Panel with the RawImage
    public VideoPlayer ScreamerVideo;        // The VideoPlayer component
    private FieldOfView fieldOfView;
    
    private void Awake()
    {
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
    }

    void OnTriggerEnter(Collider other)
    {        
        if ( other.CompareTag(PlayerTag) && Condition == true)
        {
            Debug.Log("rushactivated");
            
            Agent agent = other.gameObject.GetComponent<Agent>();
            
            if (agent.isInLocker == false)
            {
                agent.Health = 0;
                ScreamerPanel.SetActive(true);   // Show the screamer panel
                ScreamerVideo.Play();            // Start the video
            }
        }
    }
}
