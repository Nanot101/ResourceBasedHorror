using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    public QuestManager questManager;

    public bool hasCount;
    public int count;
    public int maxCount;

    public string stepDescription;

    private string questId;

    void Start()
    {
        if (questManager == null)
        {
            if (GameObject.FindGameObjectWithTag("QuestManager") != null)
            {
                questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
            }
        }
    }

    public void InitializeQuestStep(string questId)
    {
        this.questId = questId;
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;

            //advance the quest forward
            QuestEventsManager.instance.questEvents.AdvanceQuest(questId);

            Destroy(this.gameObject, 0.5f);
        }
    }

    public void UpdateQuestUICount(int count, int maxCount)
    {
        if (questManager != null)
        {
            questManager.UpdateQuestUICount(count, maxCount);

            Debug.Log("called update counts in quest step: count " + count + " and max count " + maxCount);
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("QuestManager") != null)
            {
                questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
            }

            if (questManager != null)
            {
                questManager.UpdateQuestUICount(count, maxCount);

                Debug.Log("called update counts in quest step: count " + count + " and max count " + maxCount);
            }
        }
    }
}
