using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveUIWithCredits : MonoBehaviour
{
    public RectTransform uiImage;
    public float moveDistance = 320f;
    public float moveSpeed = 5f;
    public int moveCount = 1;
    public int maxMoves = 6;
    private Vector2 targetPosition;
    private Vector2 initialPosition;
    private bool isMoving = false;
    private bool resetting = false;
    private bool shouldDisplayFirstCredit = false;
    public GameObject mainPanel;
    public GameObject creditsPanel;

    public TMP_Text positionTitleText;
    public TMP_Text creditedPersonText;

    private List<string> positions = new List<string>();
    private Dictionary<string, List<string>> credits = new Dictionary<string, List<string>>();

    void Start()
    {
        initialPosition = uiImage.anchoredPosition;
        targetPosition = initialPosition;
        InitializeCredits();
        DisplayCredit(moveCount - 1);
    }

    void Update()
    {
        if (isMoving || resetting)
        {
            // Transition smoothly
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
                    moveCount = 1;
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
            DisplayCredit(0); // Display first credit
            shouldDisplayFirstCredit = false;
        }
    }

    public void MoveImage()
    {
        if (moveCount < maxMoves && !isMoving)
        {
            targetPosition += new Vector2(-moveDistance, 0);
            isMoving = true;
            moveCount++;
            // Display the credit only if it's not at the last position
            if (moveCount <= maxMoves)
            {
                DisplayCredit(moveCount - 1);
            }
        }
        else if (moveCount >= maxMoves && !resetting)
        {
            resetting = true; 
            targetPosition = initialPosition; // Reset the image position
            positionTitleText.text = ""; // Clear texts
            creditedPersonText.text = "";
        }
    }

    private void InitializeCredits()
    {
        positions.Add("Programmers");
        positions.Add("Artists");
        positions.Add("Composers");
        positions.Add("Designers");
        positions.Add("Writers");
        positions.Add("Special Thanks To");
        

        credits["Programmers"] = new List<string> { "Artur", "Obleynix" };
        credits["Artists"] = new List<string> { "Les", "Mary", "Cloudy" };
        credits["Composers"] = new List<string> { "Aradia (Director)", "NarcolepsyDriver (Music)", "Donovan 'DannyGoldstar' Bautista (SFX)" };
        credits["Designers"] = new List<string> { "Chris", "Loon" };
        credits["Writers"] = new List<string> { "Mary", "Chris" };
        credits["Special Thanks To"] = new List<string> { "DriplessAtoms" };




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