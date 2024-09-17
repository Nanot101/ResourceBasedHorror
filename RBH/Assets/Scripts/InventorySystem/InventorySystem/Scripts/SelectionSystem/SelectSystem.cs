using System;
using UnityEngine;

public class SelectSystem : MonoBehaviour
{
    [SerializeField] private Selectables[] selectable;
    GameObject[] selectedUIPrefabInstances;
    [SerializeField] private GameObject selectedUIPrefab;
    [SerializeField] private Transform selectUIParent;
    public int selectedIndex;

    public static Action<Selectables,SelectSystem> OnSelection;
    public static Action OnOpened;

    private void Awake()
    {
        InitializeSelection();
        gameObject.SetActive(false);
    }

    private void InitializeSelection()
    {
        selectedIndex = 0;
        selectedUIPrefabInstances = new GameObject[selectable.Length];
        for (int i = 0; i < selectable.Length; i++)
        {
            selectedUIPrefabInstances[i] = Instantiate(selectedUIPrefab, selectUIParent);
            selectedUIPrefabInstances[i].GetComponent<SelectableUIPrefab>().Initialize(selectable[i], i, this);
        }
    }

    public void OpenSystem()
    {
        OnOpened?.Invoke();
        gameObject.SetActive(true);
    }

    public void Select(int index)
    {
        selectedUIPrefabInstances[selectedIndex].GetComponent<SelectableUIPrefab>().DeselectQuest();
        selectedIndex = index;
        selectedUIPrefabInstances[selectedIndex].GetComponent<SelectableUIPrefab>().SelectQuest();
        OnSelection?.Invoke(selectable[selectedIndex],this);
    }

    
    public Selectables GetCurrentSelected()
    {
        return selectable[selectedIndex];
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    
}

[Serializable]
public class Selectables
{
    public SelectableItem selection;
}


public abstract class SelectableItem : ScriptableObject
{
    public ItemData itemData;
   
}

[Serializable]
public class CraftingRecipe
{
    public CraftingIngredients[] ingredients;
}
[Serializable]
public class CraftingIngredients
{
    public ItemData item;
    public int amount;
}

