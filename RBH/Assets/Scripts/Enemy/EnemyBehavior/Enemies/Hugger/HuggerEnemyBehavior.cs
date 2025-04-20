using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuggerEnemyBehavior : EnemyBehavior, IStunnable
{
    [SerializeField] AIController aiController;
    [SerializeField] EnemyVisionSensor sensor;
    [SerializeField] EnemyPatrol enemyPatrol;
    [SerializeField] QueenBeeAttackStateBehavior attack;
    [SerializeField] HuggerPatrolStateBehavior patrol;
    [SerializeField] DefaultChaseStateBehavior chase;
    [SerializeField] DefaultSearchStateBehavior search;
    [SerializeField] StunStateBehavior stun;
    Cooldown cooldown;
    List<IAttackAbility> abilities = new List<IAttackAbility>();
    IAttackAbility currentAttack;

    public float abilityCooldownTimer = 6;

    [Header("Dash Attack")]
    public float dashChance = 1f;
    public float dashSpeed = 16;
    public float dashMaxDistance = 15;
    [Header("Hook Ability")]
    public float hookChance = 1f;
    public float hookSpeed = 20;
    public float hookMaxDistance = 30;
    public HookProjectile hookProjectilePrefab;

    [SerializeField, ReadOnly] EnemyStateType currentStateType;
    protected override void Awake()
    {
        base.Awake();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (aiController == null) aiController = GetComponentInParent<AIController>();
        sensor = GetComponent<EnemyVisionSensor>();
        enemyPatrol = GetComponent<EnemyPatrol>();

        QueenBeeStingerDashAttack dashAttack = new QueenBeeStingerDashAttack(dashSpeed, dashMaxDistance, aiController, gameObject, sensor, player, false);
        HuggerHookAbility hookAbility = new HuggerHookAbility(hookSpeed, hookMaxDistance, aiController, gameObject, sensor, player, hookProjectilePrefab);
        abilities.Add(dashAttack);
        abilities.Add(hookAbility);
        patrol.SetupDependencies(stateMachine, sensor, enemyPatrol, aiController, player);
        attack.SetupDependencies(stateMachine, dashAttack);
        chase.SetupDependencies(stateMachine, sensor, aiController, player);
        search.SetupDependencies(stateMachine, sensor, aiController, player);
        stun.SetupDependencies(stateMachine,aiController,0f);

        stateDictionary = new System.Collections.Generic.Dictionary<EnemyStateType, EnemyStatesBehavior>() {
            { EnemyStateType.Patrol, patrol },
            { EnemyStateType.Chase,chase},
            { EnemyStateType.Attack,attack},
            { EnemyStateType.Search, search},
            {EnemyStateType.Stunned, stun }
        };
    }

    protected virtual void Start()
    {
        stateMachine.Initialize(patrol, this);
        cooldown = new Cooldown(abilityCooldownTimer);
    }

    protected override void Update()
    {
        base.Update();
        //Debug only
        currentStateType = stateDictionary.FirstOrDefault(pair => pair.Value == stateMachine.GetCurrentState()).Key;
        if (stateMachine.GetCurrentState() != stateMachine.behavior.GetState(EnemyStateType.Chase))
            return;
        if (!cooldown.IsReady)
            return;
        if (currentAttack != null && currentAttack.IsFinished)
        {
            currentAttack = null;
        }
        if (currentAttack != null && !currentAttack.IsFinished)
        {
            return;
        }
        List<IAttackAbility> eligibleAbilities = abilities.Where(ability => ability.CanActivate()).ToList();
        if (eligibleAbilities.Count == 0)
        {
            return;
        }
        //Doing this manually for simplicity and because this might be due to changes so this makes it easier to change without having to rewrite lots of things
        float totalChance = 0;
        foreach (IAttackAbility ability in eligibleAbilities)
        {
            if (ability is QueenBeeStingerDashAttack)
            {
                totalChance += dashChance;
            }
            else
            if (ability is HuggerHookAbility)
            {
                totalChance += hookChance;
            }
        }

        float roll = UnityEngine.Random.Range(0f, totalChance);

        float cumulative = 0f;

        IAttackAbility selectedAbility = null;

        foreach (IAttackAbility ability in eligibleAbilities)
        {
            if (ability is QueenBeeStingerDashAttack)
            {
                cumulative += dashChance;
            }
            else
            if (ability is HuggerHookAbility)
            {
                cumulative += hookChance;
            }

            if (roll <= cumulative)
            {
                selectedAbility = ability;
                break;
            }
        }
        if (selectedAbility != null)
        {
            currentAttack = selectedAbility;
            attack.SetupDependencies(stateMachine, selectedAbility);
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Attack));
            cooldown.Use();
        }

        //foreach (IAttackAbility ability in abilities)
        //{
        //    if (ability.CanActivate() && cooldown.IsReady)
        //    {
        //        currentAttack = ability;
        //        attack.SetupDependencies(stateMachine, ability);
        //        stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Attack));
        //        cooldown.Use();
        //        break;
        //    }

        //}

    }

    public void Stun(float duration)
    {
        stun.SetupDependencies(stateMachine, aiController, duration);
        stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Stunned));
    }
}
[Serializable]
public class HuggerPatrolStateBehavior : EnemyStatesBehavior
{
    EnemyVisionSensor visionSensor;
    EnemyPatrol enemyPatrol;
    AIController agentController;
    Transform player;
    //Patrol Speed
    private Transform currentDestination;
    private Cooldown teleportCooldown;
    public float teleportCooldownTimer = 10f;
    public float minTeleportDistanceFromPlayer = 30f;
    public void SetupDependencies(StateMachine stateMachine, EnemyVisionSensor visionSensor, EnemyPatrol enemyPatrol, AIController agentController, Transform player)
    {
        Setup(stateMachine);
        this.visionSensor = visionSensor;
        this.enemyPatrol = enemyPatrol;
        this.agentController = agentController;
        this.player = player;
        teleportCooldown = new Cooldown(teleportCooldownTimer);
        teleportCooldown.Use();
    }
    public override void Initialize()
    {
        currentDestination = enemyPatrol.GetNextPatrolPoint();
        agentController.SetDestination(currentDestination.position);
    }

    public override void Execute()
    {
        if (visionSensor.CanSeeCustom(player))
        {
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Chase));
            return;
        }
        if (Vector2.Distance(enemyPatrol.transform.position, currentDestination.transform.position) < 0.5f)
        {
            currentDestination = enemyPatrol.GetNextPatrolPoint();
            agentController.SetDestination(currentDestination.position);
        }
        if (teleportCooldown.IsReady)
        {
            float distanceToPlayer = Vector2.Distance(enemyPatrol.transform.position, player.position);
            if (distanceToPlayer > minTeleportDistanceFromPlayer)
            {
                TeleportToNewPatrolPoint();
                teleportCooldown.Use();
            }
        }
    }
    public void TeleportToNewPatrolPoint()
    {
        Transform[] points = enemyPatrol.GetAllPatrolPoints();

        if (points.Length == 0)
            return;
        //I'm making it random but it could try getting points closer to player, the problem that may arise is the enemy always following the player
        //but maybe this could be designed on purpose to always keep the player on edge and make the hugger like the main enemy
        Transform newPoint = points[UnityEngine.Random.Range(0, points.Length)];
        if (Vector2.Distance(newPoint.position, player.position) < minTeleportDistanceFromPlayer)
        {
            return;
        }
        agentController.transform.position = newPoint.transform.position;

        currentDestination = enemyPatrol.GetNextPatrolPoint();
        agentController.SetDestination(currentDestination.position);
    }

    public override void Exit()
    {

    }
}

public class HuggerHookAbility : IAttackAbility
{
    public bool IsFinished => finished;
    public float speed;
    public float maxDistance;
    private bool finished;
    private AIController controller;
    private EnemyVisionSensor sensor;
    private GameObject actor;
    private Transform player;
    private HookProjectile hookPrefab;

    public HuggerHookAbility(float speed, float maxDistance, AIController controller, GameObject actor, EnemyVisionSensor sensor, Transform player, HookProjectile hookPrefab)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.controller = controller;
        this.actor = actor;
        this.player = player;
        this.sensor = sensor;
        this.hookPrefab = hookPrefab;
        finished = false;

    }

    public void Activate()
    {
        Vector3 direction = (player.transform.position - actor.transform.position).normalized;
        direction.z = 0;
        //I would probably call this a lazy solution for now as we can have other things later like tables and environment in geral and this will need to be updated
        int layerMask = LayerMask.GetMask("Wall");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, maxDistance, layerMask);
        float finalDistance;
        if (hit.collider == null)
            finalDistance = maxDistance;
        else
        {
            finalDistance = hit.distance;
        }
        HookProjectile hook = GameObject.Instantiate(hookPrefab.gameObject).GetComponent<HookProjectile>();
        hook.transform.position = actor.transform.position;
        hook.transform.parent = actor.transform;
        hook.Initialize(direction, actor.transform, speed, finalDistance, OnHookComplete);

        finished = false;
        controller.ResetPath();
    }

    public void UpdateAttack(float deltaTime)
    {

    }
    public void OnHookComplete()
    {
        finished = true;
    }
    public bool CanActivate()
    {
        if (!sensor.CanSeeCustom(player.transform)) return false;
        if (Vector2.Distance(actor.transform.position, player.transform.position) < maxDistance)
        {
            return true;
        }
        return false;
    }

}
