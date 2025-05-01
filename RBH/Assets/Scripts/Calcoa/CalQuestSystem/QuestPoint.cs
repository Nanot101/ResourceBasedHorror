using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest references")]
    [SerializeField] private QuestManager questManager;
    [SerializeField] private QuestInfoSO questInfoForPoint;
    [SerializeField] private QuestState currentQuestState;
    private string questId;
    private bool setID;
    [SerializeField] private GameObject startEffect;
    [SerializeField] private GameObject finishEffect;

    private QuestIcon questIcon;

    [Header("Quest Params")]
    [SerializeField] private bool startImmediate;
    [SerializeField] private bool disableAfterStart;
    [SerializeField] private bool disableAfterFinish;
    [SerializeField] private bool startPoint;
    [SerializeField] private bool finishPoint;

    [SerializeField] private bool playerIsNear;
    [SerializeField] private bool initialEnable;

    private void Awake()
    {
        questId = questInfoForPoint.id;

        if (gameObject.GetComponentInChildren<QuestIcon>() != null)
        {
            questIcon = GetComponentInChildren<QuestIcon>();
        }

        if(questId != null)
        {
            setID = true;
        }
    }

    private void Start()
    {
        if (!setID)
        {
            questId = questInfoForPoint.id;

            if (gameObject.GetComponentInChildren<QuestIcon>() != null)
            {
                questIcon = GetComponentInChildren<QuestIcon>();
            }

            Debug.Log("set questpoint id in start: " + questId);

            if (questId != null)
            {
                setID = true;
            }
        }
    }

    void Update()
    {
        if(questManager != null)
        {
            if (questManager.initialBroadcast)
            {
                if (!initialEnable)
                {
                    initialEnable = true;

                    OnEnable();

                    //Debug.Log("Calling OnEnable in Quest Point");
                }
            }
        }
    }

    private void OnEnable()
    {
        //QuestEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;

        if (questManager != null)
        {
            if (questManager.initialBroadcast)
            {
                QuestEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
                //QuestEventsManager.instance.questEvents.onQuestSubmitPressed += QuestSubmitPressed;

                initialEnable = true;

                questManager.initialBroadcast = false;
            }
        }
    }

    private void OnDisable()
    {
        QuestEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        //QuestEventsManager.instance.questEvents.onQuestSubmitPressed -= QuestSubmitPressed;

        initialEnable = false;
    }

    private void QuestSubmitPressed()
    {
        if (!playerIsNear)
        {
            Debug.Log("player not near");
            return;
        }

        if(currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            QuestEventsManager.instance.questEvents.StartQuest(questId);

            PlayEffect(startEffect);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            QuestEventsManager.instance.questEvents.FinishQuest(questId);

            PlayEffect(finishEffect);
        }

        Debug.Log("pressed a submit for quest at " + currentQuestState);

        if (currentQuestState.Equals(QuestState.IN_PROGRESS) && disableAfterStart)
        {
            gameObject.SetActive(false);
        }

        if (currentQuestState.Equals(QuestState.FINISHED) && disableAfterFinish)
        {
            gameObject.SetActive(false);           
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);

            Debug.Log("Quest with id of " + questId + " updated to state " + currentQuestState);
        }
        else
        {
            Debug.Log("QuestStateChange was called but info didnt match in id");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerIsNear = true;

            if (startImmediate)
            {
                startImmediate = false;

                QuestSubmitPressed();
            }

            if (finishPoint)
            {
                if (!currentQuestState.Equals(QuestState.CAN_FINISH))
                {
                    if (!currentQuestState.Equals(QuestState.FINISHED))
                    {
                        startImmediate = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void PlayEffect(GameObject effect)
    {
        if (effect != null)
        {
            Instantiate(effect, transform.position, Quaternion.identity);
        }
    }

}
