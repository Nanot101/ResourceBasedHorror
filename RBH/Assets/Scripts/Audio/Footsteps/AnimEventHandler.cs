using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private MapManager mapManager;
   [SerializeField] private float minPitch = 0.55f;
   [SerializeField] private float maxPitch = 1.5f;

    private void Awake() {
        mapManager = FindObjectOfType<MapManager>();
        audioSource = GetComponentInChildren<AudioSource>(); 
    }

    public void Step() {
        if (mapManager != null && audioSource != null) {
            AudioClip currentFloorClip = mapManager.GetCurrentFloorClip(transform.position);
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(currentFloorClip);
        }
        else {
            return;
        }
    }
}