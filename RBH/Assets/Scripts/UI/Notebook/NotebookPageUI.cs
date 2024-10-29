using UnityEngine;
using UnityEngine.UI;

public abstract class NotebookPageUI<TStore, TPage> : MonoBehaviour
    where TStore : NotebookPageStore<TPage>
    where TPage : NotebookPage
{
    [SerializeField]
    protected Button moveToPreviousBtn;

    [SerializeField]
    protected Button moveToNextBtn;

    [SerializeField]
    protected TStore pagesStore;

    protected int currentPageIndex = -1;

    protected virtual void Start()
    {
        AssertDesignerFields();

        SetButtons();
    }

    public virtual void Show(bool showFirstPage = false)
    {
        if (currentPageIndex == -1 && pagesStore.Count > 0)
        {
            currentPageIndex = 0;
        }
        else if (showFirstPage && currentPageIndex != -1)
        {
            currentPageIndex = 0;
        }

        UpdatePageUI();
    }

    public virtual void Hide()
    {
        moveToPreviousBtn.gameObject.SetActive(false);
        moveToNextBtn.gameObject.SetActive(false);
    }

    protected virtual void UpdatePageUI()
    {
        if (currentPageIndex == -1)
        {
            moveToPreviousBtn.gameObject.SetActive(false);
            moveToNextBtn.gameObject.SetActive(false);

            return;
        }

        if (currentPageIndex == pagesStore.Count - 1)
        {
            moveToNextBtn.gameObject.SetActive(false);
        }
        else
        {
            moveToNextBtn.gameObject.SetActive(true);
        }

        if (currentPageIndex == 0)
        {
            moveToPreviousBtn.gameObject.SetActive(false);
        }
        else
        {
            moveToPreviousBtn.gameObject.SetActive(true);
        }
    }

    private void MoveToPrevious()
    {
        if (pagesStore.Count == 0)
        {
            currentPageIndex = -1;
        }
        else if (currentPageIndex > 0)
        {
            currentPageIndex--;
        }
        else
        {
            return;
        }

        UpdatePageUI();
    }

    private void MoveToNext()
    {
        if (pagesStore.Count == 0)
        {
            currentPageIndex = -1;
        }
        else if (currentPageIndex < pagesStore.Count - 1)
        {
            currentPageIndex++;
        }
        else
        {
            return;
        }

        UpdatePageUI();
    }

    private void SetButtons()
    {
        moveToPreviousBtn.onClick.AddListener(MoveToPrevious);
        moveToNextBtn.onClick.AddListener(MoveToNext);
    }

    private void AssertDesignerFields()
    {
        Debug.Assert(moveToPreviousBtn != null, $"{nameof(moveToPreviousBtn)} is required for {gameObject.name}", this);
        Debug.Assert(moveToNextBtn != null, $"{nameof(moveToNextBtn)} is required for {gameObject.name}", this);

        Debug.Assert(pagesStore != null, $"{nameof(pagesStore)} is required for {gameObject.name}", this);
    }
}
