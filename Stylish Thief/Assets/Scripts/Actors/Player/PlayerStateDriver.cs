using HSM;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateDriver : Actor, IDamageable
{
    public PlayerContext ctx;
    public PlayerRoot Root;
    public StateMachine Machine;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction grabAction;
    private InputAction poundAction;
    private InputAction panLeftAction;
    private InputAction panRightAction;

    private void Start()
    {
        // Set input references
        InitializeControls();
        ctx.cam = Camera.main.transform;
        ctx.currentJumpData = ctx.baseJumpData;
        ctx.currentHealth = ctx.maxHealth;

        // Initialize state machine
        Root = new(null, ctx);
        StateMachineBuilder builder = new(Root);
        Machine = builder.Build();

        // Instantiate player UI
        GameObject ui = Instantiate(ctx.playerUIPrefab);
        ctx.healthBar = ui.GetComponentInChildren<HealthBar>();
        if (CrimeSpreeManager.instance != null)
        {
            CrimeSpreeManager.instance.chaseUI = ui.GetComponentInChildren<ChaseUI>(true);
        }

        ctx.player = this;
        ctx.currentJumpData = ctx.baseJumpData;
        ctx.gravMultiplier = ctx.currentJumpData.downwardAccel;
    }

    private void Update()
    {

        // Perform physics checks
        ctx.rb.isGrounded = ctx.rb.IsGrounded();
        ctx.anim.SetBool("Grounded", ctx.rb.isGrounded);
        Jump.JumpBuffer(ctx);
        Jump.SetPhysics(ctx);

        // Read input
        ctx.moveInputValue = moveAction.ReadValue<Vector2>();
        float targetAngle = Mathf.Atan2(ctx.moveInputValue.x, ctx.moveInputValue.y) * Mathf.Rad2Deg + ctx.cam.eulerAngles.y;
        ctx.moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * ctx.moveInputValue.magnitude;
        if (ctx.moveDirection.sqrMagnitude > 0)
        {
            ctx.facing = ctx.moveDirection;
        }

        // Face model forward
        if (ctx.rb.velocity.sqrMagnitude > 0)
        {
            ctx.anim.transform.LookAt(ctx.anim.transform.position + (new Vector3(ctx.rb.velocity.x, 0, ctx.rb.velocity.z) + ctx.anim.transform.forward * ctx.cmd.modelTurnDelay));
        }

        Machine.Update(Time.deltaTime * ctx.timeScale);
        Debug.Log(Root.Leaf());
    }

    private void FixedUpdate()
    {

    }

    public void TakeKnockback(Vector3 knockback)
    {
        ctx.rb.velocity += knockback;
        Machine.ChangeState(Root.Leaf(), Root.airborne.stunnedAirborne);
    }

    public void SetVelocity(Vector3 newVel)
    {
        ctx.rb.velocity = newVel;
        if (newVel.y > 0) { ctx.currentlyJumping = false; }
        if (Root.Leaf().IsChildOf(Root.fixedSpeed))
        {
            Machine.ChangeState(Root.Leaf(), Root.airborne);
        }
    }

    public void SetTrigger(string name)
    {
        foreach(var trigger in ctx.anim.parameters)
        {
            if(trigger.type == AnimatorControllerParameterType.Trigger)
            {
                ctx.anim.ResetTrigger(trigger.name);
            }
        }
        ctx.anim.SetTrigger(name);
    }

    private int disableControlCounter;
    public void DisableControls()
    {
        disableControlCounter = 1;
        //disableControlCounter++;
        if (disableControlCounter > 0)
        //if (disableControlCounter == 1)
        {
            moveAction.Disable();
            jumpAction.Disable();
            grabAction.Disable();
            poundAction.Disable();
            panLeftAction.Disable();
            panRightAction.Disable();
        }
    }

    public void EnableControls()
    {
        disableControlCounter = 0;
        //disableControlCounter--;
        if (disableControlCounter == 0)
        {
            moveAction.Enable();
            jumpAction.Enable();
            grabAction.Enable();
            poundAction.Enable();
            panLeftAction.Enable();
            panRightAction.Enable();
        }
    }

    public void OnJumpStart(InputAction.CallbackContext c)
    {
        ctx.desiredJump = true;
        ctx.jumpBufferCounter = 0;
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

    public void OnPoundStart(InputAction.CallbackContext c)
    {
        ctx.pressingPound = true;
    }
    public void OnPoundStop(InputAction.CallbackContext c)
    {
        ctx.pressingPound = false;
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
        if (time == 0) { time = Time.deltaTime; }
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

    private void InitializeControls()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        grabAction = InputSystem.actions.FindAction("Grab");
        poundAction = InputSystem.actions.FindAction("Pound");
        panLeftAction = InputSystem.actions.FindAction("BumperLeft");
        panRightAction = InputSystem.actions.FindAction("BumperRight");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        grabAction.started += OnGrabStart;
        grabAction.canceled += OnGrabStop;
        poundAction.started += OnPoundStart;
        poundAction.canceled += OnPoundStop;
        panLeftAction.started += OnPanLeft;
        panRightAction.started += OnPanRight;
    }


#if UNITY_EDITOR
    private void OnGUI()
    {
        Vector2 horizontalVel = new Vector2(ctx.rb.velocity.x, ctx.rb.velocity.z);
        GUI.Label(new Rect(0, 10, 200, 30), $"XZ speed: {horizontalVel.magnitude}");
        GUI.Label(new Rect(0, 30, 200, 30), $"Y speed: {ctx.rb.velocity.y}");
        GUI.Label(new Rect(0, 50, 250, 30), $"Player state: {Machine.Root.Leaf()}");
    }
#endif

    private void Die()
    {
        Instantiate(ctx.gameOverUIPrefab);
        enabled = false;
    }

    //IDamageable
    public void TakeDamage(float damage)
    {
        ctx.currentHealth -= damage;
        if (ctx.healthBar != null)
        {
            ctx.healthBar.SetFill(ctx.currentHealth / ctx.maxHealth);
        }
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
    public MoveData groundMoveData;
    //[Tooltip("Acceleration in units/s^2")] public float acceleration;
    //[Tooltip("Friction applied when on the ground.")] public float groundFriction;
    //[Tooltip("Extra friction applied when on the ground AND not pressing any move input.")] public float groundDeceleration;
    //[Tooltip("Additional multiplier applied only when moving over the max speed.")] public float groundSpeedCapMult = 0.9f;
    //[Tooltip("Maximum grounded speed.")] public float maxSpeed;
    //[Tooltip("Multiplier on turn deceleration curve for convenience. Represents units per second squared")] public float turnDecelerationMult = 1;
    //[Tooltip("Intensity of deceleration when trying to switch direction. Read as a gradient from 0 degrees to 180 degrees")] public AnimationCurve turnDeceleration;

    [Header("Air Movement")]
    public MoveData airMoveData;
    //[Tooltip("Acceleration when airborne.")] public float airAccel;
    //[Tooltip("Friction applied when airborne.")] public float airFriction;

    [Header("Jump")]
    public JumpData baseJumpData;
    public bool disableJump;
    public float coyoteTime;
    [Tooltip("Jump input buffer time")] public float jumpBuffer;

    [Header("Grab")]
    public bool disableGrab;
    [Tooltip("Speed added when entering grab")] public float grabSpeed;
    [Tooltip("Time before grab ends")] public float grabDuration;
    [Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [Tooltip("Time spent decelerating after grab")] public float grabDeceleration;
    [Tooltip("Time until player can move after grab")] public float grabEndLag;

    [Header("Grab Targeting")]
    public float maxGrabTargetAngle;
    public float maxGrabTargetDistanceHorizontal;
    public float maxGrabTargetDistanceUp;
    public float maxGrabTargetDistanceDown;

    [Header("Vault")]
    public bool disableVault;
    public float ledgeCheckDistance;
    public float maxLedgeHeight;
    public float vaultMaxDuration;
    public MoveData vaultMoveData;

    public bool disableVaultJump;
    public JumpData vaultJump;

    [Header("Slide")]
    public bool disableSlide;
    public bool disableAirborneSlide;
    [Tooltip("Minimum duration of slide state")] public float minSlideTime;
    [Tooltip("Friction applied when sliding")] public float slideFriction;
    [Tooltip("Multiplier applied to movement input while sliding")] public float slideMoveMult;
    [Tooltip("Maximum horizontal impact angle for a bonk")] public float maxSlideBonkAngle;

    [Header("Slide Jump")]
    public bool disableSlideJump;
    public JumpData slideJumpData;

    [Header("Stunned")]
    public bool disableStun;
    public bool useStunDeceleration = true;
    [Tooltip("Multiplier applied to speed when entering stun")] public float stunDeceleration;
    [Tooltip("If speed is lower than this when entering stun, this speed is applied")] public float stunMinSpeed;
    [Tooltip("Speed added to Y velocity when entering stun")] public float stunUpwardSpeed;
    [Tooltip("Duration of stun state")] public float stunDuration;

    [Header("Harsh Landing")]
    public float harshLandingDuration;
    public float harshLandingDamage;
    public MoveData harshLandingData;

    [Header("Very Bad Landing")]
    public float veryBadLandingDuration;
    public float veryBadLandingDamage;
    public MoveData veryBadLandingData;

    [Header("Roll")]
    public bool disableRoll;
    public float rollTiming;
    [Tooltip("Speed added when entering roll")] public float rollSpeed;
    [Tooltip("Time before roll ends")] public float rollDuration;
    [Tooltip("Target speed at the end of the roll")] public float rollEndSpeed;
    [Tooltip("Time spent decelerating after roll")] public float rollDeceleration;
    [Tooltip("Time until player can move after roll")] public float rollEndLag;
    public bool disableRollJump;
    public JumpData rollJump;

    [Header("Bag Throw")]
    public bool disablePound;
    public bool additive;
    public bool poundAccelerate;
    public float prePoundUpBoost;
    public float prePoundDuration;
    public float prePoundGrav;
    public MoveData prePoundMove;
    public float poundSpeedDown;
    public float poundSpeedFw;
    public float downAcceleration;
    public float forwardAcceleration;
    public float poundLandDelay;
    public float poundLandSpeed;

    [Header("Camera Move")]
    [Tooltip("Total pan time")] public float panTime = 0.2f;
    [Tooltip("Amount of Y-axis rotation applied")] public float panAngle = 90;

    [Header("References")]
    [HideInInspector] public PlayerStateDriver player;
    public ActorPhysics rb;
    public Animator anim;
    [HideInInspector] public Transform cam;
    public CinemachineOrbitalFollow orbitalFollow;
    [HideInInspector] public HealthBar healthBar;
    public Material playerMat;
    public ParticleManager particleManager;
    public PlayerAnimEventHandler playerAnimEventHandler;

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
    public float animIdleSpeed;

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
    public float landingSpeed;
    public Vector3 currentVelocity;
    public bool useGravity = true;
    public bool hasGrabbed;
    public float grabTimer;
    public float rollTimer;
    public float stunTimer;
    public float slideTimer;
    public float regenTimer;
    public float jumpTimer;
    public float jumpApexTimer;
    public float blockJump;
    public float currentMoveMult;
    public float currentJumpMoveMult = 1;
    public MoveData cmd;
    public JumpData currentJumpData;
    public bool isStunned;

    [Header("Input values")]
    public Vector2 moveInputValue;
    public bool desiredJump;
    public bool pressingJump;
    public bool desiredGrab;
    public bool pressingGrab;
    public bool pressingPound;

}

[Serializable]
public class MoveData
{
    [Tooltip("Acceleration in units per second squared.")] public float acceleration;
    [Tooltip("Extra friction applied when not pressing any move input.")] public float deceleration;
    [Tooltip("Maximum speed.")] public float maxSpeed;
    public float maxSpeedDeceleration;
    public float turnSpeedMult;
    public float modelTurnDelay;
    [Tooltip("Multiplier on turn deceleration curve. Represents units per second squared.")] public float turnDecelerationMult = 1;
    [Tooltip("Intensity of deceleration when trying to switch direction. Read as a gradient from 0 degrees to 180 degrees.")] public AnimationCurve turnDeceleration;
}

[Serializable]
public class JumpData
{
    public float jumpImpulse;
    public bool cuttable;
    public float maxMaxSpeedTime;
    public float minMaxSpeedTime;
    public bool cutJump;
    public bool useCutoffGravMult;
    public float cutoffGravMult;
    public float cutSpeed;
    public float upwardDeceleration;
    public float upwardDecelApexThreshold;
    public float upwardDecelApex;
    public float hangtimeDuration;

    [Header("Extra")]
    public bool setSpeed;
    public float setSpeedSpeed;
    public float horizontalBoost;
    public float jumpMovementMult;
    public float jumpMovementMultTime;

    [Header("Falling")]
    public float downwardAccel;
    public float maxFallSpeed;
    public float fastFallSpeed;
}