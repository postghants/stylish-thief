using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : Actor
{
    [Header("Grounded Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float groundFriction;
    [SerializeField] private float groundDeceleration;
    [SerializeField] private float maxSpeed;

    [Header("Air Movement")]
    [SerializeField] private float airAccel;
    [SerializeField] private float airFriction;

    [Header("Jump")]
    [SerializeField] float jumpHeight; //Typically between 0 and 5
    [SerializeField] float timeToJumpApex; //Typically between 0.2 and 2.5
    [SerializeField] float upwardMovementMultiplier = 1;
    [SerializeField] float downwardMovementMultiplier; //Typically between 1 and 10
    [SerializeField] float jumpCutOff; //THIS IS A GRAVITY MULTIPLIER
    [SerializeField] float coyoteTime; //How many seconds until you can't jump anymore when falling off a ledge
    [SerializeField] float jumpBuffer;

    [Header("References")]
    [SerializeField] private CustomBoxRigidbody rb;

    [Header("Internal NO TOUCH")]
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferCounter;
    [SerializeField] private bool desiredJump;
    [SerializeField] private bool pressingJump;
    [SerializeField] private bool currentlyJumping;
    [SerializeField] private float baseGrav;
    [SerializeField] private float gravMultiplier;
    [SerializeField] private float jumpSpeed;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Transform cam;

    private Vector3 currentVelocity;


    //private void Start()
    //{
    //    moveAction = InputSystem.actions.FindAction("Move");
    //    jumpAction = InputSystem.actions.FindAction("Jump");
    //    jumpAction.started += OnJumpStart;
    //    jumpAction.canceled += OnJumpStop;
    //    cam = Camera.main.transform;
    //}

    //private void Update()
    //{
    //    SetPhysics();
    //}

    //private void FixedUpdate()
    //{
    //    rb.isGrounded = rb.IsGrounded();
    //    JumpBuffer();
    //    if (!currentlyJumping && !rb.isGrounded)
    //    {
    //        coyoteTimeCounter += Time.deltaTime;
    //    }
    //    else
    //    {
    //        coyoteTimeCounter = 0;
    //    }

    //    currentVelocity = rb.velocity; //Reads the current speed we're shmoving at to make new calculations with
    //    if (desiredJump)
    //    {
    //        PerformJump(); //Resets jump preparations and calculates a new Y speed to jump with
    //        rb.velocity = currentVelocity; //Applies new Y speed as well as the X that was read earlier
    //        currentlyJumping = true; //Tells the code we're jumping now. Used for variable height
    //    }
    //    CalculateGravity();

    //    // read move input
    //    Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
    //    float targetAngle = Mathf.Atan2(moveInputValue.x, moveInputValue.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
    //    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveInputValue.magnitude;

    //    if (moveInputValue != Vector2.zero)
    //    {
    //        if (rb.isGrounded)
    //        {
    //            rb.velocity += acceleration * Time.deltaTime * moveDirection;
    //        }
    //        else
    //        {
    //            rb.velocity += airAccel * Time.deltaTime * moveDirection;
    //        }
    //        OnMoveInput(moveInputValue);
    //    }
    //    else if (rb.isGrounded)
    //    {
    //        rb.velocity += new Vector3(-rb.velocity.x, 0, -rb.velocity.z) * groundDeceleration;
    //    }

    //    if (rb.isGrounded)
    //    {
    //        rb.velocity += new Vector3(-rb.velocity.x, 0, -rb.velocity.z) * groundFriction;
    //    }
    //    else
    //    {
    //        rb.velocity += new Vector3(-rb.velocity.x, 0, -rb.velocity.z) * airFriction;

    //    }

    //    if (!rb.isGrounded)
    //    {
    //        rb.velocity.y += Time.deltaTime * -baseGrav;
    //    }

    //    Vector2 horizontalVel = new Vector2(rb.velocity.x, rb.velocity.z);
    //    horizontalVel = Vector2.ClampMagnitude(horizontalVel, maxSpeed);
    //    rb.velocity.x = horizontalVel.x; rb.velocity.z = horizontalVel.y;
    //    if (rb.velocity.sqrMagnitude < 0.001f) { rb.velocity = Vector3.zero; }

    //    bool doGravityPass = !currentlyJumping;

    //    rb.Move(Time.deltaTime * rb.velocity, doGravityPass);
    //}

    //public void OnMoveInput(Vector2 input)
    //{

    //}

    //public void OnJumpStart(InputAction.CallbackContext c)
    //{
    //    desiredJump = true;
    //    pressingJump = true;
    //}

    //public void OnJumpStop(InputAction.CallbackContext c)
    //{
    //    pressingJump = false;
    //}

    public static void SetPhysics(PlayerContext ctx)
    {
        //Determine the character's gravity scale, using the stats provided. Multiply it by a gravMultiplier, used later
        Vector2 newGravity = new Vector2(0, (-2 * ctx.jumpHeight) / (ctx.timeToJumpApex * ctx.timeToJumpApex));
        ctx.baseGrav = (newGravity.y / ctx.rb.gravity.y) * ctx.gravMultiplier;
    }

    public static void PerformJump(PlayerContext ctx)
    {
        if ((ctx.rb.isGrounded && ctx.rb.velocity.y > -0.1) || (ctx.coyoteTimeCounter > 0.03f && ctx.coyoteTimeCounter < ctx.coyoteTime)) //If grounded or if you still have coyote time
        {
            ctx.desiredJump = false;
            ctx.jumpBufferCounter = 0;
            ctx.currentVelocity.y = 0; //Very brute force fix for super jump I guess...
            CalculateJump(ctx);
            ctx.currentVelocity.y += ctx.jumpSpeed; //Swaps Y speed for the newly calculated one in CalculateJump()
        }
        if (ctx.jumpBuffer == 0)
        {
            ctx.desiredJump = false;
        }
    }

    public static void CalculateJump(PlayerContext ctx)
    {
        ctx.jumpSpeed = Mathf.Sqrt(-2f * ctx.rb.gravity.y * ctx.jumpHeight);
        // was causing issues with coyote jump
        //if (velocity.y > 0f)
        //{
        //    jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        //}
        //else if (velocity.y < 0f)
        //{
        //    jumpSpeed += Mathf.Abs(velocity.y);
        //}
    }

    public static void JumpBuffer(PlayerContext ctx)
    {
        if (ctx.desiredJump)
        {
            ctx.jumpBufferCounter += Time.deltaTime;
            if (ctx.jumpBufferCounter > ctx.jumpBuffer)
            {
                ctx.desiredJump = false;
                ctx.jumpBufferCounter = 0;
            }
        }
    }
    public static void CalculateGravity(PlayerContext ctx)
    {
        //We change the character's gravity based on her Y direction

        //If Kit is going up...
        if (ctx.rb.velocity.y > 0.01f)
        {
            if (ctx.rb.isGrounded)
            {
                //Don't change it if Kit is stood on something (such as a moving platform)
                ctx.gravMultiplier = 1;
            }
            else
            {
                //Apply upward multiplier if player is rising and holding jump
                if (ctx.pressingJump && ctx.currentlyJumping)
                {
                    ctx.gravMultiplier = ctx.upwardMovementMultiplier;
                }
                //But apply a special downward multiplier if the player lets go of jump
                else
                {
                    ctx.gravMultiplier = ctx.jumpCutOff;
                }
            }
        }

        //Else if going down...
        else if (ctx.rb.velocity.y < -0.01f)
        {

            if (ctx.rb.isGrounded)
            //Don't change it if Kit is stood on something (such as a moving platform)
            {
                ctx.gravMultiplier = 1;
                ctx.rb.velocity.y = 0f;
            }
            else
            {
                //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                ctx.gravMultiplier = ctx.downwardMovementMultiplier;
            }

        }
        //Else not moving vertically at all
        else
        {
            if (ctx.rb.isGrounded)
            {
                ctx.currentlyJumping = false;
                ctx.rb.velocity.y = 0f;
            }

            ctx.gravMultiplier = 1;
        }

        //Set the character's Rigidbody's velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
        //rb.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -speedLimit, 100));
    }
}
