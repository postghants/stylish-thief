
using HSM;
using System;
using UnityEngine;
using UnityEngine.AI;

public class GrabberStateDriver : EnemyStateDriver
{
    public GrabberContext ctx;

    private HSM.StateMachine machine;
    private GrabberRoot root;

    private void Start()
    {

        // Initialize state machine
        root = new(null, ctx);
        StateMachineBuilder builder = new(root);
        machine = builder.Build();

        
        ctx.activeZone.OnPlayerEnter.AddListener(OnPlayerEnterZone);
        ctx.activeZone.OnPlayerExit.AddListener(OnPlayerExitZone);
    }

    private void FixedUpdate()
    {
        machine.Update(Time.deltaTime);
        ctx.currentState = root.Leaf().ToString();

        ctx.rb.Move(ctx.rb.velocity * Time.deltaTime, false);
        if (!ctx.activeZone.IsPointInZone(transform.position))
        {
            transform.position = ctx.activeZone.ClosestPoint(transform.position);
        }
    }

    private void OnPlayerEnterZone()
    {
        ctx.playerInZone = true;
    }

    private void OnPlayerExitZone()
    {
        ctx.playerInZone = false;
    }

    public void OnPlayerHit()
    {
        ctx.hitEvent?.Invoke();
    }
}

[Serializable]
public class GrabberContext : EnemyContext
{
    public PlayerStateDriver player;

    [Header("References")]
    public ActorPhysics rb;
    public NavMeshAgent agent;
    public Animator animator;
    public Collider grabHitbox;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float walkSpeed;
    public float walkAccel;

    [Header("Grab")]
    public float grabTriggerDistance;
    [Tooltip("Speed added when entering grab")] public float grabSpeed;
    [Tooltip("Time before grab ends")] public float grabDuration;
    [Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [Tooltip("Speed multiplier applied when exiting grab")] public float grabDeceleration;
    [Tooltip("Friction applied during grab state")] public float grabFriction;
    public float grabEndLag;

    [Header("Grab hitbox")]
    public float hitboxOffset;
    public float grabDamage;

    [Header("Internal")]
    public float grabTimer = 0;
    public bool hasGrabbed;
    public bool playerInZone;
    public string currentState;

    //Events
    public delegate void HitEvent();
    public HitEvent hitEvent;
}
