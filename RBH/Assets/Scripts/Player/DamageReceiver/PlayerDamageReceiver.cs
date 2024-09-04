using System;
using System.Collections;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [SerializeField]
    private PlayerHealthSystem healthSystem;

    [Tooltip("How long in seconds the player is invincible after receiving damage. Must be greater than 0.")]
    [SerializeField]
    private float invincibilityTime = 2.0f;

    private Coroutine invincibilityCoroutine;

    private bool playerDied = false;

    public event EventHandler OnInvincibilityStarted;

    public event EventHandler OnInvincibilityEnded;

    public bool IsInvincible { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(invincibilityTime > 0.0f, "Invincibility time must be greater than 0");

        if (healthSystem == null)
        {
            Debug.LogError("Health system is required for player damage reciver.");
            Destroy(this);
        }

        PlayerHealthSystem.onPlayerDied += OnPlayerDied;
    }

    /// <summary>
    /// Tries to deal damage to the player.
    /// </summary>
    /// <remarks>
    /// If the player is invincible then the method will return <see langword="false"/>.
    /// </remarks>
    /// <param name="damageAmount">Amount of damage</param>
    /// <returns><see langword="true"/> if damage was dealt, <see langword="false"/> otherwise.</returns>
    public bool TryDealDamage(float damageAmount)
    {
        Debug.Assert(damageAmount > 0.0f);

        if (IsInvincible || playerDied)
        {
            //Debug.Log("Player is invincible");
            return false;
        }

        healthSystem.TakeDamage(damageAmount);

        if (!playerDied)
        {
            StartInvincibility();
        }

        return true;
    }

    private void StartInvincibility()
    {
        //Debug.Log("Invincibility started");

        IsInvincible = true;

        OnInvincibilityStarted?.Invoke(this, EventArgs.Empty);

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);

        EndInvincibility();
    }

    private void EndInvincibility()
    {
        //Debug.Log("Invincibility ended");

        IsInvincible = false;

        OnInvincibilityEnded?.Invoke(this, EventArgs.Empty);

        invincibilityCoroutine = null;
    }

    public void OnPlayerRespawned()
    {
        playerDied = false;

        if (!IsInvincible)
        {
            return;
        }

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        EndInvincibility();
    }

    private void OnPlayerDied() => playerDied = true;
}
