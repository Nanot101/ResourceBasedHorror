using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFadeWhileInvincible : MonoBehaviour
{
    [Tooltip("A list of sprites that are supposed to fade in and out while the player is invincible.")]
    [SerializeField]
    private List<SpriteRenderer> spritesToFade;

    [Tooltip("How much time in seconds will it take to fade. Must be greater than 0.")]
    [SerializeField]
    private float fadeCycleTime = 0.2f;

    [SerializeField]
    private PlayerDamageReceiver playerDamageReciver;

    private Coroutine fadeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(fadeCycleTime > 0.0f, "Fade cycle time must be greater than 0");
        Debug.Assert(playerDamageReciver != null, "Player damage reciver must be set");

        playerDamageReciver.OnInvincibilityStarted += OnInvincibilityStarted;
        playerDamageReciver.OnInvincibilityEnded += OnInvincibilityEnded;
    }

    private void OnDestroy()
    {
        playerDamageReciver.OnInvincibilityStarted -= OnInvincibilityStarted;
        playerDamageReciver.OnInvincibilityEnded -= OnInvincibilityEnded;
    }

    private void OnInvincibilityStarted(object sender, EventArgs e)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeSprites());
    }

    private void OnInvincibilityEnded(object sender, EventArgs e)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);

            fadeCoroutine = null;
        }

        SetSpritesAlpha(1.0f);
    }

    private IEnumerator FadeSprites()
    {
        const float stepDelay = 0.05f;

        var stepValue = stepDelay / (fadeCycleTime / 2.0f);

        while (true)
        {
            yield return FadeInSprites(stepDelay, stepValue);

            yield return FadeOutSprites(stepDelay, stepValue);
        }
    }

    private IEnumerator FadeInSprites(float stepDelay, float stepValue)
    {
        var currentAlpha = 1.0f;

        while (currentAlpha > 0.0f)
        {
            currentAlpha -= stepValue;

            SetSpritesAlpha(currentAlpha);

            yield return new WaitForSeconds(stepDelay);
        }
    }

    private IEnumerator FadeOutSprites(float stepDelay, float stepValue)
    {
        var currentAlpha = 0.0f;

        while (currentAlpha < 1.0f)
        {
            currentAlpha += stepValue;

            SetSpritesAlpha(currentAlpha);

            yield return new WaitForSeconds(stepDelay);
        }
    }

    private void SetSpritesAlpha(float alpha)
    {
        foreach (var sprite in spritesToFade)
        {
            var spriteColor = sprite.color;

            spriteColor.a = alpha;

            sprite.color = spriteColor;
        }
    }
}
