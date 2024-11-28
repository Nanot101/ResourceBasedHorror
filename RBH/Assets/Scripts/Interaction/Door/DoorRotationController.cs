using UnityEngine;

public class DoorRotationController : MonoBehaviour
{
    public bool IsLocked { get; private set; } = false;

    public bool IsOpen => currentRotation && currentRotation.IsOpen;

    public bool CanToggleState => !IsLocked && (!currentRotation || !currentRotation.IsRotating);

    public DoorRotation TopLeftRotation { get; set; }

    public DoorRotation BottomLeftRotation { get; set; }

    private DoorRotation currentRotation;

    public void ToggleState(Vector3 callerPosition)
    {
        if (!CanToggleState)
        {
            return;
        }

        if (currentRotation && currentRotation.IsOpen)
        {
            currentRotation.CloseDoor();
            return;
        }

        SetCurrentRotation(callerPosition);

        if (currentRotation)
        {
            currentRotation.OpenDoor();
        }
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