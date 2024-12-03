using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayNightTransitionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI transitionText;

    [SerializeField]
    private Image background;

    [Tooltip("How long in seconds UI fades in. It must be greater than zero")]
    [SerializeField]
    private float fadeInTime = 2.0f;

    [Tooltip("How long in seconds UI stays after fade in. It must be greater than zero")]
    [SerializeField]
    private float stayTime = 1.0f;

    [Tooltip("How long in seconds UI fades out after stay. It must be greater than zero")]
    [SerializeField]
    private float fadeOutTime = 2.0f;

    [SerializeField]
    private string dayText = "Day";

    [SerializeField]
    private string nightText = "Night";

    // Start is called before the first frame update
    void Awake()
    {
        if (transitionText == null)
        {
            Debug.LogError("Transition text is required for day night transition UI");
            Destroy(this);
        }

        if (background == null)
        {
            Debug.LogError("Background is required for day night transition UI");
            Destroy(this);
        }

        if (DayNightCounter.Instance == null)
        {
            Debug.LogError("Single instance of day night counter is required for day night transition UI");
            Destroy(this);
        }

        DayNightCounter.Instance.OnNewDay += OnNewDay;
        DayNightCounter.Instance.OnNewNight += OnNewNight;

        AssertDesignerFileds();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (DayNightCounter.TryGetInstance(out var dayNightCounter))
        {
            dayNightCounter.OnNewDay -= OnNewDay;
            dayNightCounter.OnNewNight -= OnNewNight;
        }
    }

    private void OnNewDay(object sender, OnNewDayArgs args)
    {
        var text = $"{dayText} {args.NewDayNumber}";

        ShowDayNightText(text);
    }

    private void OnNewNight(object sender, OnNewNightArgs args)
    {
        var text = $"{nightText} {args.NewNightNumber}";

        ShowDayNightText(text);
    }

    private void ShowDayNightText(string transitionText)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);

        this.transitionText.text = transitionText;

        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        //Debug.Log("Day night UI fade start");

        yield return FadeIn();

        yield return Stay();

        yield return FadeOut();

        gameObject.SetActive(false);

        //Debug.Log("Day night UI fade stop");
    }

    private IEnumerator Stay()
    {
        SetAlpha(1.0f);

        yield return new WaitForSeconds(stayTime);
    }

    private IEnumerator FadeIn()
    {
        const float stepDelay = 0.05f;

        var stepValue = stepDelay / fadeInTime;
        var currentAlpha = 0.0f;

        while (currentAlpha < 1.0f)
        {
            currentAlpha += stepValue;

            SetAlpha(currentAlpha);

            yield return new WaitForSeconds(stepDelay);
        }
    }

    private IEnumerator FadeOut()
    {
        const float stepDelay = 0.1f;

        var stepValue = stepDelay / fadeOutTime;
        var currentAlpha = 1.0f;

        while (currentAlpha > 0.0f)
        {
            currentAlpha -= stepValue;

            SetAlpha(currentAlpha);

            yield return new WaitForSeconds(stepDelay);
        }
    }

    private void SetAlpha(float alpha)
    {
        var backgroundColor = background.color;
        backgroundColor.a = alpha;
        background.color = backgroundColor;

        var textColor = transitionText.color;
        textColor.a = alpha;
        transitionText.color = textColor;
    }

    private void AssertDesignerFileds()
    {
        Debug.Assert(fadeInTime > 0.0f, "Fade in time must be greater than 0");
        Debug.Assert(stayTime > 0.0f, "Stay time must be greater than 0");
        Debug.Assert(stayTime > 0.0f, "Fade out time must be greater than 0");
    }
}
