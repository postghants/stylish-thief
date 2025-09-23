using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateDriver : MonoBehaviour
{
    public PlayerContext ctx;

    private PlayerRoot root;
    private StateMachine machine;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        ctx.cam = Camera.main.transform;

        root = new(null, ctx);
        StateMachineBuilder builder = new(root);
        machine = builder.Build();

    }

    private void Update()
    {
        PlayerMove.SetPhysics(ctx);
    }

    private void FixedUpdate()
    {
        ctx.moveInputValue = moveAction.ReadValue<Vector2>();
        ctx.rb.isGrounded = ctx.rb.IsGrounded();
        PlayerMove.JumpBuffer(ctx);
        PlayerMove.SetPhysics(ctx);

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
    public float jumpHeight; //Typically between 0 and 5
    public float timeToJumpApex; //Typically between 0.2 and 2.5
    public float upwardMovementMultiplier = 1;
    public float downwardMovementMultiplier; //Typically between 1 and 10
    public float jumpCutOff; //THIS IS A GRAVITY MULTIPLIER
    public float coyoteTime; //How many seconds until you can't jump anymore when falling off a ledge
    public float jumpBuffer;

    [Header("References")]
    public CustomBoxRigidbody rb;
    [HideInInspector] public Transform cam;

    [Header("Internal NO TOUCHY")]
    public Vector3 velocity;
    public float coyoteTimeCounter;
    public float jumpBufferCounter;
    public bool desiredJump;
    public bool pressingJump;
    public bool currentlyJumping;
    public float baseGrav;
    public float gravMultiplier;
    public float jumpSpeed;
    public Vector3 currentVelocity;

    [Header("Input values")]
    public Vector2 moveInputValue;
}