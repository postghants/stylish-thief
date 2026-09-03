using Alchemy.Inspector;
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
    private InputAction trickAction;

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
        if (ctx.spawnSpreeUI)
        {
            GameObject ui = Instantiate(ctx.playerUIPrefab);
            ctx.healthBar = ui.GetComponentInChildren<UIBar>();
            if (CrimeSpreeManager.instance != null)
            {
                CrimeSpreeManager.instance.chaseUI = ui.GetComponentInChildren<ChaseUI>(true);
            }
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
        RollBuffer(ctx);
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
        if (ctx.iFramesOn && Root.Leaf().ToString() != "HSM.PlayerStunnedAirborne" && Root.Leaf().ToString() != "HSM.PlayerStunned")
        {
            if (ctx.iFrameTimer < ctx.invincibilityLength)
            {
                ctx.iFrameTimer += Time.deltaTime;
            }
            else
            {
                ctx.iFrameTimer = 0;
                ctx.iFramesOn = false;
            }
        }
    }

    public void TakeKnockback(Vector3 knockback)
    {
        //if (!ctx.iFramesOn)
        {
            ctx.rb.velocity += knockback;
            Machine.ChangeState(Root.Leaf(), Root.airborne.stunnedAirborne);
            ctx.iFramesOn = true;
        }
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
        foreach (var trigger in ctx.anim.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
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
            trickAction.Disable();
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
            trickAction.Enable();
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
    public void OnTrickStart(InputAction.CallbackContext c)
    {
        ctx.desiredRoll = true;
        ctx.pressingTrick = true;
        ctx.rollBufferCounter = 0;
    }
    public void OnTrickStop(InputAction.CallbackContext c)
    {
        ctx.pressingTrick = false;
    }
    public static void RollBuffer(PlayerContext ctx)
    {
        if (ctx.desiredRoll)
        {
            ctx.rollBufferCounter += Time.deltaTime;
            if (ctx.rollBufferCounter > ctx.rollTiming)
            {
                ctx.desiredRoll = false;
            }
        }
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
        trickAction = InputSystem.actions.FindAction("Trick");
        jumpAction.started += OnJumpStart;
        jumpAction.canceled += OnJumpStop;
        grabAction.started += OnGrabStart;
        grabAction.canceled += OnGrabStop;
        poundAction.started += OnPoundStart;
        poundAction.canceled += OnPoundStop;
        panLeftAction.started += OnPanLeft;
        panRightAction.started += OnPanRight;
        trickAction.started += OnTrickStart;
        trickAction.canceled += OnTrickStop;
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

        Cursor.lockState = CursorLockMode.None;
        enabled = false;
    }

    //IDamageable
    public void TakeDamage(float damage)
    {
        //if (!ctx.iFramesOn)
        {
            ctx.iFramesOn = true;
            ctx.currentHealth -= damage;
            CrimeSpreeManager.instance.ChaseTimer -= damage;
            if (ctx.healthBar != null)
            {
                ctx.healthBar.SetFill(ctx.currentHealth / ctx.maxHealth);
            }
            ctx.regenTimer = 0;
            if (ctx.currentHealth <= 0)
            {
                ctx.iFramesOn = false;
                Die();
            }
        }
    }

    private void OnDestroy()
    {
        jumpAction.started -= OnJumpStart;
        jumpAction.canceled -= OnJumpStop;
        grabAction.started -= OnGrabStart;
        grabAction.canceled -= OnGrabStop;
        trickAction.started -= OnTrickStart;
        trickAction.canceled -= OnTrickStop;
    }
}

[Serializable]
public class PlayerContext
{
    [FoldoutGroup("Status")] public float maxHealth = 100;
    [FoldoutGroup("Status")] public float regenRate = 10;
    [FoldoutGroup("Status")] public float regenDelay = 1.5f;
    [FoldoutGroup("Status")] public float invincibilityLength;

    [FoldoutGroup("General")] public float timeScale = 1;
    [FoldoutGroup("General")] public bool spawnSpreeUI = true;

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
    [FoldoutGroup("Jump")] public bool disableJump;
    [FoldoutGroup("Jump")] public float coyoteTime;
    [FoldoutGroup("Jump")][Tooltip("Jump input buffer time")] public float jumpBuffer;

    [FoldoutGroup("Grab")] public bool disableGrab;
    [FoldoutGroup("Grab")][Tooltip("Speed added when entering grab")] public float grabSpeed;
    [FoldoutGroup("Grab")][Tooltip("Time before grab ends")] public float grabDuration;
    [FoldoutGroup("Grab")][Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [FoldoutGroup("Grab")][Tooltip("Time spent decelerating after grab")] public float grabDeceleration;
    [FoldoutGroup("Grab")][Tooltip("Time until player can move after grab")] public float grabEndLag;

    [Header("Grab Targeting")]
    [FoldoutGroup("Grab Targeting")] public float maxGrabTargetAngle;
    [FoldoutGroup("Grab Targeting")] public float maxGrabTargetDistanceHorizontal;
    [FoldoutGroup("Grab Targeting")] public float maxGrabTargetDistanceUp;
    [FoldoutGroup("Grab Targeting")] public float maxGrabTargetDistanceDown;

    [FoldoutGroup("Vault")] public bool disableVault;
    [FoldoutGroup("Vault")] public float ledgeCheckDistance;
    [FoldoutGroup("Vault")] public float maxLedgeHeight;
    [FoldoutGroup("Vault")] public float maxLedgeHeightGround;
    [FoldoutGroup("Vault")] public float vaultMaxDuration;
    [FoldoutGroup("Vault")] public MoveData vaultMoveData;

    [FoldoutGroup("Vault Jump")] public bool disableVaultJump;
    [FoldoutGroup("Vault Jump")] public JumpData vaultJump;

    [FoldoutGroup("Slide")] public bool disableSlide;
    [FoldoutGroup("Slide")] public bool disableAirborneSlide;
    [FoldoutGroup("Slide")][Tooltip("Minimum duration of slide state")] public float minSlideTime;
    [FoldoutGroup("Slide")][Tooltip("Friction applied when sliding")] public float slideFriction;
    [FoldoutGroup("Slide")][Tooltip("Multiplier applied to movement input while sliding")] public float slideMoveMult;
    [FoldoutGroup("Slide")][Tooltip("Maximum horizontal impact angle for a bonk")] public float maxSlideBonkAngle;

    [FoldoutGroup("Slide Jump")] public bool disableSlideJump;
    [FoldoutGroup("Slide Jump")] public JumpData slideJumpData;

    [FoldoutGroup("Stunned")] public bool disableStun;
    [FoldoutGroup("Stunned")] public bool useStunDeceleration = true;
    [FoldoutGroup("Stunned")][Tooltip("Multiplier applied to speed when entering stun")] public float stunDeceleration;
    [FoldoutGroup("Stunned")][Tooltip("If speed is lower than this when entering stun, this speed is applied")] public float stunMinSpeed;
    [FoldoutGroup("Stunned")][Tooltip("Speed added to Y velocity when entering stun")] public float stunUpwardSpeed;
    [FoldoutGroup("Stunned")][Tooltip("Duration of stun state")] public float stunDuration;
    [FoldoutGroup("Stunned")][Tooltip("Maximum duration of the stun in the air")] public float airStunDuration;
    [FoldoutGroup("Stunned")] public bool setSpeedToZero;


    [FoldoutGroup("Harsh Landing")] public float harshLandingDuration;
    [FoldoutGroup("Harsh Landing")] public float harshLandingDamage;
    [FoldoutGroup("Harsh Landing")] public MoveData harshLandingData;

    [FoldoutGroup("Very Bad Landing")] public float veryBadLandingDuration;
    [FoldoutGroup("Very Bad Landing")] public float veryBadLandingDamage;
    [FoldoutGroup("Very Bad Landing")] public MoveData veryBadLandingData;

    [Header("Roll")]
    [FoldoutGroup("Roll")] public bool disableRoll;
    [FoldoutGroup("Roll")] public float rollTiming;
    [FoldoutGroup("Roll")][Tooltip("Speed added when entering roll")] public float rollSpeed;
    [FoldoutGroup("Roll")][Tooltip("Time before roll ends")] public float rollDuration;
    [FoldoutGroup("Roll")][Tooltip("Target speed at the end of the roll")] public float rollEndSpeed;
    [FoldoutGroup("Roll")][Tooltip("Time spent decelerating after roll")] public float rollDeceleration;
    [FoldoutGroup("Roll")][Tooltip("Time until player can move after roll")] public float rollEndLag;
    [FoldoutGroup("Roll")] public bool disableRollJump;
    [FoldoutGroup("Roll")] public JumpData rollJump;

    [FoldoutGroup("Bag Throw")] public bool disablePound;
    [FoldoutGroup("Bag Throw")] public bool additive;
    [FoldoutGroup("Bag Throw")] public bool poundAccelerate;
    [FoldoutGroup("Bag Throw")] public float prePoundUpBoost;
    [FoldoutGroup("Bag Throw")] public float prePoundDuration;
    [FoldoutGroup("Bag Throw")] public float prePoundGrav;
    [FoldoutGroup("Bag Throw")] public MoveData prePoundMove;
    [FoldoutGroup("Bag Throw")] public float poundSpeedDown;
    [FoldoutGroup("Bag Throw")] public float poundSpeedFw;
    [FoldoutGroup("Bag Throw")] public float downAcceleration;
    [FoldoutGroup("Bag Throw")] public float forwardAcceleration;
    [FoldoutGroup("Bag Throw")] public float poundLandDelay;
    [FoldoutGroup("Bag Throw")] public float poundLandSpeed;

    [FoldoutGroup("Camera Move")][Tooltip("Total pan time")] public float panTime = 0.2f;
    [FoldoutGroup("Camera Move")][Tooltip("Amount of Y-axis rotation applied")] public float panAngle = 90;

    [FoldoutGroup("References")][HideInInspector] public PlayerStateDriver player;
    [FoldoutGroup("References")]public ActorPhysics rb;
    [FoldoutGroup("References")]public Animator anim;
    [FoldoutGroup("References")][HideInInspector] public Transform cam;
    [FoldoutGroup("References")]public CinemachineOrbitalFollow orbitalFollow;
    [FoldoutGroup("References")][HideInInspector] public UIBar healthBar;
    [FoldoutGroup("References")]public Material playerMat;
    [FoldoutGroup("References")]public ParticleManager particleManager;
    [FoldoutGroup("References")] public PlayerAnimEventHandler playerAnimEventHandler;

    [FoldoutGroup("Prefabs")]public GameObject playerUIPrefab;
    [FoldoutGroup("Prefabs")] public GameObject gameOverUIPrefab;

    [FoldoutGroup("Animation Variables")]public float animRunSpeed;
    [FoldoutGroup("Animation Variables")] public float animIdleSpeed;

    [FoldoutGroup("Internal")][ReadOnly]public float currentHealth;
    [FoldoutGroup("Internal")][ReadOnly]public float iFrameTimer;
    [FoldoutGroup("Internal")][ReadOnly]public bool iFramesOn;
    [FoldoutGroup("Internal")][ReadOnly]public Vector3 moveDirection;
    [FoldoutGroup("Internal")][ReadOnly]public Vector3 facing;
    [FoldoutGroup("Internal")][ReadOnly]public float coyoteTimeCounter;
    [FoldoutGroup("Internal")][ReadOnly]public float jumpBufferCounter;
    [FoldoutGroup("Internal")][ReadOnly]public float rollBufferCounter;
    [FoldoutGroup("Internal")][ReadOnly]public bool currentlyJumping;
    [FoldoutGroup("Internal")][ReadOnly]public float baseGrav;
    [FoldoutGroup("Internal")][ReadOnly]public float gravMultiplier;
    [FoldoutGroup("Internal")][ReadOnly]public float jumpSpeed;
    [FoldoutGroup("Internal")][ReadOnly]public float landingSpeed;
    [FoldoutGroup("Internal")][ReadOnly]public Vector3 currentVelocity;
    [FoldoutGroup("Internal")][ReadOnly]public bool useGravity = true;
    [FoldoutGroup("Internal")][ReadOnly]public bool hasGrabbed;
    [FoldoutGroup("Internal")][ReadOnly]public float grabTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float rollTimer;
    [FoldoutGroup("Internal")][ReadOnly]public bool desiredRoll;
    [FoldoutGroup("Internal")][ReadOnly]public float stunTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float airStunTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float slideTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float regenTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float jumpTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float jumpApexTimer;
    [FoldoutGroup("Internal")][ReadOnly]public float blockJump;
    [FoldoutGroup("Internal")][ReadOnly]public float currentMoveMult;
    [FoldoutGroup("Internal")][ReadOnly]public float currentJumpMoveMult = 1;
    [FoldoutGroup("Internal")][ReadOnly]public MoveData cmd;
    [FoldoutGroup("Internal")][ReadOnly]public JumpData currentJumpData;
    [FoldoutGroup("Internal")][ReadOnly] public bool isStunned;

    [FoldoutGroup("Input values")][ReadOnly]public Vector2 moveInputValue;
    [FoldoutGroup("Input values")][ReadOnly]public bool desiredJump;
    [FoldoutGroup("Input values")][ReadOnly]public bool pressingJump;
    [FoldoutGroup("Input values")][ReadOnly]public bool desiredGrab;
    [FoldoutGroup("Input values")][ReadOnly]public bool pressingGrab;
    [FoldoutGroup("Input values")][ReadOnly]public bool pressingPound;
    [FoldoutGroup("Input values")][ReadOnly] public bool pressingTrick;

}

[Serializable]
public class MoveData
{
    [Tooltip("Acceleration in units per second squared.")] public float acceleration;
    [Tooltip("Extra friction applied when not pressing any move input.")] public float deceleration;
    public float minWalkSpeed;
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