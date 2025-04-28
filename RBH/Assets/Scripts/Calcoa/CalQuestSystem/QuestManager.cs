using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestUI questUI;

    private Dictionary<string, Quest> questMap;

    public bool mapCreated;
    public bool initialBroadcast;

    //quest start reqs
    [SerializeField] private int currentDay;
    [SerializeField] private int currentNight;

    //quest day/night progression
    [SerializeField] private int currentExperience;
    [SerializeField] private int currentCurrency;

    public float questDayProgress;
    public float nextPhaseExperience = 100f;

    private void Awake()
    {
        questMap = CreateQuestMap();
    }

    private void Start()
    {
        if (!mapCreated)
        {
            questMap = CreateQuestMap();
        }
    }

    public void AdjustDayNight(int day, int night)
    {
        currentDay = day;
        currentNight = night;
    }

    private void OnEnable()
    {
        QuestEventsManager.instance.questEvents.onStartQuest += StartQuest;
        QuestEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        QuestEventsManager.instance.questEvents.onFinishQuest += FinishQuest;
        QuestEventsManager.instance.questEvents.onQuestSubmitPressed += QuestSubmitPressed;
    }

    private void OnDisbale()
    {
        QuestEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        QuestEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        QuestEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
        QuestEventsManager.instance.questEvents.onQuestSubmitPressed -= QuestSubmitPressed;
    }

    private void Update()
    {
        if (mapCreated)
        {
            if (!initialBroadcast)
            {
                initialBroadcast = true;

                foreach (Quest quest in questMap.Values)
                {
                    QuestEventsManager.instance.questEvents.QuestStateChange(quest);

                    //CheckQuestInfo(quest);
                }
            }

            foreach (Quest quest in questMap.Values)
            {
                if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
                {
                    ChangeQuestState(quest.info.id, QuestState.CAN_START);
                }
            }

        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        QuestEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        //start true and prove false
        bool meetsRequirements = true;

        //check day reqs
        if(currentDay < quest.info.dayRequirement)
        {
            meetsRequirements = false;

            Debug.Log("currentDay less than needed");
        }

        //check night reqs
        if (currentNight < quest.info.nightRequirement)
        {
            meetsRequirements = false;

            Debug.Log("currentNight less than needed");
        }

        //check other reqs
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if(GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;

                Debug.Log("other req needed");

                break;
            }
        }

        return meetsRequirements;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);

        if (questUI != null)
        {
            questUI.UpdateTitle(quest.info.displayName);
        }

        Debug.Log("Start Quest: " + id);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();

        //if more steps
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }

        Debug.Log("Advance Quest: " + id);
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);

        if(questUI != null)
        {
            questUI.UpdateTitle("None");
            questUI.KillStep();
        }

        Debug.Log("Finish Quest: " + id);
    }

    private void ClaimRewards(Quest quest)
    {
        Debug.Log("rewarded " + quest.info.currencyReward + " currency");
        Debug.Log("rewarded " + quest.info.experienceReward + " experience");

        currentCurrency += quest.info.currencyReward;
        currentExperience += quest.info.experienceReward;

        questDayProgress = currentExperience;
    }

    private void QuestSubmitPressed(string id)
    {
        // to do interact stuffs

        Debug.Log("Submit Quest: " + id);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        //Load all QuestInfoSO Scriptable Objects under the Assets/resources/Quests folder

        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        //create quest map

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();

        foreach(QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }

            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }

        mapCreated = true;

        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];

        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map " + id);
        }

        return quest;
    }

    public void MapQuestUI(QuestStep questStep)
    {
        if(questUI != null)
        {
            questUI.currentStepItemText = questStep.stepDescription;

            CheckQuestStepInfo(questStep);

            questUI.currentStepItemHasCount = questStep.hasCount;
            questUI.currentStepItemCount = questStep.count;
            questUI.currentStepItemMaxCount = questStep.maxCount;
            questUI.currentStepItemQuestStep = questStep;

            CheckQuestStepInfo(questStep);

            questUI.MapStep();
        }
    }

    public void UpdateQuestUICount(int count, int maxCount)
    {
        if (questUI != null)
        {
            questUI.UpdateCounts(count, maxCount);

            Debug.Log("called update counts in quest manager: count " + count + " and max count " + maxCount);
        }
    }

    public void NextPhase()
    {
        currentExperience = 0;

        nextPhaseExperience = 100f;

        questDayProgress = currentExperience;
    }

    // for debug purposes
    public void CheckQuestInfo(Quest quest)
    {
        Debug.Log("Quest name " + quest.info.displayName);
        Debug.Log("Quest day req. " + quest.info.dayRequirement);
        Debug.Log("Quest night req. " + quest.info.nightRequirement);
        Debug.Log("Quest state " + quest.state);
        Debug.Log("Step exists " + quest.CurrentStepExists());
    }

    public void CheckQuestStepInfo(QuestStep questStep)
    {
        Debug.Log("Step Text Description = " + questStep.stepDescription);

    }
}
