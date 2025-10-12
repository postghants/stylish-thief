
using HSM;
using System;
using Unity.VisualScripting;
using UnityEngine;

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
    }

    private void FixedUpdate()
    {
        machine.Update(Time.deltaTime);
    }
}

[Serializable]
public class GrabberContext : EnemyContext
{
    public PlayerStateDriver player;

    [Header("References")]
    public ActorPhysics rb;

    [Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float walkSpeed;
    public float walkAccel;

    [Header("Grab")]
    [Tooltip("Speed added when entering grab")] public float grabSpeed;
    [Tooltip("Time before grab ends")] public float grabDuration;
    [Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [Tooltip("Speed multiplier applied when exiting grab")] public float grabDeceleration;
    [Tooltip("Friction applied during grab state")] public float grabFriction;
    public float grabEndLag;

    [Header("Internal")]
    public float grabTimer = 0;
    public bool hasGrabbed;
}
