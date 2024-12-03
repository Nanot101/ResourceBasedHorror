using System;
using UnityEngine;

public abstract class NotebookPage : ScriptableObject, IComparable<NotebookPage>, IEquatable<NotebookPage>
{
    [field: SerializeField]
    public int PageNumber { get; set; }

    [field: SerializeField]
    public Sprite Sprite { get; set; }

    public int CompareTo(NotebookPage other) => PageNumber.CompareTo(other.PageNumber);

    public bool Equals(NotebookPage other) => PageNumber.Equals(other.PageNumber);

    public override bool Equals(object other)
    {
        if (other is not NotebookPage)
        {
            return false;
        }

        return Equals((NotebookPage)other);
    }

    public override int GetHashCode() => PageNumber.GetHashCode();
}
