using InventorySystem;
using UnityEngine;

/// <summary>
/// Marker interface for all interaction callers
/// </summary>
public interface IInteractionCaller
{
    public GameObject GameObject { get; }

    public StoryPageStore StoryPages { get; }

    public RecipePageStore RecipePages { get; }

    public ContainerHandler InventoryContainer { get; }
}