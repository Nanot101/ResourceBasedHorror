using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyStoryPage", menuName = "Notebook Page/Story Page")]
public class StoryPage : NotebookPage
{
    [field: SerializeField]
    public TextAsset DialogueScript { get; set; }

    [field: SerializeField]
    public List<Texture> IconList { get; set; } = new();
}
