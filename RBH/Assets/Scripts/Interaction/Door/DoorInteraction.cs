using UnityEngine;
using UnityEngine.Serialization;

public class DoorInteraction : InteractionBase
{
    [SerializeField] private DoorRotationController doorRotationController;

    [SerializeField] private bool enemyLocked = false;

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

        if (caller is PlayerInteractionCaller playerCaller)
        {
            doorRotationController.ToggleState(playerCaller.GameObject.transform.position);
            return;
        }

        //TODO: Uncomment when enemy will have proper navigation sys
        // if (enemyCanInteract && caller is EnemyInteractionCaller enemyCaller)
        // {
        //     doorRotationController.ToggleState(enemyCaller.GameObject.transform.position, true);
        //     return;
        // }
    }
}