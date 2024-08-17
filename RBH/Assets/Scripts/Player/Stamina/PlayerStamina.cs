using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Tooltip("Maximum amount of stamina available to the player. Must be greater than 0.")]
    [SerializeField]
    private float maxStamina = 100.0f;

    [Tooltip("How much stamina is recovered per second. Must be greater than 0.")]
    [SerializeField]
    private float recoveryRateNormal = 30.0f;

    [Tooltip("Delay in seconds between stamina consumption and recovery. Must be greater than 0.")]
    [SerializeField]
    private float recoveryDelayNormal = 0.1f;

    [Tooltip("How much stamina is recovered per second when consumption has depleted stamina. Must be greater than 0.")]
    [SerializeField]
    private float recoveryRateOnDepleted = 50.0f;

    [Tooltip("Delay in seconds between stamina consumption and recovery when consumption has depleted stamina. Must be greater than 0.")]
    [SerializeField]
    private float recoveryDelayOnDepleted = 5.0f;

    [Tooltip("At what amount of stamina in percentage when it is recovered from depleted change the recovery rate to normal. " +
        "Must be between 0 and 1, where 1 is 100%")]
    [SerializeField]
    private float recoverySwitchValue = 0.5f;

    [Tooltip("At what amount of stamina in percentage when it is recovered from depleted it is possible to start consumption. " +
        "Must be between 0 and 1, where 1 is 100%")]
    [SerializeField]
    private float minStaminaAvailable = 0.2f;

    private float currentRecoveryDelay = 0.0f;
    private bool recoveryFromDepleated;
    private bool canBeConsumedFormDepleated;

    public float CurrentStamina { get; private set; }

    public float CurrentStaminaNormalized => CurrentStamina / maxStamina;

    public bool HasStamina => !Mathf.Approximately(CurrentStamina, 0.0f);

    public bool HasMaxStamina => Mathf.Approximately(CurrentStamina, maxStamina);

    // Start is called before the first frame update
    private void Start()
    {
        AssertDesinerFileds();

        CurrentStamina = maxStamina;
        canBeConsumedFormDepleated = true;
    }

    // Update is called once per frame
    private void Update()
    {
        DebugHandleInput();

        HandleRecovery();

        DebugDisplayValues();
    }

    /// <summary>
    /// Tries to consume the given amount of stamina.
    /// </summary>
    /// <param name="staminaAmount">Amount of stamina to consume. Must be greater than 0.</param>
    /// <returns><see langword="true"/> if the specified amount of stamina was successfully consumed, <see langword="false"/> otherwise.</returns>
    public bool TryConsumeExact(float staminaAmount)
    {
        Debug.Assert(staminaAmount > 0.0f);

        if (CurrentStamina < staminaAmount || !CanBeConsumed())
        {
            return false;
        }

        RemoveStamina(staminaAmount);

        return true;
    }

    /// <summary>
    /// Consume an approximate amount of stamina.
    /// </summary>
    /// <remarks>
    /// If the amount of stamina is less than the amount to be consumed, it will consume all the stamina and return how much was actually consumed.
    /// </remarks>
    /// <param name="staminaAmount">Amount of stamina to consume. Must be greater than 0.</param>
    /// <returns>The actual amount of stamina consumed.</returns>
    public float ConsumeApproximate(float staminaAmount)
    {
        Debug.Assert(staminaAmount > 0.0f);

        if (!CanBeConsumed())
        {
            return 0.0f;
        }

        var consumedStamina = Mathf.Min(staminaAmount, CurrentStamina);

        RemoveStamina(consumedStamina);

        return consumedStamina;
    }

    /// <summary>
    /// Checks whether stamina can be consumed in the current state.
    /// </summary>
    /// <returns><see langword="true"/> if stamina can be consumed, <see langword="false"/> otherwise.</returns>
    public bool CanBeConsumed()
    {
        if (!HasStamina)
        {
            return false;
        }

        if (!canBeConsumedFormDepleated)
        {
            return false;
        }

        return true;
    }

    private void RemoveStamina(float amount)
    {
        CurrentStamina -= amount;

        if (!HasStamina)
        {
            currentRecoveryDelay = recoveryDelayOnDepleted;
            recoveryFromDepleated = true;
            canBeConsumedFormDepleated = false;
            return;
        }

        currentRecoveryDelay = recoveryDelayNormal;
    }

    private void HandleRecovery()
    {
        if (currentRecoveryDelay > 0)
        {
            currentRecoveryDelay -= Time.deltaTime;
            return;
        }

        var recoveryRate = CalculateRecoveryRate();

        CurrentStamina = Mathf.Min(CurrentStamina + recoveryRate * Time.deltaTime, maxStamina);

        if (recoveryFromDepleated && CurrentStaminaNormalized > minStaminaAvailable)
        {
            canBeConsumedFormDepleated = true;
        }
    }

    private float CalculateRecoveryRate()
    {
        if (!recoveryFromDepleated)
        {
            return recoveryRateNormal;
        }

        if (CurrentStaminaNormalized >= recoverySwitchValue)
        {
            recoveryFromDepleated = false;

            return recoveryRateNormal;
        }

        return recoveryRateOnDepleted;
    }

    private void AssertDesinerFileds()
    {
        Debug.Assert(maxStamina > 0.0f);
        Debug.Assert(recoveryRateNormal > 0.0f);
        Debug.Assert(recoveryDelayNormal > 0.0f);
        Debug.Assert(recoveryRateOnDepleted > 0.0f);
        Debug.Assert(recoverySwitchValue > 0.0f && recoverySwitchValue <= 1.0f);
        Debug.Assert(minStaminaAvailable > 0.0f && minStaminaAvailable <= 1.0f);
    }

    private void DebugHandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ConsumeApproximate(17.0f);
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            TryConsumeExact(17.0f);
            return;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ConsumeApproximate(maxStamina);
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CurrentStamina = maxStamina;
            currentRecoveryDelay = 0.0f;
            recoveryFromDepleated = false;
        }
    }

    private void DebugDisplayValues()
    {
        Debug.Log($"stamina: {CurrentStamina}");
        Debug.Log($"recovery delay: {currentRecoveryDelay}");
        Debug.Log($"recovery from depleated: {recoveryFromDepleated}");
        Debug.Log($"can be consumed form depleated: {canBeConsumedFormDepleated}");
    }
}
