// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class MoveUIWithResetAndDisable : MonoBehaviour
// {
//     public RectTransform uiImage;  
//     public float moveDistance = 320f;  // Distance to move (pixels)
//     public float moveSpeed = 5f;  // Speed at which the image will move
//     private int moveCount = 0;  // Counter to track the number of times the image has been moved
//     public int maxMoves = 6;    
//     private Vector2 targetPosition;  // The target position to move to
//     private Vector2 initialPosition;  // Store the initial position of the image
//     private bool isMoving = false;  
//     private bool resetting = false;  // Flag to track if the UI is resetting back to the initial position
//     public GameObject mainPanel;  
//     public GameObject creditsPanel;

//     void Start()
//     {
//         // Store the initial position of the UI image
//         initialPosition = uiImage.anchoredPosition;
//         // Set the initial target position to the current position
//         targetPosition = initialPosition;
//     }

//     void Update()
//     {
//         if (isMoving || resetting)
//         {
//             // Move smoothly towards the target position using Lerp
//             uiImage.anchoredPosition = Vector2.Lerp(uiImage.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed * 3);

//             // Stop moving when the image reaches the target position
//             if (Vector2.Distance(uiImage.anchoredPosition, targetPosition) < 0.1f)
//             {
//                 uiImage.anchoredPosition = targetPosition;  // Snap exactly to the target to avoid overshooting
//                 if (resetting)
//                 {
//                     resetting = false;
//                     mainPanel.SetActive(true);
//                     creditsPanel.SetActive(false);
//                     moveCount = 0;
//                 }
//                 else
//                 {
//                     isMoving = false;
//                 }
//             }
//         }
//     }

//     // Public method to move the UI Image
//     public void MoveImage()
//     {
//         if (moveCount < maxMoves && !isMoving)
//         {
//             // Set the new target position to move 320 units to the left
//             targetPosition += new Vector2(-moveDistance, 0);
//             isMoving = true;  // Start moving
//             moveCount++;  // Increment the counter
//         }
//         else if (moveCount >= maxMoves) //&& !resetting)
//         {
//             // Debug.Log("Move limit reached. Resetting position and disabling GameObject.");
//             resetting = true;  // Set the resetting flag to true to trigger smooth reset
//             targetPosition = initialPosition;
//         }

//     }
// }


using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveUIWithCredits : MonoBehaviour
{
    public RectTransform uiImage;
    public float moveDistance = 320f; // Distance to move (pixels)
    public float moveSpeed = 5f; // Speed at which the image will move
    public int moveCount = 1; // Counter to track the number of times the image has been moved
    public int maxMoves = 6;
    private Vector2 targetPosition; // The target position to move to
    private Vector2 initialPosition; // Store the initial position of the image
    private bool isMoving = false;
    private bool resetting = false; // Flag to track if the UI is resetting back to the initial position
    private bool shouldDisplayFirstCredit = false; // Flag to control when the first credit is displayed after reset
    public GameObject mainPanel;
    public GameObject creditsPanel;

    public TMP_Text positionTitleText; // TextMeshPro element for the position title
    public TMP_Text creditedPersonText; // TextMeshPro element for the credited persons

    private List<string> positions = new List<string>();
    private Dictionary<string, List<string>> credits = new Dictionary<string, List<string>>();

    void Start()
    {
        // Store the initial position of the UI image
        initialPosition = uiImage.anchoredPosition;
        // Set the initial target position to the current position
        targetPosition = initialPosition;

        // Initialize credits
        InitializeCredits();

        // Display the first credit
        DisplayCredit(moveCount - 1); // Adjust the index to 0-based indexing
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
                uiImage.anchoredPosition = targetPosition; // Snap exactly to the target to avoid overshooting
                if (resetting)
                {
                    resetting = false;
                    mainPanel.SetActive(true);
                    creditsPanel.SetActive(false);
                    moveCount = 1; // Reset moveCount to start at 1 for the next time credits are shown

                    // After reset, wait briefly before showing the first credit again
                    shouldDisplayFirstCredit = true;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
        else if (shouldDisplayFirstCredit)
        {
            // This will ensure we display the first credit after a brief reset delay
            DisplayCredit(0);
            shouldDisplayFirstCredit = false;
        }
    }

    public void MoveImage()
    {
        if (moveCount < maxMoves && !isMoving)
        {
            // Set the new target position to move 320 units to the left
            targetPosition += new Vector2(-moveDistance, 0);
            isMoving = true; // Start moving
            moveCount++; // Increment the counter

            // Display the credit only if we're not at the last position
            if (moveCount <= maxMoves) // Ensure we only display credits up to the last one
            {
                DisplayCredit(moveCount - 1); // Adjust the index to 0-based indexing
            }
        }
        else if (moveCount >= maxMoves && !resetting)
        {
            // Once the last credit is shown, we set resetting to true to reset the UI
            resetting = true; 
            targetPosition = initialPosition; // Reset the image position

            // Clear the text temporarily before resetting
            positionTitleText.text = "";
            creditedPersonText.text = "";
        }
    }

    private void InitializeCredits()
    {
        // Example data
        positions.Add("Programmer");
        positions.Add("Designer");
        positions.Add("Composer");

        credits["Programmer"] = new List<string> { "Person A" };
        credits["Designer"] = new List<string> { "Person B", "Person C" };
        credits["Composer"] = new List<string> { "Person R" };

        maxMoves = positions.Count; // Adjust max moves to match the number of credits
    }

    private void DisplayCredit(int index)
    {
        if (index >= 0 && index < positions.Count)
        {
            string position = positions[index];
            positionTitleText.text = position;

            if (credits.ContainsKey(position))
            {
                creditedPersonText.text = string.Join("\n", credits[position]);
            }
            else
            {
                creditedPersonText.text = "No one credited.";
            }
        }
        else
        {
            // If index is out of bounds, reset the texts
            positionTitleText.text = "";
            creditedPersonText.text = "";
        }
    }
}