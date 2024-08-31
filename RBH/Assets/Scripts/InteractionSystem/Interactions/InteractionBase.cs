using UnityEngine;

/// <summary>
/// Base class for all interactions on the scene
/// </summary>
public abstract class InteractionBase : MonoBehaviour
{
    public virtual bool CanInteract(IInteractionCaller caller) => true;

    public abstract void Interact(IInteractionCaller caller);
}
