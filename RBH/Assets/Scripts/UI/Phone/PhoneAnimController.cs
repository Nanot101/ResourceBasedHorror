using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneAnimController : MonoBehaviour
{
    public GameObject animatorObject; // Drag the GameObject with the Animator component here
    public GameObject phonePanel; // Drag your UI panel here in the Inspector
    private Animator animator;
    private bool isOpen = false;
    private bool isAnimating = false; // Prevents spam input


    void Start()
    {
        if (animatorObject != null)
        {
            animator = animatorObject.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Animator object not assigned!");
        }
        if (phonePanel != null)
        {
            phonePanel.SetActive(false); // Ensure the panel starts hidden
        }
        else
        {
            Debug.LogError("Phone Panel not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && animator != null && !isAnimating)
        {
            isAnimating = true;
            isOpen = !isOpen;
            animator.SetBool("isOpen", isOpen);

            // If closing, disable the panel immediately
            if (!isOpen && phonePanel != null)
            {
                phonePanel.SetActive(false);
            }
        }
    }

    // Called automatically when an animation ends
    public void OnPhoneAnimationComplete()
    {
        if (isOpen && phonePanel != null)
        {
            phonePanel.SetActive(true);
        }
        isAnimating = false;
    }

    // This function is called when the "PhoneClose" animation ends
    public void OnPhoneCloseAnimationComplete()
    {
        isAnimating = false; // Allow pressing P again after closing
    }
}
