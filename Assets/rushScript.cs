using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

public class rushScript : NetworkBehaviour
{
    public string PlayerTag;
    public bool Condition = false;

    public GameObject ScreamerPanel;         // The UI Panel with the RawImage
    public VideoPlayer ScreamerVideo;        // The VideoPlayer component
    private FieldOfView fieldOfView;

    private void Awake()
    {
        // Start the field of view coroutine
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        StartCoroutine(fieldOfView.FOVCoroutine());
    }

    private void Start()
    {
        // Find ScreamerPanel in the scene
        ScreamerPanel = GameObject.Find("ScreamerPanel");
        if (ScreamerPanel == null)
        {
            Debug.LogWarning("ScreamerPanel not found in the scene hierarchy.");
        }

        // Find the VideoPlayer (assumes it's named "ScreamerVideo" in the hierarchy)
        GameObject videoObject = GameObject.Find("ScreamerVideoPlayer");
        if (videoObject != null)
        {
            ScreamerVideo = videoObject.GetComponent<VideoPlayer>();
            if (ScreamerVideo == null)
            {
                Debug.LogWarning("VideoPlayer component not found on ScreamerVideo GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("ScreamerVideo GameObject not found in the scene hierarchy.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) && Condition == true)
        {
            Debug.Log("rushactivated");

            Agent agent = other.gameObject.GetComponent<Agent>();

            if (agent != null && agent.isInLocker == false)
            {
                agent.Health = 0;

                if (ScreamerPanel != null)
                    ScreamerPanel.SetActive(true);

                if (ScreamerVideo != null)
                    ScreamerVideo.Play();
            }
        }
    }
}