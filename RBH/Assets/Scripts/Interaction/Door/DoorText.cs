using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorText : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    void Update()
    {
        if (mainCamera != null)
        {
            Vector3 lookDirection = transform.position - mainCamera.transform.position; // Look towards the camera
            lookDirection.x = 0; // Keep the text upright (don't rotate on the X-axis)
            lookDirection.z = 0; // Keep the text upright (don't rotate on the Z-axis)

            transform.rotation = Quaternion.LookRotation(lookDirection); // Apply the rotation to face the camera
        }
        else if (mainCamera == null) {
            Debug.Log("camera is null");
        }
    }
}

