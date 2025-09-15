using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : Actor
{
    [Header("Grounded Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float groundFriction;
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

    [Header("Internal NO TOUCH")]
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpBufferCounter;
    [SerializeField] private bool desiredJump;
    [SerializeField] private bool pressingJump;
    [SerializeField] private bool currentlyJumping;
    [SerializeField] private float gravMultiplier;
    [SerializeField] private float jumpSpeed;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Transform cam;

    private Vector3 currentVelocity;


    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        cam = Camera.main.transform;
        isGrounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        JumpBuffer();
        if (!currentlyJumping && !isGrounded)
        {
            coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            coyoteTimeCounter = 0;
        }

        currentVelocity = velocity; //Reads the current speed we're shmoving at to make new calculations with
        if (desiredJump)
        {
            PerformJump(); //Resets jump preparations and calculates a new Y speed to jump with
            velocity = currentVelocity; //Applies new Y speed as well as the X that was read earlier
            currentlyJumping = true; //Tells the code we're jumping now. Used for variable height
        }
        CalculateGravity();

        // read move input
        Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
        float targetAngle = Mathf.Atan2(moveInputValue.x, moveInputValue.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveInputValue.magnitude;

        if (moveInputValue != Vector2.zero)
        {
            if (isGrounded)
            {
                velocity += moveDirection * acceleration * Time.deltaTime;
            }
            else
            {
                velocity += moveDirection * airAccel * Time.deltaTime;
            }
            OnMoveInput(moveInputValue);

        }
        else
        {
            if (isGrounded)
            {
                velocity.x /= groundFriction; velocity.z /= groundFriction;
            }
            else
            {
                velocity.x /= airFriction; velocity.z /= airFriction;
            }
        }

        if (!isGrounded)
        {
            velocity += gravMultiplier * Time.deltaTime * gravity;
        }

        Vector2 horizontalVel = new Vector2(velocity.x, velocity.z);
        horizontalVel = Vector2.ClampMagnitude(horizontalVel, maxSpeed);
        velocity.x = horizontalVel.x; velocity.z = horizontalVel.y;
        if (velocity.sqrMagnitude < 0.001f) { velocity = Vector3.zero; }

        bool doGravityPass = !currentlyJumping;

        Move(Time.deltaTime * velocity, doGravityPass);
    }

    public void OnMoveInput(Vector2 input)
    {

    }

    public void OnJumpStart(InputAction.CallbackContext c)
    {
        desiredJump = true;
        pressingJump = true;
    }

    public void OnJumpStop(InputAction.CallbackContext c)
    {
        pressingJump = false;
    }

    private void PerformJump()
    {
        if ((isGrounded && velocity.y > -0.1) || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime)) //If grounded or if you still have coyote time
        {
            desiredJump = false;
            jumpBufferCounter = 0;
            currentVelocity.y = 0; //Very brute force fix for super jump I guess...
            CalculateJump();
            currentVelocity.y += jumpSpeed; //Swaps Y speed for the newly calculated one in CalculateJump()
        }
        if (jumpBuffer == 0)
        {
            desiredJump = false;
        }
    }

    public void CalculateJump()
    {
        jumpSpeed = Mathf.Sqrt(-2f * gravity.y * jumpHeight);
        if (velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        }
        else if (velocity.y < 0f)
        {
            jumpSpeed += Mathf.Abs(velocity.y);
        }
    }

    private void JumpBuffer()
    {
        if (desiredJump)
        {
            jumpBufferCounter += Time.deltaTime;
            if (jumpBufferCounter > jumpBuffer)
            {
                desiredJump = false;
                jumpBufferCounter = 0;
            }
        }
    }
    private void CalculateGravity()
    {
        //We change the character's gravity based on her Y direction

        //If Kit is going up...
        if (velocity.y > 0.01f)
        {
            if (isGrounded)
            {
                //Don't change it if Kit is stood on something (such as a moving platform)
                gravMultiplier = 1;
            }
            else
            {
                //Apply upward multiplier if player is rising and holding jump
                if (pressingJump && currentlyJumping)
                {
                    gravMultiplier = upwardMovementMultiplier;
                }
                //But apply a special downward multiplier if the player lets go of jump
                else
                {
                    gravMultiplier = jumpCutOff;
                }
            }
        }

        //Else if going down...
        else if (velocity.y < -0.01f)
        {

            if (isGrounded)
            //Don't change it if Kit is stood on something (such as a moving platform)
            {
                gravMultiplier = 1;
                velocity.y = 0f;
            }
            else
            {
                //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                gravMultiplier = downwardMovementMultiplier;
            }

        }
        //Else not moving vertically at all
        else
        {
            if (isGrounded)
            {
                currentlyJumping = false;
                velocity.y = 0f;
            }

            gravMultiplier = 1;
        }

        //Set the character's Rigidbody's velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
        //rb.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -speedLimit, 100));
    }
}
