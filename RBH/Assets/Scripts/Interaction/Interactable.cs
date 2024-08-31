using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private InteractableUI UI;

    [SerializeField]
    private InteractionBase interaction;

    private IInteractionCaller currentCaller = null;

    // Start is called before the first frame update
    void Start()
    {
        if (UI == null)
        {
            Debug.LogError("UI is required for interactable");
            Destroy(this);
        }

        if (interaction == null)
        {
            Debug.LogError("Interaction is required for interactable");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (currentCaller == null)
        {
            UI.Hide();
            return;
        }

        if (!interaction.CanInteract(currentCaller))
        {
            UI.Hide();
            return;
        }

        UI.Show();

        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        interaction.Interact(currentCaller);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (currentCaller != null)
        {
            //Debug.Log("More than one caller is not supported.");
            return;
        }

        if (!collider.TryGetComponent<IInteractionCaller>(out var caller))
        {
            //Debug.Log("Collider doesn't have interaction caller");
            return;
        }

        currentCaller = caller;
        //Debug.Log("Current caller set");
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (currentCaller == null)
        {
            //Debug.Log("Current caller is null");
            return;
        }

        if (!collider.TryGetComponent<IInteractionCaller>(out var caller))
        {
            //Debug.Log("Collider doesn't have interaction caller");
            return;
        }

        if (caller != currentCaller)
        {
            //Debug.Log("Exited collider isn't current caller");
            return;
        }

        currentCaller = null;
        //Debug.Log("Current caller removed");
    }
}
