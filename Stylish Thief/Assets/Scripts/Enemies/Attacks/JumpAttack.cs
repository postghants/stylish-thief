using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class JumpAttack : EnemyAttack
{
    [Header("Dropkick")]
    public DropkickerJumpData dropkickData;
    public JumperContext jumperContext = new();
    public float dropkickSpeed;
    public float recoveryTime = 4;

    public float hitboxDelay;
    public float hitboxActiveTime;
    public float grabDamage;
    public float grabKbHorizontal;
    public float grabKbVertical;

    private float timer = 0;
    private float recoveryTimer = 0;
    private bool playedRecovery = false;


    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;
    public Collider grabHitbox;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
        jumperContext.rb = rb; 
        if (ctr == null)
        {
            if (TryGetComponent(out EnemyController controller))
            {
                ctr = controller;
                ctr.AddAnimationNames(animationCodeNames);
            }
        }
    }

    public override void OnEnter()
    {
        timer = 0;
        recoveryTimer = 0;
        ctr.PlayAnimation("JumpAttack");
        agent.enabled = false;
        jumperContext.currentJumpData = dropkickData;
        EnemyJump.PerformJump(jumperContext);
        rb.velocity += jumperContext.currentVelocity;
        jumperContext.gravMultiplier = jumperContext.currentJumpData.upwardMovementMultiplier;

        Debug.Log(ctr);
        Vector3 dist = ctr.ctx.player.transform.position - rb.transform.position;
        dist.y = 0;
        rb.velocity += dropkickSpeed * dist.normalized;

        Vector3 lookPos = ctr.ctx.player.transform.position;
        lookPos.y = agent.transform.position.y;
        rotationTf.LookAt(lookPos);
    }

    public override void OnUpdate(float deltaTime)
    {
        timer += deltaTime;
        if (recoveryTimer > recoveryTime - 0.6 && !playedRecovery)
        {
            ctr.SetAnimationTrigger("JumpAttackRecoverTrigger");
            playedRecovery = true;
        }
        if (recoveryTimer > recoveryTime)
        {
            ctr.ExitAttack(this);
        }

        EnemyJump.SetPhysics(jumperContext);
        EnemyJump.CalculateGravity(jumperContext);
        rb.velocity += jumperContext.baseGrav * jumperContext.gravMultiplier * Time.deltaTime * rb.gravity;
        rb.Move(rb.velocity * Time.deltaTime, false);

        if (timer > hitboxDelay)
        {
            if (timer < hitboxDelay + hitboxActiveTime)
            {
                grabHitbox.gameObject.SetActive(true);
            }
            else
            {
                grabHitbox.gameObject.SetActive(false);
            }
        }

        rb.isGrounded = rb.IsGrounded();
        if (rb.isGrounded)
        {
            recoveryTimer += deltaTime;
            rb.velocity = Vector3.zero;
        }
    }

    public void OnHit()
    {
        Vector3 kb = rb.velocity * grabKbHorizontal;
        kb.y = grabKbVertical;
        ctr.ctx.player.TakeKnockback(kb);
        ctr.ctx.player.TakeDamage(grabDamage);
        StartCoroutine(Hitstop.FreezeTimescale(0f, 0.2f));
        StartCoroutine(CamShake.ShakeCam(3, 0.3f, 0.15f, FindAnyObjectByType<CinemachineCamera>()));
    }

    public override void OnExit()
    {
        timer = 0;
        recoveryTimer = 0;
        playedRecovery = false;
        agent.enabled = true;
        rb.velocity = Vector3.zero;
        rb.isGrounded = false;
        grabHitbox.gameObject.SetActive(false);
    }

    protected override void Reset()
    {
        animationCodeNames.Add("JumpAttack");
        animationCodeNames.Add("JumpAttackRecoverTrigger");
        base.Reset();
    }
}
