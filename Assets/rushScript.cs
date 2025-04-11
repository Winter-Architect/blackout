using UnityEngine;
using UnityEngine.Video;
public class rushScript : MonoBehaviour
{
    public string PlayerTag;
    public bool Condition = false;
    
    public GameObject ScreamerPanel;         // The UI Panel with the RawImage
    public VideoPlayer ScreamerVideo;        // The VideoPlayer component
    void OnTriggerEnter(Collider other)
    {        
        if ( other.CompareTag(PlayerTag) && Condition == true)
        {
            Debug.Log("rushactivated");
            ScreamerPanel.SetActive(true);   // Show the screamer panel
            ScreamerVideo.Play();            // Start the video
        }
    }
}
