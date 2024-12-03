using System;
using UnityEngine;

public class DoorUIPositionToggle : MonoBehaviour
{
    [SerializeField] private Transform doorUI;

    private Vector3 uiPositionSave;
    private Transform interactionCallerTransform;

    private void Start()
    {
        Debug.Assert(doorUI,
            $"{nameof(doorUI)} is required for {gameObject.name} {nameof(DoorRotationSetup)}", this);

        uiPositionSave = doorUI.localPosition;
    }

    private void LateUpdate()
    {
        if (!interactionCallerTransform)
        {
            return;
        }

        var directionToCaller = interactionCallerTransform.position - transform.position;
        directionToCaller.Normalize();

        var doorUp = transform.up;

        var doorUpDirectionDot = Vector3.Dot(doorUp, directionToCaller);

        if (doorUpDirectionDot > 0)
        {
            doorUI.localPosition = uiPositionSave;
            return;
        }

        var flippedPosition = uiPositionSave;
        flippedPosition.y *= -1.0f;

        doorUI.localPosition = flippedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.TryGetComponent<IInteractionCaller>(out var interactionCaller))
        {
            return;
        }

        interactionCallerTransform = interactionCaller.GameObject.transform;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.TryGetComponent<IInteractionCaller>(out var interactionCaller))
        {
            return;
        }

        if (interactionCallerTransform != interactionCaller.GameObject.transform)
        {
            return;
        }

        interactionCallerTransform = null;
    }
}