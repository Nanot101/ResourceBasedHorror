using System;
using UnityEngine;

public class DoorRotationController : MonoBehaviour
{
    [SerializeField] private DoorAudio doorAudio;

    public bool IsLocked { get; private set; } = false;

    public bool IsOpen => currentRotation && currentRotation.IsOpen;

    public bool CanToggleState => !IsLocked && (!currentRotation || !currentRotation.IsRotating);

    public DoorRotation TopLeftRotation { get; set; }

    public DoorRotation BottomLeftRotation { get; set; }

    private DoorRotation currentRotation;

    private void Start()
    {
        Debug.Assert(doorAudio != null, $"{nameof(doorAudio)} is required for {nameof(DoorRotationController)}",
            this);
    }

    public void ToggleState(Vector3 callerPosition)
    {
        if (!CanToggleState)
        {
            return;
        }

        if (currentRotation && currentRotation.IsOpen)
        {
            currentRotation.CloseDoor();
            doorAudio.PlayCloseAudio();
            return;
        }

        SetCurrentRotation(callerPosition);

        if (!currentRotation)
        {
            Debug.LogWarning("You should never be able to read me!");
            return;
        }

        currentRotation.OpenDoor();
        doorAudio.PlayOpenAudio();
    }

    private void SetCurrentRotation(Vector3 callerPosition)
    {
        var directionToCaller = callerPosition - transform.position;
        directionToCaller.Normalize();

        var doorUp = transform.up;

        var doorUpDirectionDot = Vector3.Dot(doorUp, directionToCaller);

        if (doorUpDirectionDot < 0)
        {
            currentRotation = TopLeftRotation;
            return;
        }

        currentRotation = BottomLeftRotation;
    }
}