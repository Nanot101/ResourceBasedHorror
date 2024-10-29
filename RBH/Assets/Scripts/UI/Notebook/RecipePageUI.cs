using TMPro;
using UnityEngine;

public class RecipePageUI : NotebookPageUI<RecipePageStore, RecipePage>
{
    [SerializeField]
    private TextMeshProUGUI text;

    protected override void Start()
    {
        base.Start();

        Debug.Assert(text != null, $"{nameof(text)} is required for {gameObject.name}", this);
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
            text.text = "You haven't collected any recipe pages yet.";

            return;
        }

        text.text = $"Recipe Page {pagesStore[currentPageIndex].PageNumber}";
    }
}
