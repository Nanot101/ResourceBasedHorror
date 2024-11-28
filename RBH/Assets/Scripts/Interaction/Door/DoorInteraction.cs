using UnityEngine;
using UnityEngine.Serialization;

public class DoorInteraction : InteractionBase
{
    [SerializeField]
    private DoorRotationController doorRotationController;

    private void Start()
    {
        Debug.Assert(doorRotationController,
            $"{nameof(doorRotationController)} reference is missing!", this);
    }

    public override bool CanInteract(IInteractionCaller caller) => doorRotationController.CanToggleState;

    public override void Interact(IInteractionCaller caller)
    {
        if (!doorRotationController.CanToggleState)
        {
            return;
        }

        // Debug.Log($"Interact triggered for door: {doorRotationController.name}");

        doorRotationController.ToggleState(caller.GameObject.transform.position);
    }
}