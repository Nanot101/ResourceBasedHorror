using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class MainLevelMusic : MonoBehaviour
{
    [SerializeField] private AudioSource mainLevelAudioSource;

    [FormerlySerializedAs("mainLevelDayClips")] [SerializeField]
    private List<MainLevelClipSO> dayClips = new();

    [FormerlySerializedAs("mainLevelNightClips")] [SerializeField]
    private List<MainLevelClipSO> nightClips = new();

    private MainLevelClipSO currentMainLevelClip;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Assert(mainLevelAudioSource != null,
            $"{nameof(mainLevelAudioSource)} is required for {nameof(MainLevelMusic)}", this);

        DayNightCounter.Instance.OnNewDay += OnNewDay;
        DayNightCounter.Instance.OnNewNight += OnNewNight;
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
        => ChangeCurrentClip(dayClips, args.NewDayNumber);

    private void OnNewNight(object sender, OnNewNightArgs args)
        => ChangeCurrentClip(nightClips, args.NewNightNumber);

    private void ChangeCurrentClip(IReadOnlyList<MainLevelClipSO> dayOrNightClips, int newDayOrNightNumber)
    {
        StopCurrentClip();

        currentMainLevelClip = dayOrNightClips.FirstOrDefault(x => x.DayOrNightNumber == newDayOrNightNumber);

        if (!currentMainLevelClip)
        {
            return;
        }

        PlayCurrentClip();

        if (currentMainLevelClip.FadeOutTime <= 0.0f)
        {
            return;
        }

        StartCoroutine(FadeCoroutine());
    }

    private void StopCurrentClip()
    {
        mainLevelAudioSource.Stop();
        StopAllCoroutines();
    }

    private void PlayCurrentClip()
    {
        mainLevelAudioSource.clip = currentMainLevelClip.AudioClip;
        mainLevelAudioSource.volume = 1.0f;
        mainLevelAudioSource.Play();
    }

    private IEnumerator FadeCoroutine()
    {
        var normalPlayDuration = DayNightSystem.Instance.CurrentPhase.Duration - currentMainLevelClip.FadeOutTime;

        yield return new WaitForSeconds(normalPlayDuration);

        var volumeStep = 1.0f / currentMainLevelClip.FadeOutTime;

        while (mainLevelAudioSource.volume > 0.0f)
        {
            mainLevelAudioSource.volume -= volumeStep * Time.deltaTime;

            yield return null;
        }
    }
}