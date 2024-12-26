using UnityEngine;

[CreateAssetMenu(fileName = "MyMainLevelMusicSO", menuName = "Main Level Music SO")]
public class MainLevelClipSO : ScriptableObject
{
    [field: SerializeField] public AudioClip AudioClip { get; set; }

    [field: SerializeField]
    [Tooltip("The time before the end of the day or night phase when the clip should fade")]
    public float FadeOutTime { get; set; } = 2.0f;

    [field: SerializeField]
    [Tooltip("The number of the day or night you want the clip to play")]
    public int DayOrNightNumber { get; set; }
}