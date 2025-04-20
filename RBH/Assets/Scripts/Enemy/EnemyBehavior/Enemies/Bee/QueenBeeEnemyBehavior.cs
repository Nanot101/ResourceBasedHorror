using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class QueenBeeEnemyBehavior : EnemyBehavior, IStunnable
{
    [SerializeField] AIController aiController;
    [SerializeField] EnemyVisionSensor sensor;
    [SerializeField] EnemyPatrol enemyPatrol;
    [SerializeField] QueenBeePatrolStateBehavior patrol;
    [SerializeField] QueenBeeAttackStateBehavior attack;
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
    [Header("Screech Attack")]
    public float screechChance = 0.3f;
    public float screechMinDistance = 16;
    [Header("Stinger Shotgun Attack")]
    public float stingerShotgunChance = 0.8f;
    public float stingerShotgunMinDistance = 15;
    [SerializeField] ParticleSystem stingerSpreadPrefab;

    [SerializeField, ReadOnly] EnemyStateType currentStateType;
    protected override void Awake()
    {
        base.Awake();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (aiController == null) aiController = GetComponentInParent<AIController>();
        sensor = GetComponent<EnemyVisionSensor>();
        enemyPatrol = GetComponent<EnemyPatrol>();

        QueenBeeStingerDashAttack dashAttack = new QueenBeeStingerDashAttack(16, 15, aiController, gameObject, sensor, player);
        QueenBeeScreechAttack screechAttack = new QueenBeeScreechAttack(screechMinDistance, aiController, gameObject, sensor, player);
        QueenBeeStingerSpread stingerSpreadAttack = new QueenBeeStingerSpread(stingerSpreadPrefab, stingerShotgunMinDistance, aiController, sensor, gameObject, player);

        abilities.Add(dashAttack);
        abilities.Add(screechAttack);
        abilities.Add(stingerSpreadAttack);
        patrol.SetupDependencies(stateMachine, sensor, enemyPatrol, aiController, player);
        attack.SetupDependencies(stateMachine, screechAttack);
        chase.SetupDependencies(stateMachine, sensor, aiController, player);
        search.SetupDependencies(stateMachine, sensor, aiController, player);
        stun.SetupDependencies(stateMachine,aiController,0f);
        stateDictionary = new System.Collections.Generic.Dictionary<EnemyStateType, EnemyStatesBehavior>() {
            { EnemyStateType.Patrol,patrol},
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
        if (stateMachine.GetCurrentState() != stateMachine.behavior.GetState(EnemyStateType.Chase)||stateMachine.GetCurrentState() == stateMachine.behavior.GetState(EnemyStateType.Stunned))
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
            if (ability is QueenBeeScreechAttack)
            {
                totalChance += screechChance;
            }else if (ability is QueenBeeStingerSpread)
            {
                totalChance += stingerShotgunChance;
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
            if (ability is QueenBeeScreechAttack)
            {
                cumulative += screechChance;
            }
            else if (ability is QueenBeeStingerSpread)
            {
                cumulative += stingerShotgunChance;
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
        //abilities = abilities.OrderBy(x => UnityEngine.Random.value).ToList();
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
        stun.SetupDependencies(stateMachine,aiController,duration);
        stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Stunned));
    }
}

[Serializable]
public class QueenBeePatrolStateBehavior : EnemyStatesBehavior
{
    EnemyVisionSensor visionSensor;
    EnemyPatrol enemyPatrol;
    AIController agentController;
    Transform player;
    //Patrol Speed
    private Transform currentDestination;

    public float patrolDefenseDistance = 5;
    public void SetupDependencies(StateMachine stateMachine, EnemyVisionSensor visionSensor, EnemyPatrol enemyPatrol, AIController agentController, Transform player)
    {
        Setup(stateMachine);
        this.visionSensor = visionSensor;
        this.enemyPatrol = enemyPatrol;
        this.agentController = agentController;
        this.player = player;
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
            foreach (var point in enemyPatrol.GetAllPatrolPoints())
            {
                if (Vector2.Distance(player.position, point.position) <= patrolDefenseDistance)
                {
                    stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Chase));
                    return;
                }
            }
        }
        if (Vector2.Distance(enemyPatrol.transform.position, currentDestination.transform.position) < 0.5f)
        {
            //Todo Idle for like 4 seconds then keep moving
            currentDestination = enemyPatrol.GetNextPatrolPoint();
            agentController.SetDestination(currentDestination.position);
        }
    }

    public override void Exit()
    {

    }

}
[Serializable]
public class DefaultChaseStateBehavior : EnemyStatesBehavior
{
    EnemyVisionSensor visionSensor;
    AIController agentController;
    Transform player;

    public float stopDistance = 1.5f;
    //Patrol Speed
    public void SetupDependencies(StateMachine stateMachine, EnemyVisionSensor visionSensor, AIController agentController, Transform player)
    {
        Setup(stateMachine);
        this.visionSensor = visionSensor;
        this.agentController = agentController;
        this.player = player;
    }
    public override void Initialize()
    {
        agentController.SetDestination(player.position);
    }

    public override void Execute()
    {
        if (!visionSensor.CanSeeCustom(player))
        {
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Search));
            return;
        }
        float distance = Vector2.Distance(agentController.transform.position, player.position);
        if (distance > stopDistance)
            agentController.SetDestination(player.position);
        else
        {
            agentController.ResetPath();
        }
    }

    public override void Exit()
    {

    }
}
[Serializable]
public class DefaultSearchStateBehavior : EnemyStatesBehavior
{
    EnemyVisionSensor visionSensor;
    AIController agentController;
    Transform player;
    Vector3 searchCenter;
    public float searchRadius = 10;
    public float searchDuration = 6;
    Cooldown cooldown;
    //Patrol Speed
    public void SetupDependencies(StateMachine stateMachine, EnemyVisionSensor visionSensor, AIController agentController, Transform player)
    {
        Setup(stateMachine);
        this.visionSensor = visionSensor;
        this.agentController = agentController;
        this.player = player;
    }
    public override void Initialize()
    {
        agentController.SetDestination(player.position);
        searchCenter = player.position;
        cooldown = new Cooldown(searchDuration);
        cooldown.Use();

    }

    public override void Execute()
    {
        if (cooldown.IsReady)
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Patrol));
        if (visionSensor.CanSeeCustom(player))
        {
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Chase));
        }
        if (agentController.HasPath()) return;
        Vector3 searchPos = GetSearchPosition(searchCenter, searchRadius);
        agentController.SetDestination(searchPos);
    }
    Vector3 GetSearchPosition(Vector3 origin, float radius, int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * radius;
            randomOffset.z = 0;

            Vector3 candidate = origin + randomOffset;

            if (visionSensor.CanSeeCustom(candidate, radius))
            {
                return candidate;
            }
        }
        //Return to same place
        return agentController.transform.position;
    }
    public override void Exit()
    {

    }
}
[Serializable]
public class QueenBeeAttackStateBehavior : EnemyStatesBehavior
{
    IAttackAbility attackAbility;

    public void SetupDependencies(StateMachine stateMachine, IAttackAbility attackAbility)
    {
        Setup(stateMachine);
        this.attackAbility = attackAbility;
    }

    public override void Initialize()
    {
        attackAbility.Activate();
    }

    public override void Execute()
    {
        attackAbility.UpdateAttack(Time.deltaTime);
        if (attackAbility.IsFinished)
        {
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Chase));
            Debug.Log("Attack completed. Transitioning to chase.");
        }
    }

    public override void Exit()
    {

    }
}
[Serializable]
public class StunStateBehavior : EnemyStatesBehavior
{
    float stunDuration;
    Cooldown stunCooldown;
    AIController controller;
    public void SetupDependencies(StateMachine stateMachine,AIController controller, float stunDuration)
    {
        Setup(stateMachine);
        this.stunDuration = stunDuration;
        this.controller = controller;
        stunCooldown = new Cooldown(stunDuration);
    }

    public override void Initialize()
    {
        stunCooldown.Use();
        controller.Stop();
    }

    public override void Execute()
    {
        if (stunCooldown.IsReady)
        {
            stateMachine.ChangeState(stateMachine.behavior.GetState(EnemyStateType.Search));
        
        }
    }

    public override void Exit()
    {

    }
}
public interface IAttackAbility
{
    bool CanActivate();
    void Activate();
    void UpdateAttack(float deltaTime);
    bool IsFinished { get; }
}

public class QueenBeeStingerDashAttack : IAttackAbility
{
    public bool IsFinished => finished;
    public float speed;
    public float maxDistance;
    private bool finished;
    private AIController controller;
    private EnemyVisionSensor sensor;
    private GameObject actor;
    private Transform player;
    private Vector3 target;

    bool rotate;
    public QueenBeeStingerDashAttack(float speed, float maxDistance, AIController controller, GameObject actor, EnemyVisionSensor sensor, Transform player, bool rotate = true)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.controller = controller;
        this.actor = actor;
        this.player = player;
        this.sensor = sensor;
        this.rotate = rotate;
        finished = false;

    }

    public void Activate()
    {
        Vector3 direction = (player.transform.position - actor.transform.position).normalized;
        direction.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, maxDistance);
        if (hit.collider == null)
            target = actor.transform.position + (direction * maxDistance);
        else
        {
            target = hit.point;
        }
        target.z = 0;
        finished = false;
        controller.ResetPath();
        controller.SetVelocity(direction * speed);
        controller.SetDestination(target);
        controller.SetSpeed(speed);
    }

    public void UpdateAttack(float deltaTime)
    {
        if (rotate)
        {
            Vector3 velocity = controller.GetVelocity();
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            controller.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
        if (!controller.HasPath())
        {
            controller.ResetSpeed();
            controller.transform.eulerAngles = Vector3.zero;
            finished = true;
        }
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
//The game has no battle so common enemies won't work, enemie's objective is to follow you so they should have abilities that are dangerous for running away
//Ok the bee will screech draining our stamina entirely making the character slow
public class QueenBeeScreechAttack : IAttackAbility
{
    public bool IsFinished => finished;
    public float abilityDistance;
    private bool finished;
    private AIController controller;
    private EnemyVisionSensor sensor;
    private GameObject actor;
    private Transform player;
    Cooldown cooldown;
    public QueenBeeScreechAttack( float maxDistance, AIController controller, GameObject actor, EnemyVisionSensor sensor, Transform player)
    {
        this.abilityDistance = maxDistance;
        this.controller = controller;
        this.actor = actor;
        this.player = player;
        this.sensor = sensor;
        finished = false;
    }

    public void Activate()
    {
        finished = false;
        controller.Stop();
        //Double checking
        if (Vector2.Distance(actor.transform.position, player.position) > abilityDistance)
        {
            PlayerStamina stamina = player.GetComponent<PlayerStamina>();
            stamina.TryConsumeExact(stamina.CurrentStamina);
        }
        cooldown = new Cooldown(1.5f);
    }

    public void UpdateAttack(float deltaTime)
    {
        if (cooldown.IsReady)
        {
            finished = true;
        }
    }

    public bool CanActivate()
    {
        if (!sensor.CanSeeCustom(player.transform)) return false;
        if (Vector2.Distance(actor.transform.position, player.transform.position) > abilityDistance)
        {
            return true;
        }
        return false;
    }
}
public class QueenBeeStingerSpread : IAttackAbility
{
    public bool IsFinished => finished;
    public float abilityDistance;
    private bool finished;
    private EnemyVisionSensor sensor;
    private AIController controller;
    private GameObject actor;
    private Transform player;
    Cooldown cooldown;

    ParticleSystem stingerPrefab;
    ParticleSystem stingerInstance;
    public QueenBeeStingerSpread(ParticleSystem stingerPrefab, float abilityDistance, AIController controller, EnemyVisionSensor sensor, GameObject actor, Transform player)
    {
        this.stingerPrefab = stingerPrefab;
        this.abilityDistance = abilityDistance;
        this.controller = controller;
        this.sensor = sensor;
        this.actor = actor;
        this.player = player;
        finished = false;
    }
    //I can use particle system caching or something like this if performance issue arise
    public void Activate()
    {
        finished = false;
        controller.Stop();
        stingerInstance = GameObject.Instantiate(stingerPrefab.gameObject, actor.transform).GetComponent<ParticleSystem>();
        cooldown = new Cooldown(2);
        cooldown.Use();
    }

    public void UpdateAttack(float deltaTime)
    {
        Vector3 direction = (player.transform.position - actor.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        stingerInstance.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        if (cooldown.IsReady)
        {
            stingerInstance.GetComponent<ParticleSystem>().Stop();
            finished = true;
        }
    }

    public bool CanActivate()
    {
        if (!sensor.CanSeeCustom(player.transform)) return false;
        if (Vector2.Distance(actor.transform.position, player.transform.position) < abilityDistance)
        {
            return true;
        }
        return false;
    }
}
public class Cooldown
{
    private float cooldownTime;
    private float lastUsedTime;

    public Cooldown(float cooldownDuration)
    {
        cooldownTime = cooldownDuration;
        lastUsedTime = -cooldownDuration;
    }

    public bool IsReady => Time.time >= lastUsedTime + cooldownTime;

    public void Use()
    {
        lastUsedTime = Time.time;
    }

    public float TimeLeft => Mathf.Max(0f, (lastUsedTime + cooldownTime) - Time.time);
}
