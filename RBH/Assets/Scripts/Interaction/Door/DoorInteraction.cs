using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : InteractionBase
{
    [SerializeField]
    private Door door;

    private void Awake() {
        if (door == null) {
            Debug.LogError("Door reference is missing!");
        }
    }

    public override bool CanInteract(IInteractionCaller caller) {
        return door != null;
    }

    public override void Interact(IInteractionCaller caller) {
        Debug.Log("Interact triggered for door: " + door.name);
        if (door != null) {
            door.ToggleState();
        }
    }
}