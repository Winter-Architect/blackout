using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform playerCamera;

    void Update()
    {
        if (playerCamera == null) // If no camera is assigned, find it
        {
            if (Camera.main != null)
                playerCamera = Camera.main.transform;
            else
                return; // Exit if the camera doesn't exist yet
        }

        transform.LookAt(playerCamera); // Make the canvas face the camera*
    }
}