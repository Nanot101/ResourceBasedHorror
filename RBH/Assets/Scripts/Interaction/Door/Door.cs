using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsLocked { get; private set; } = false;
    private bool isOpen = false;
    private bool isAnimating = false;

    [SerializeField] private float openRotation = 90f; // Degrees to rotate when opening
    [SerializeField] private float animationSpeed = 2f; // Speed of the door animation
    [SerializeField] private Transform pivot;

    private Quaternion closedRotation;
    private Quaternion openRotationTarget;

    private void Awake() {
        if (pivot == null) {
            Debug.LogError("Pivot reference is missing!");
        }
        closedRotation = pivot.rotation; // Use the pivot's rotation
        openRotationTarget = Quaternion.Euler(pivot.eulerAngles + new Vector3(0, 0, openRotation));
    }

    public void ToggleState() {
        if (isAnimating) return; // Do nothing if the door is animating
        if (isOpen) {
            StartCoroutine(AnimateDoor(closedRotation));
        }
        else {
            StartCoroutine(AnimateDoor(openRotationTarget));
        }
        isOpen = !isOpen;
    }

    private System.Collections.IEnumerator AnimateDoor(Quaternion targetRotation) {
        isAnimating = true;
        while (Quaternion.Angle(pivot.rotation, targetRotation) > 0.01f) {
            pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, Time.deltaTime * animationSpeed);
            yield return null;
        }
        pivot.rotation = targetRotation; // Snap to final rotation
        isAnimating = false;
    }
}