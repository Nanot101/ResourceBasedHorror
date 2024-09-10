using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyDropSo : ScriptableObject
{
    [field: SerializeField]
    public string DropName { get; set; }

    [field: SerializeField]
    public Sprite DropIcon { get; set; }

    [field: SerializeField]
    public int MinQuantity { get; set; } = 1;

    [field: SerializeField]
    public int MaxQuantity { get; set; } = 5;
}
