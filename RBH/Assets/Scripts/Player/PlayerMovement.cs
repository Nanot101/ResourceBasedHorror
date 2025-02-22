using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Player's walking speed. Must be greater than 0.")] [SerializeField]
    private float walkingSpeed = 5.0f;

    [Tooltip("Player's running speed. Must be greater than 0.")] [SerializeField]
    private float runningSpeed = 10.0f;

    [Tooltip("Player's speed when he has no stamina. Must be greater than 0.")] [SerializeField]
    private float exhaustedSpeed = 2.5f;

    [SerializeField] [Tooltip("Amount of stamina consumption per second while running. Must be greater than 0.")]
    private float runningStaminaConsumption = 40.0f;

    [SerializeField] private DayNightPhase stopMovementPhase;

    [SerializeField] private Animator _animator;

    #region Properties

    public float RunningSpeed
    {
        get { return runningSpeed; }
    }

    #endregion

    private PlayerStamina playerStamina;

    private Rigidbody2D rb;
    private float velocityY;
    private float velocityX;
    private Vector2 velocity;
    private bool wantsToRun;

    void Start()
    {
        AssertDesignerFileds();

        playerStamina = GetComponent<PlayerStamina>();
        rb = GetComponent<Rigidbody2D>();

        PlayerHealthSystem.onPlayerDied += StopMovement;
        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy()
    {
        PlayerHealthSystem.onPlayerDied -= StopMovement;

        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }
    }

    void Update()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        velocityX = InputManager.Instance.MoveInput.x;
        velocityY = InputManager.Instance.MoveInput.y;
        wantsToRun = InputManager.Instance.IsSprinting;

        if (GamePause.IsPaused)
        {
            velocityX = 0.0f;
            velocityY = 0.0f;
        }

        velocity = new Vector2(velocityX, velocityY);
        velocity.Normalize();

        if (velocity.magnitude > 0)
        {
            velocity *= CalculateSpeed();
        }

        rb.velocity = velocity;

        if (velocity.magnitude != 0 && playerStamina.HasStamina == true) {
            _animator.SetBool("isWalking", true);
            _animator.SetBool("isExhausted", false);
        }
        else if (velocity.magnitude != 0 && playerStamina.HasStamina == false) {
            _animator.SetBool("isWalking", true);
            _animator.SetBool("isExhausted", true);
        }
        else {
            _animator.SetBool("isWalking", false);
        }

        if (wantsToRun && playerStamina.HasStamina == true) {
            _animator.SetBool("isSprinting", true);
            _animator.SetBool("isExhausted", false);
        }
        else if (wantsToRun && playerStamina.HasStamina == false) {
            _animator.SetBool("isSprinting", true);
            _animator.SetBool("isExhausted", true);
        }
        else {
            _animator.SetBool("isSprinting", false);
        }


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

    private void AssertDesignerFileds()
    {
        Debug.Assert(walkingSpeed > 0.0f);
        Debug.Assert(runningSpeed > 0.0f);
        Debug.Assert(exhaustedSpeed > 0.0f);
        Debug.Assert(runningStaminaConsumption > 0.0f);
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == stopMovementPhase)
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        enabled = false;

        rb.velocity = Vector2.zero;
    }
}