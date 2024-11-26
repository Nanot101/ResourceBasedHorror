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
    public StoryPageStore StoryPages { get; private set; }
    public RecipePageStore RecipePages { get; private set; }

    private Interactable focusedInteractable;

    private void Update() {
        DetectInteractable();

        if (Input.GetKeyDown(KeyCode.E) && focusedInteractable != null) {
            Debug.Log("Interacting with: " + focusedInteractable.name);
            focusedInteractable.Interaction?.Interact(this);
        }
        // else {
        //     Debug.Log("No interactable in range.");
        // }
    }

    private void DetectInteractable() {
        // Get mouse position, calculate direction from player to mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        // Offsetting the raycast so it doesn't hit th player (0.5 can be changed, should probably make a ser field var for this)
        Vector2 rayStartPosition = (Vector2)transform.position + directionToMouse * 0.5f;

        // Raycast from player's offset to mouse
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, directionToMouse, interactionRange);
        //Debug.DrawRay(rayStartPosition, directionToMouse * interactionRange, Color.red, 0.1f);

        if (hit.collider != null) {
            //Debug.Log("Raycast hit: " + hit.collider.name);
            // Check for the Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null) {
                //Debug.Log("Interactable detected: " + interactable.name);
                if (focusedInteractable != interactable) {
                    focusedInteractable?.HideUI(); // Hide previous UI
                    focusedInteractable = interactable;
                    focusedInteractable.ShowUI(); // Show new UI
                }
                return;
            }
        }
        //Debug.Log("No interactable detected.");
        focusedInteractable?.HideUI();
        focusedInteractable = null;
    }
}