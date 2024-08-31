using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionLog : InteractionBase
{
    [SerializeField]
    private string text;

    public override void Interact(IInteractionCaller caller)
    {
        Debug.Log($"Interaction with text: \"{text}\" called by: {caller.GameObject.name} on: {gameObject.name}");
        //Destroy(gameObject);
    }
}
