using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    [SerializeField]
    private Button moveToStoryBtn;

    [SerializeField]
    private Button moveToRecipeBtn;

    [SerializeField]
    private StoryPageUI storyPageUI;

    [SerializeField]
    private RecipePageUI recipePageUI;

    private bool storyPageUISelected = true;

    private bool noteBookSelected = false;

    private void Start()
    {
        AssertDesignerFields();

        SetButtons();

        MoveToStory();

        HideAll();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.N))
        {
            return;
        }

        if (noteBookSelected)
        {
            HideAll();

            return;
        }

        ShowSelectedPages();
    }

    private void MoveToStory()
    {
        moveToStoryBtn.gameObject.SetActive(false);
        moveToRecipeBtn.gameObject.SetActive(true);

        storyPageUI.Show(true);
        recipePageUI.Hide();

        storyPageUISelected = true;
    }

    private void MoveToRecipe()
    {
        moveToStoryBtn.gameObject.SetActive(true);
        moveToRecipeBtn.gameObject.SetActive(false);

        storyPageUI.Hide();
        recipePageUI.Show(true);

        storyPageUISelected = false;
    }

    private void ShowSelectedPages()
    {
        if (storyPageUISelected)
        {
            moveToStoryBtn.gameObject.SetActive(false);
            moveToRecipeBtn.gameObject.SetActive(true);

            storyPageUI.Show();
            recipePageUI.Hide();
        }
        else
        {
            moveToStoryBtn.gameObject.SetActive(true);
            moveToRecipeBtn.gameObject.SetActive(false);

            storyPageUI.Hide();
            recipePageUI.Show();
        }

        noteBookSelected = true;
    }

    private void HideAll()
    {
        moveToStoryBtn.gameObject.SetActive(false);
        moveToRecipeBtn.gameObject.SetActive(false);

        storyPageUI.Hide();
        recipePageUI.Hide();

        noteBookSelected = false;
    }

    private void SetButtons()
    {
        moveToStoryBtn.onClick.AddListener(MoveToStory);
        moveToRecipeBtn.onClick.AddListener(MoveToRecipe);
    }

    private void AssertDesignerFields()
    {
        Debug.Assert(moveToStoryBtn != null, $"{nameof(moveToStoryBtn)} is required for {gameObject.name}", this);
        Debug.Assert(moveToRecipeBtn != null, $"{nameof(moveToRecipeBtn)} is required for {gameObject.name}", this);

        Debug.Assert(storyPageUI != null, $"{nameof(storyPageUI)} is required for {gameObject.name}", this);
        Debug.Assert(recipePageUI != null, $"{nameof(recipePageUI)} is required for {gameObject.name}", this);
    }
}
