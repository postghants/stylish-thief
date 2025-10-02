using HSM;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerStateDriver : Actor
{
    public PlayerContext ctx;

    private PlayerRoot root;
    private StateMachine machine;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction grabAction;

    private void Awake()
    {

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        grabAction = InputSystem.actions.FindAction("Grab");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        grabAction.started += OnGrabStart;
        grabAction.canceled += OnGrabStop;
        ctx.cam = Camera.main.transform;
        ctx.currentJumpData = ctx.baseJumpData;

        root = new(null, ctx);
        StateMachineBuilder builder = new(root);
        machine = builder.Build();

    }

    private void Update()
    {
        Jump.SetPhysics(ctx);
    }

    private void FixedUpdate()
    {
        // Read input
        ctx.moveInputValue = moveAction.ReadValue<Vector2>();
        float targetAngle = Mathf.Atan2(ctx.moveInputValue.x, ctx.moveInputValue.y) * Mathf.Rad2Deg + ctx.cam.eulerAngles.y;
        ctx.moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * ctx.moveInputValue.magnitude;
        if (ctx.moveDirection.sqrMagnitude > 0)
        {
            ctx.facing = ctx.moveDirection;
        }

        // Perform physics checks
        ctx.rb.isGrounded = ctx.rb.IsGrounded();
        Jump.JumpBuffer(ctx);
        Jump.SetPhysics(ctx);

        machine.Update(Time.deltaTime);
        Debug.Log(root.Leaf());
    }
    public void OnJumpStart(InputAction.CallbackContext c)
    {
        ctx.desiredJump = true;
        ctx.pressingJump = true;
    }

    public void OnJumpStop(InputAction.CallbackContext c)
    {
        ctx.pressingJump = false;
    }

    public void OnGrabStart(InputAction.CallbackContext c)
    {
        StartCoroutine(GrabTimer());
        ctx.pressingGrab = true;
    }

    public void OnGrabStop(InputAction.CallbackContext c)
    {
        ctx.pressingGrab = false;
    }

    private IEnumerator GrabTimer()
    {
        ctx.desiredGrab = true;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        ctx.desiredGrab = false;
    }

    private void OnGUI()
    {
        Vector2 horizontalVel = new Vector2(ctx.rb.velocity.x, ctx.rb.velocity.z);
        GUI.Label(new Rect(0, 10, 200, 30), $"XZ speed: {horizontalVel.magnitude}");
        GUI.Label(new Rect(0, 30, 200, 30), $"Y speed: {ctx.rb.velocity.y}");
        GUI.Label(new Rect(0, 50, 250, 30), $"Player state: {machine.Root.Leaf()}");
    }

}

[Serializable]
public class PlayerContext
{
    [Header("Grounded Movement")]
    [Tooltip("Acceleration in units/s^2")] public float acceleration;
    [Tooltip("Friction applied when on the ground.")] public float groundFriction;
    [Tooltip("Extra friction applied when on the ground AND not pressing any move input.")] public float groundDeceleration;
    [Tooltip("Additional multiplier applied only when moving over the max speed.")] public float groundSpeedCapMult = 0.9f;
    [Tooltip("Maximum grounded speed.")] public float maxSpeed;

    [Header("Air Movement")]
    [Tooltip("Acceleration when airborne.")] public float airAccel;
    [Tooltip("Friction applied when airborne.")] public float airFriction;

    [Header("Jump")]
    public JumpData baseJumpData;
    public float coyoteTime;
    [Tooltip("Jump input buffer time")] public float jumpBuffer;

    [Header("Grab")]
    [Tooltip("Speed added when entering grab")] public float grabSpeed;
    [Tooltip("Time before grab ends")] public float grabDuration;
    [Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [Tooltip("Speed multiplier applied when exiting grab")] public float grabDeceleration;
    [Tooltip("Friction applied during grab state")] public float grabFriction;

    [Header("Slide")]
    [Tooltip("Minimum duration of slide state")] public float minSlideTime;
    [Tooltip("Friction applied when sliding")] public float slideFriction;
    [Tooltip("Multiplier applied to movement input while sliding")] public float slideMoveMult;
    [Tooltip("Maximum horizontal impact angle for a bonk")] public float maxSlideBonkAngle;

    [Header("Slide Jump")]
    public JumpData slideJumpData;

    [Header("Stunned")]
    [Tooltip("Multiplier applied to speed when entering stun")]public float stunDeceleration;
    [Tooltip("Speed added to Y velocity when entering stun")]public float stunUpwardSpeed;
    [Tooltip("Duration of stun state")]public float stunDuration;

    [Header("References")]
    public ActorPhysics rb;
    [HideInInspector] public Transform cam;
    public Material playerMat;
    public ParticleSystem landParticles;

    [Header("State colors")]
    public Color baseColor;
    public Color airColor;
    public Color grabColor;
    public Color slidingColor;
    public Color stunnedColor;

    [Header("Internal NO TOUCHY")]
    public Vector3 moveDirection;
    public Vector3 facing;
    public float coyoteTimeCounter;
    public float jumpBufferCounter;
    public bool currentlyJumping;
    public float baseGrav;
    public float gravMultiplier;
    public float jumpSpeed;
    public Vector3 currentVelocity;
    public bool useGravity = true;
    public bool hasGrabbed;
    public float grabTimer;
    public float stunTimer;
    public float slideTimer;
    public float currentFriction;
    public float currentMoveMult;
    public JumpData currentJumpData;
    public bool isStunned;

    [Header("Input values")]
    public Vector2 moveInputValue;
    public bool desiredJump;
    public bool pressingJump;
    public bool desiredGrab;
    public bool pressingGrab;
}

[Serializable]
public class JumpData
{
    [Tooltip("Expected total jump height")] public float jumpHeight; //Typically between 0 and 5
    [Tooltip("Expected time to jump apex")] public float timeToJumpApex; //Typically between 0.2 and 2.5
    [Tooltip("Gravity multiplier while moving up")] public float upwardMovementMultiplier = 1;
    [Tooltip("Gravity multiplier while moving down")] public float downwardMovementMultiplier; //Typically between 1 and 10
    [Tooltip("Gravity multiplier while moving up after letting go of jump")] public float jumpCutOff; //THIS IS A GRAVITY MULTIPLIER
}