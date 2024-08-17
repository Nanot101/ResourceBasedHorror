using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Player's walking speed. Must be greater than 0.")]
    [SerializeField]
    private float walkingSpeed = 5.0f;

    [Tooltip("Player's running speed. Must be greater than 0.")]
    [SerializeField]
    private float runningSpeed = 10.0f;

    [Tooltip("Player's speed when he has no stamina. Must be greater than 0.")]
    [SerializeField]
    private float exhaustedSpeed = 2.5f;

    [SerializeField]
    [Tooltip("Amount of stamina consumption per second while running. Must be greater than 0.")]
    private float runningStaminaConsumption = 40.0f;

    private PlayerStamina playerStamina;

    private Rigidbody2D rb;
    private float velocityY;
    private float velocityX;
    private Vector2 velocity;
    private bool wantsToRun;

    void Start()
    {
        AssertDesinerFileds();

        playerStamina = GetComponent<PlayerStamina>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        PlayerMove();
    }
    private void PlayerMove()
    {
        velocityX = Input.GetAxisRaw("Horizontal");
        velocityY = Input.GetAxisRaw("Vertical");
        wantsToRun = Input.GetKey(KeyCode.LeftShift);

        velocity = new Vector2(velocityX, velocityY);
        velocity.Normalize();

        if (velocity.magnitude > 0)
        {
            velocity *= CalculateSpeed();
        }

        rb.velocity = velocity;
    }

    private float CalculateSpeed()
    {
        if (!playerStamina.HasStamina)
        {
            return exhaustedSpeed;
        }

        if (wantsToRun)
        {
            var staminaToConsume = runningStaminaConsumption * Time.deltaTime;

            if (playerStamina.ConsumeApproximate(staminaToConsume) > 0.0f)
            {
                return runningSpeed;
            }
        }

        return walkingSpeed;
    }

    private void AssertDesinerFileds()
    {
        Debug.Assert(walkingSpeed > 0.0f);
        Debug.Assert(runningSpeed > 0.0f);
        Debug.Assert(exhaustedSpeed > 0.0f);
        Debug.Assert(runningStaminaConsumption > 0.0f);
    }
}
