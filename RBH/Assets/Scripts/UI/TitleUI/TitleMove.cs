using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUIWithResetAndDisable : MonoBehaviour
{
    public RectTransform uiImage;  
    public float moveDistance = 320f;  // Distance to move (pixels)
    public float moveSpeed = 5f;  // Speed at which the image will move
    private int moveCount = 0;  // Counter to track the number of times the image has been moved
    public int maxMoves = 6;    
    private Vector2 targetPosition;  // The target position to move to
    private Vector2 initialPosition;  // Store the initial position of the image
    private bool isMoving = false;  
    private bool resetting = false;  // Flag to track if the UI is resetting back to the initial position
    public GameObject mainPanel;  
    public GameObject creditsPanel;

    void Start()
    {
        // Store the initial position of the UI image
        initialPosition = uiImage.anchoredPosition;
        // Set the initial target position to the current position
        targetPosition = initialPosition;
    }

    void Update()
    {
        if (isMoving || resetting)
        {
            // Move smoothly towards the target position using Lerp
            uiImage.anchoredPosition = Vector2.Lerp(uiImage.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed * 3);

            // Stop moving when the image reaches the target position
            if (Vector2.Distance(uiImage.anchoredPosition, targetPosition) < 0.1f)
            {
                uiImage.anchoredPosition = targetPosition;  // Snap exactly to the target to avoid overshooting
                if (resetting)
                {
                    resetting = false;
                    mainPanel.SetActive(true);
                    creditsPanel.SetActive(false);
                    moveCount = 0;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
    }

    // Public method to move the UI Image
    public void MoveImage()
    {
        if (moveCount < maxMoves && !isMoving)
        {
            // Set the new target position to move 320 units to the left
            targetPosition += new Vector2(-moveDistance, 0);
            isMoving = true;  // Start moving
            moveCount++;  // Increment the counter
        }
        else if (moveCount >= maxMoves) //&& !resetting)
        {
            // Debug.Log("Move limit reached. Resetting position and disabling GameObject.");
            resetting = true;  // Set the resetting flag to true to trigger smooth reset
            targetPosition = initialPosition;
        }

    }
}