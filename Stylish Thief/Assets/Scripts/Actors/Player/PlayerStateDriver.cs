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
        if(ctx.moveDirection.sqrMagnitude > 0)
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

}

[Serializable]
public class PlayerContext
{
    [Header("Grounded Movement")]
    public float acceleration;
    public float groundFriction;
    public float groundDeceleration;
    public float maxSpeed;

    [Header("Air Movement")]
    public float airAccel;
    public float airFriction;

    [Header("Jump")]
    public JumpData baseJumpData;
    public float coyoteTime; //How many seconds until you can't jump anymore when falling off a ledge
    public float jumpBuffer;

    [Header("Grab")]
    public float grabSpeed;
    public float grabDuration;
    public float grabDeceleration;
    public float grabFriction;

    [Header("Slide")]
    public float minSlideTime;
    public float slideFriction;
    public float slideMoveMult;
    public float maxSlideBonkAngle;

    [Header("Slide Jump")]
    public JumpData slideJumpData;

    [Header("Stunned")]
    public float stunDeceleration;
    public float stunUpwardSpeed;
    public float stunDuration;

    [Header("References")]
    public ActorPhysics rb;
    [HideInInspector] public Transform cam;

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
    public float jumpHeight; //Typically between 0 and 5
    public float timeToJumpApex; //Typically between 0.2 and 2.5
    public float upwardMovementMultiplier = 1;
    public float downwardMovementMultiplier; //Typically between 1 and 10
    public float jumpCutOff; //THIS IS A GRAVITY MULTIPLIER
}