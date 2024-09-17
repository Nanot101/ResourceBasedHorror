using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableUIPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectNameText;
    [SerializeField] private Image selectIconImage;
    [SerializeField] private Image selectBackgroundImage;
    [SerializeField] private Button selectButton;

    private int selectIndex;

    private Color selectDefaultBackgroundColor;

    private void Awake()
    {
        selectDefaultBackgroundColor = selectBackgroundImage.color;
    }
    public void Initialize(Selectables craft, int index,SelectSystem selectSystem)
    {
        selectIndex = index;
        selectNameText.text = craft.selection.itemData.itemName;
        if (craft.selection.itemData.itemIcon != null)
            selectIconImage.sprite = craft.selection.itemData.itemIcon;
        selectButton.onClick.AddListener(() => selectSystem.Select(selectIndex));
    }
    public void SelectQuest()
    {
        selectBackgroundImage.color = Color.yellow;
    }

    public void DeselectQuest()
    {
        selectBackgroundImage.color = selectDefaultBackgroundColor;
    }
}

