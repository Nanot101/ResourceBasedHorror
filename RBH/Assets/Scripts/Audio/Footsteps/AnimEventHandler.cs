using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private MapManager mapManager;

    private void Awake() {
        mapManager = FindObjectOfType<MapManager>();
        audioSource = GetComponentInChildren<AudioSource>(); 
    }

    public void Step() {
        if (mapManager != null && audioSource != null) {
            AudioClip currentFloorClip = mapManager.GetCurrentFloorClip(transform.position);
            audioSource.PlayOneShot(currentFloorClip);
        }
        else {
            return;
        }
    }
}