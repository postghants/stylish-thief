using HSM;
using System;
using UnityEngine;
using UnityEngine.AI;

public class DropkickerStateDriver : EnemyStateDriver
{
    public DropkickerContext ctx;

    private DropkickerRoot root;
    private StateMachine machine;

    private void Start()
    {

        // Initialize state machine
        root = new(null, ctx);
        StateMachineBuilder builder = new(root);
        machine = builder.Build();

        ctx.activeZone.OnPlayerEnter.AddListener(OnPlayerEnterZone);
        ctx.activeZone.OnPlayerExit.AddListener(OnPlayerExitZone);
        ctx.playerInZone = ctx.activeZone.IsPlayerInZone();
    }

    public override void Initialize(PatrolZone zone, PlayerStateDriver player)
    {
        ctx.activeZone = zone;
        ctx.player = player;
    }

    private void FixedUpdate()
    {
        machine.Update(Time.deltaTime);
        ctx.currentState = root.Leaf().ToString();

        if (!ctx.activeZone.IsPointInZone(transform.position))
        {
            Collider[] colliders = Physics.OverlapBox(transform.position, ctx.rb.environmentCollider.bounds.extents);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out PatrolZone zone))
                {
                    SwitchActiveZone(zone);
                }
            }
            transform.position = ctx.activeZone.ClosestPoint(transform.position);
        }
    }

    public void OnPlayerHit()
    {
        Vector3 kb = ctx.rb.velocity * ctx.grabKbHorizontal;
        kb.y = ctx.grabKbVertical;
        ctx.player.TakeKnockback(kb);
    }

    private void OnPlayerEnterZone()
    {
        ctx.playerInZone = true;
    }

    private void OnPlayerExitZone()
    {
        ctx.playerInZone = false;
    }

    public override void SwitchActiveZone(PatrolZone zone)
    {
        ctx.activeZone.OnPlayerEnter.RemoveListener(OnPlayerEnterZone);
        ctx.activeZone.OnPlayerExit.RemoveListener(OnPlayerExitZone);

        ctx.activeZone = zone;

        ctx.activeZone.OnPlayerEnter.AddListener(OnPlayerEnterZone);
        ctx.activeZone.OnPlayerExit.AddListener(OnPlayerExitZone);

    }
}

[Serializable]
public class DropkickerContext : JumperContext
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Collider grabHitbox;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float walkSpeed;
    public float walkAccel;
    public float chaseDistance;

    [Header("Dropkick")]
    public float dropkickDistance;
    public DropkickerJumpData dropkickData;
    public float dropkickSpeed;
    public float recoveryTime = 4;

    public float hitboxDelay;
    public float hitboxActiveTime;
    public float grabDamage;
    public float grabKbHorizontal;
    public float grabKbVertical;

    [Header("Internal")]
    public bool playerInZone;
    public string currentState;
}

public class DropkickerJumpData : JumpData
{

    [Tooltip("Expected total jump height")] public float jumpHeight; //Typically between 0 and 5
    [Tooltip("Expected time to jump apex")] public float timeToJumpApex; //Typically between 0.2 and 2.5
    [Tooltip("Gravity multiplier while moving up")] public float upwardMovementMultiplier = 1;
    [Tooltip("Gravity multiplier while moving down")] public float downwardMovementMultiplier; //Typically between 1 and 10
    [Tooltip("Gravity multiplier during hangtime")] public float hangtimeMovementMultiplier;
    [Tooltip("Duration of hangtime at jump apex")] public float jumpApexHangtime;
}