using HSM;
using System;
using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateDriver : Actor, IDamageable
{
    public PlayerContext ctx;

    private PlayerRoot root;
    private StateMachine machine;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction grabAction;
    private InputAction panLeftAction;
    private InputAction panRightAction;

    private void Awake()
    {
        // Set input references
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        grabAction = InputSystem.actions.FindAction("Grab");
        panLeftAction = InputSystem.actions.FindAction("BumperLeft");
        panRightAction = InputSystem.actions.FindAction("BumperRight");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        grabAction.started += OnGrabStart;
        grabAction.canceled += OnGrabStop;
        panLeftAction.started += OnPanLeft;
        panRightAction.started += OnPanRight;
        ctx.cam = Camera.main.transform;
        ctx.currentJumpData = ctx.baseJumpData;
        ctx.currentHealth = ctx.maxHealth;


        // Initialize state machine
        root = new(null, ctx);
        StateMachineBuilder builder = new(root);
        machine = builder.Build();

        // Instantiate player UI
        ctx.healthBar = Instantiate(ctx.playerUIPrefab).GetComponentInChildren<HealthBar>();


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
        ctx.anim.SetBool("Grounded", ctx.rb.isGrounded);
        Jump.JumpBuffer(ctx);
        Jump.SetPhysics(ctx);

        // Face model forward
        if (ctx.rb.velocity.sqrMagnitude > 0)
        {
            ctx.anim.transform.LookAt(ctx.anim.transform.position + new Vector3(ctx.rb.velocity.x, 0, ctx.rb.velocity.z));
        }

        machine.Update(Time.deltaTime * ctx.timeScale);
        Debug.Log(root.Leaf());
    }

    public void TakeKnockback(Vector3 knockback)
    {
        ctx.rb.velocity += knockback;
        machine.ChangeState(root.Leaf(), root.airborne.stunnedAirborne);
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

    public void OnPanLeft(InputAction.CallbackContext c)
    {
        StartCoroutine(PanCamera(ctx.panAngle, ctx.panTime));
    }
    public void OnPanRight(InputAction.CallbackContext c)
    {
        StartCoroutine(PanCamera(-ctx.panAngle, ctx.panTime));
    }

    private IEnumerator PanCamera(float angle, float time)
    {
        float timer = 0;
        if(time == 0) { time = Time.deltaTime; }
        while (timer < time)
        {
            ctx.orbitalFollow.HorizontalAxis.Value += angle * Time.deltaTime / time;
            timer += Time.deltaTime;
            yield return null;
        }
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

    private void Die()
    {
        Instantiate(ctx.gameOverUIPrefab);
        enabled = false;
    }

    public void TakeDamage(float damage)
    {
        ctx.currentHealth -= damage;
        ctx.healthBar.SetFill(ctx.currentHealth / ctx.maxHealth);
        ctx.regenTimer = 0;
        if (ctx.currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnDestroy()
    {
        jumpAction.started -= OnJumpStart;
        jumpAction.canceled -= OnJumpStop;
        grabAction.started -= OnGrabStart;
        grabAction.canceled -= OnGrabStop;
    }
}

[Serializable]
public class PlayerContext
{
    [Header("Status")]
    public float maxHealth = 100;
    public float regenRate = 10;
    public float regenDelay = 1.5f;

    [Header("General")]
    public float timeScale = 1;

    [Header("Grounded Movement")]
    [Tooltip("Acceleration in units/s^2")] public float acceleration;
    [Tooltip("Friction applied when on the ground.")] public float groundFriction;
    [Tooltip("Extra friction applied when on the ground AND not pressing any move input.")] public float groundDeceleration;
    [Tooltip("Additional multiplier applied only when moving over the max speed.")] public float groundSpeedCapMult = 0.9f;
    [Tooltip("Maximum grounded speed.")] public float maxSpeed;
    [Tooltip("Multiplier on turn deceleration curve for convenience. Represents units per second squared")] public float turnDecelerationMult = 1;
    [Tooltip("Intensity of deceleration when trying to switch direction. Read as a gradient from 0 degrees to 180 degrees")] public AnimationCurve turnDeceleration;

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
    [Tooltip("Time spent decelerating after grab")] public float grabDeceleration;
    [Tooltip("Friction applied during grab state")] public float grabFriction;
    [Tooltip("Time until player can move after grab")] public float grabEndLag;

    [Header("Slide")]
    [Tooltip("Minimum duration of slide state")] public float minSlideTime;
    [Tooltip("Friction applied when sliding")] public float slideFriction;
    [Tooltip("Multiplier applied to movement input while sliding")] public float slideMoveMult;
    [Tooltip("Maximum horizontal impact angle for a bonk")] public float maxSlideBonkAngle;

    [Header("Slide Jump")]
    public JumpData slideJumpData;

    [Header("Stunned")]
    [Tooltip("Multiplier applied to speed when entering stun")] public float stunDeceleration;
    [Tooltip("If speed is lower than this when entering stun, this speed is applied")] public float stunMinSpeed;
    [Tooltip("Speed added to Y velocity when entering stun")] public float stunUpwardSpeed;
    [Tooltip("Duration of stun state")] public float stunDuration;

    [Header("Camera Move")]
    [Tooltip("Total pan time")] public float panTime = 0.2f;
    [Tooltip("Amount of Y-axis rotation applied")] public float panAngle = 90;

    [Header("References")]
    public ActorPhysics rb;
    public Animator anim;
    [HideInInspector] public Transform cam;
    public CinemachineOrbitalFollow orbitalFollow;
    [HideInInspector] public HealthBar healthBar;
    public Material playerMat;
    public ParticleSystem landParticles;

    [Header("Prefabs")]
    public GameObject playerUIPrefab;
    public GameObject gameOverUIPrefab;

    [Header("State colors")]
    public Color baseColor;
    public Color airColor;
    public Color grabColor;
    public Color slidingColor;
    public Color stunnedColor;

    [Header("Animation variables")]
    public float animRunSpeed;

    [Header("Internal NO TOUCHY")]
    public float currentHealth;
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
    public float regenTimer;
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