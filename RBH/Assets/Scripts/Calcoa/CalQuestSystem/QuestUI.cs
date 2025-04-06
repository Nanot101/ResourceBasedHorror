using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public TextMeshProUGUI titleUIText;
    public string titleText;

    [SerializeField] private GameObject stepPanel;
    [SerializeField] private GameObject currentStepItem;
    [SerializeField] private QuestStepUI currentQuestStepUI;

    public string currentStepItemText;
    public bool currentStepItemHasCount;
    public int currentStepItemCount;
    public int currentStepItemMaxCount;
    public QuestStep currentStepItemQuestStep;

    public void UpdateTitle(string name)
    {
        if(name != null && name != "")
        {
            titleText = name;
        }

        titleUIText.text = titleText;
    }

    public void MapStep()
    {
        // to do populate arrays with quest info


        //step counts if any

        //currentStepItemHasCount = ;
        //currentStepItemCount = ;
        //currentStepItemMaxCount = ;
        //currentStepItemQuestStep = ;

        //instantiate ui
        InstantiateUIStep();
    }

    public void UpdateCounts(int count, int maxCount)
    {
        if (currentStepItemHasCount)
        {
            currentStepItemCount = count;
            currentStepItemMaxCount = maxCount;

            if (currentQuestStepUI != null)
            {
                currentQuestStepUI.UpdateCount(currentStepItemCount, currentStepItemMaxCount);

                Debug.Log("called update count in quest UI: count " + count + " and max count " + maxCount);
            }
        }
    }

    public void InstantiateUIStep()
    {
        if(stepPanel != null)
        {
            if (currentStepItem != null)
            {
                GameObject tempStep = Instantiate(currentStepItem, stepPanel.transform);

                currentQuestStepUI = tempStep.GetComponent<QuestStepUI>();

                currentQuestStepUI.questStep = currentStepItemQuestStep;

                currentQuestStepUI.steptext = currentStepItemText;

                currentQuestStepUI.hasCount = currentStepItemHasCount;
                currentQuestStepUI.count = currentStepItemCount;
                currentQuestStepUI.maxCount = currentStepItemMaxCount;

                currentQuestStepUI.UpdateText(currentStepItemText);
                currentQuestStepUI.UpdateCount(currentStepItemCount, currentStepItemMaxCount);
            }
        }
    }

    public void KillStep()
    {
        if(currentQuestStepUI != null)
        {
            currentQuestStepUI.KillStepItem();
        }
    }
}
