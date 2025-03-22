using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all interactions on the scene
/// </summary>
public abstract class InteractionBase : MonoBehaviour
{
    public virtual bool CanInteract(IInteractionCaller caller) => true;

    public abstract void Interact(IInteractionCaller caller);
}

public class InteractionEventBase : InteractionBase
{
    public UnityEvent<IInteractionCaller> OnInteract;

    public override void Interact(IInteractionCaller caller)
    {
        OnInteract?.Invoke(caller);
    }
}
