using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public int dayRequirement;
    public int nightRequirement;

    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int currencyReward;
    public int experienceReward;

    //ensure the id is always the name of the Scriptable Object asset
    private void OnValidate()
    {
#if UNITY_EDITOR
id = this.name;
UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
