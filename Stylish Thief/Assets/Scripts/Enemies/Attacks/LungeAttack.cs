using UnityEngine;
using UnityEngine.AI;

public class LungeAttack : EnemyAttack
{
    [Header("Grab")]
    [Tooltip("Speed added when entering grab")] public float grabSpeed;
    [Tooltip("Time before grab ends")] public float grabDuration;
    [Tooltip("Target speed at the end of the grab")] public float grabEndSpeed;
    [Tooltip("Speed multiplier applied when exiting grab")] public float grabDeceleration;
    [Tooltip("Friction applied during grab state")] public float grabFriction;
    public float grabEndLag;

    [Header("Grab hitbox")]
    public float hitboxOffset;
    public float grabDamage;
    public float grabKbHorizontal;
    public float grabKbVertical;
    [Tooltip("Adds x times the grabber's velocity to the knockback")] public float grabKbVelocityMult;

    [Header("References")]
    private NavMeshAgent agent;
    private ActorPhysics rb;
    [SerializeField] private Transform rotationTf;
    [SerializeField] private GameObject grabHitbox;

    [Header("Internal")]
    private float grabTimer;
    private bool isDecelerating;
    private Vector2 initialVelocity;
    private Vector2 targetVelocity;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<ActorPhysics>();
    }

    public override void OnEnter()
    {
        grabTimer = 0.001f;
        agent.enabled = false;
        ctr.PlayAnimation("Grab");

        Vector3 lookPos = ctr.ctx.player.transform.position;
        lookPos.y = agent.transform.position.y;
        rotationTf.transform.LookAt(lookPos);

        grabHitbox.SetActive(true);
        grabHitbox.transform.localPosition = (ctr.ctx.player.transform.position - rb.transform.position).normalized * hitboxOffset;

        Vector3 horizontalVel = ctr.ctx.player.transform.position - rb.transform.position;
        horizontalVel.y = 0;
        horizontalVel = horizontalVel.normalized * grabSpeed;
        rb.velocity = horizontalVel;
    }

    public override void OnUpdate(float deltaTime)
    {
        grabTimer += deltaTime;
        if (grabTimer > grabDuration)
        {
            if (!isDecelerating)
            {
                isDecelerating = true;
                Vector2 horizontalVel = new(rb.velocity.x, rb.velocity.z);
                initialVelocity = horizontalVel;
                targetVelocity = horizontalVel.normalized * grabEndSpeed;
            }
            if (grabTimer <= grabDuration + grabDeceleration)
            {
                var newVel = Vector2.Lerp(initialVelocity, targetVelocity, (grabTimer - grabDuration) / grabDeceleration);
                rb.velocity.x = newVel.x; rb.velocity.z = newVel.y;
            }
            if (grabTimer > grabDuration + grabDeceleration + grabEndLag)
            {
                grabTimer = 0;
                ctr.ExitAttack(this);
            }
            rb.Move(rb.velocity * deltaTime, true);
        }
    }

    public void OnHit()
    {
        if(ctr.currentAttack == this)
        {
            ctr.ctx.player.TakeDamage(grabDamage);
            ctr.ctx.player.TakeKnockback(rb.velocity * grabKbVelocityMult + rb.velocity.normalized * grabKbHorizontal + Vector3.up * grabKbVertical);
        }
    }

    public override void OnExit()
    {
        grabTimer = 0;
        isDecelerating = false;
        initialVelocity = Vector2.zero;
        targetVelocity = Vector2.zero;
        agent.enabled = true;

        grabHitbox.SetActive(false);

        rb.velocity = Vector3.zero;
    }

    protected override void Reset()
    {
        animationCodeNames.Add("Grab");
        base.Reset();
    }

}
