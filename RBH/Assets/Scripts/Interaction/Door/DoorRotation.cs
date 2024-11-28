using System.Collections;
using UnityEngine;

public class DoorRotation : MonoBehaviour
{
    public Transform Door { get; set; }

    public float DoorOpenRotation { get; set; }

    public float DoorOpenSpeed { get; set; }

    public bool IsOpen { get; private set; } = false;

    public bool IsRotating { get; private set; } = false;

    public void OpenDoor()
    {
        StopAllCoroutines();
        StartCoroutine(RotateDoor(1.0f));

        IsOpen = true;
    }

    public void CloseDoor()
    {
        StopAllCoroutines();
        StartCoroutine(RotateDoor(-1.0f));

        IsOpen = false;
    }

    private IEnumerator RotateDoor(float speedScale)
    {
        var initialRotation = Door.rotation;
        var initialRelativePosition = Door.position - transform.position;
        var rotationDuration = Mathf.Abs(DoorOpenRotation / DoorOpenSpeed);
        var rotationTime = 0.0f;

        IsRotating = true;

        while (rotationTime < rotationDuration)
        {
            var t = rotationTime / rotationDuration;

            SetDoorPositionAndRotation(t);

            rotationTime += Time.deltaTime;
            yield return null;
        }

        SetDoorPositionAndRotation(1.0f);

        IsRotating = false;

        void SetDoorPositionAndRotation(float t)
        {
            var rotationOffset = Quaternion.AngleAxis(DoorOpenRotation * t * speedScale, Vector3.forward);

            Door.position = transform.position + rotationOffset * initialRelativePosition;
            Door.rotation = initialRotation * rotationOffset;
        }
    }
}