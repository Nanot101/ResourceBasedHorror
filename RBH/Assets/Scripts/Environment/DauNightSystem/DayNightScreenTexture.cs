using UnityEngine;

public class DayNightScreenTexture : MonoBehaviour
{
    [SerializeField]
    private DayNightPhase clearTexturePhase;

    private float currentPhaseTime = 0.0f;
    private Texture2D currentTexture;

    // Start is called before the first frame update
    private void Start()
    {
        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy() => DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;

    private void Update()
    {
        if (DayNightSystem.Instance.CurrentPhase is not DayNightPhaseWithScreenTextureFade currentPhase)
        {
            return;
        }

        var currentPhaseDuration = currentPhase.Duration;

        if (currentPhaseTime >= currentPhaseDuration)
        {
            return;
        }

        currentPhaseTime += Time.deltaTime;

        switch (currentPhase.Direction)
        {
            case DayNightPhaseWithScreenTextureFade.FadeDirection.FadeIn:
                TextureFadeIn(currentPhaseDuration);
                break;

            case DayNightPhaseWithScreenTextureFade.FadeDirection.FadeOut:
                TextureFadeOut(currentPhaseDuration);
                break;
        }
    }

    private void OnGUI()
    {
        if (currentTexture == null)
        {
            return;
        }

        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), currentTexture);
    }

    private void TextureFadeIn(float phaseDuration)
    {
        var currentAlpha = currentPhaseTime / phaseDuration;

        SetTextureAlpha(currentAlpha);
    }

    private void TextureFadeOut(float phaseDuration)
    {
        var currentAlpha = 1.0f - (currentPhaseTime / phaseDuration);

        SetTextureAlpha(currentAlpha);
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase is DayNightPhaseWithScreenTextureFade phaseWithTextureFade)
        {
            var phaseTexture = phaseWithTextureFade.Texture;

            currentTexture = new Texture2D(phaseTexture.width, phaseTexture.height, phaseTexture.format, false);
            currentTexture.LoadRawTextureData(phaseTexture.GetRawTextureData());
            currentTexture.Apply();

            currentPhaseTime = 0.0f;

            if (phaseWithTextureFade.Direction == DayNightPhaseWithScreenTextureFade.FadeDirection.FadeIn)
            {
                SetTextureAlpha(0.0f);
            }
        }
        else if (currentPhase is DayNightPhaseWithScreenTexture phaseWithTexture)
        {
            currentTexture = phaseWithTexture.Texture;
        }
        else if (currentPhase == clearTexturePhase)
        {
            currentTexture = null;
        }
    }

    private void SetTextureAlpha(float alpha)
    {
        var texturePixels = currentTexture.GetPixels32();

        for (var i = 0; i < texturePixels.Length; i++)
        {
            var pixel = texturePixels[i];
            pixel.a = (byte)(alpha * byte.MaxValue);

            texturePixels[i] = pixel;
        }

        currentTexture.SetPixels32(texturePixels);
        currentTexture.Apply();
    }
}
