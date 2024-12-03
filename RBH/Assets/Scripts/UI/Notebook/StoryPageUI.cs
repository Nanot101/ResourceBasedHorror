using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPageUI : NotebookPageUI<StoryPageStore, StoryPage>
{
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Image image;

    protected override void Start()
    {
        base.Start();

        Debug.Assert(text != null, $"{nameof(text)} is required for {gameObject.name}", this);
        Debug.Assert(image != null, $"{nameof(image)} is required for {gameObject.name}", this);
    }

    public override void Show(bool showFirstPage = false)
    {
        base.Show(showFirstPage);

        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();

        gameObject.SetActive(false);
    }

    protected override void UpdatePageUI()
    {
        base.UpdatePageUI();

        if (currentPageIndex == -1)
        {
            text.text = "You haven't collected any story pages yet.";
            image.sprite = null;
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

            return;
        }

        text.text = $"Story Page {pagesStore[currentPageIndex].PageNumber}";
        image.sprite = pagesStore[currentPageIndex].Sprite;
        image.color = Color.white;
    }
}
