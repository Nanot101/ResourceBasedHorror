using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorInteraction : MonoBehaviour, IInteractionCaller
{
    [SerializeField]
    private float interactionRange = 5f;
    [SerializeField]
    private LayerMask interactableLayer;

    public GameObject GameObject => gameObject;
    public StoryPageStore StoryPages { get; private set; } // Replace or remove if unnecessary
    public RecipePageStore RecipePages { get; private set; } // Replace or remove if unnecessary

    private Interactable focusedInteractable;

    private void Update()
    {
        DetectInteractable();

        if (Input.GetKeyDown(KeyCode.E) && focusedInteractable != null)
        {
            Debug.Log("Interacting with: " + focusedInteractable.name);
            focusedInteractable.Interaction?.Interact(this);
        }
        // else
        // {
        //     Debug.Log("No interactable in range.");
        // }
    }

    private void DetectInteractable()
    {
        // Get mouse position in world space
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the player to the mouse
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        // Optionally, add an offset to start the ray in front of the player (based on the player's facing direction)
        Vector2 rayStartPosition = (Vector2)transform.position + directionToMouse * 0.5f; // 0.5f can be adjusted based on your desired offset

        // Define a layer mask to exclude the player (make sure Player layer is assigned)
        int layerMask = ~LayerMask.GetMask("Player");

        // Perform raycast from the player's position (with offset) towards the mouse
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, directionToMouse, interactionRange, layerMask);

        // Debug the raycast line in the Scene view
        //Debug.DrawRay(rayStartPosition, directionToMouse * interactionRange, Color.red, 0.1f);

        if (hit.collider != null)
        {
            //Debug.Log("Raycast hit: " + hit.collider.name);

            // Check for the Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                //Debug.Log("Interactable detected: " + interactable.name);

                if (focusedInteractable != interactable)
                {
                    focusedInteractable?.HideUI(); // Hide previous UI
                    focusedInteractable = interactable;
                    focusedInteractable.ShowUI(); // Show new UI
                }
                return;
            }
        }

        // Hide UI if no interactable detected
        //Debug.Log("No interactable detected.");
        focusedInteractable?.HideUI();
        focusedInteractable = null;
    }

}