using HSM;
using UnityEngine;

// Behaviour during grab
public class GrabberGrabbing : State
{
    readonly GrabberContext ctx;
    private bool isDecelerating;
    private Vector2 initialVelocity;
    private Vector2 targetVelocity;
    public GrabberGrabbing(StateMachine m, State parent, GrabberContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }

    protected override void OnEnter()
    {
        ctx.hasGrabbed = true;
        ctx.grabTimer = 0.001f;
        ctx.agent.enabled = false;

        ctx.grabHitbox.gameObject.SetActive(true);
        ctx.grabHitbox.transform.localPosition = (ctx.player.transform.position - ctx.rb.transform.position).normalized * ctx.hitboxOffset;
        ctx.hitEvent += OnHit;

        Vector3 horizontalVel = ctx.player.transform.position - ctx.rb.transform.position;
        horizontalVel.y = 0;
        horizontalVel = horizontalVel.normalized * ctx.grabSpeed;
        ctx.rb.velocity = horizontalVel;
    }


    protected override void OnExit()
    {
        ctx.grabTimer = 0;
        isDecelerating = false;
        initialVelocity = Vector2.zero;
        targetVelocity = Vector2.zero;
        ctx.agent.enabled = true;

        ctx.grabHitbox.gameObject.SetActive(false);
        ctx.hitEvent -= OnHit;

        ctx.rb.velocity = Vector3.zero;
    }

    private void OnHit()
    {
        Debug.Log("Player Hit!");
        ctx.player.TakeDamage(ctx.grabDamage);
        ctx.player.TakeKnockback(ctx.rb.velocity * 5 + Vector3.up * 10);
    }

    protected override State GetTransition(float deltaTime)
    {
        ctx.grabTimer += deltaTime;
        if (ctx.grabTimer > ctx.grabDuration)
        {
            if (!isDecelerating)
            {
                isDecelerating = true;
                Vector2 horizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);
                initialVelocity = horizontalVel;
                targetVelocity = horizontalVel.normalized * ctx.grabEndSpeed;
            }
            if (ctx.grabTimer <= ctx.grabDuration + ctx.grabDeceleration)
            {
                var newVel = Vector2.Lerp(initialVelocity, targetVelocity, (ctx.grabTimer - ctx.grabDuration) / ctx.grabDeceleration);
                ctx.rb.velocity.x = newVel.x; ctx.rb.velocity.z = newVel.y;
            }
            if (ctx.grabTimer > ctx.grabDuration + ctx.grabDeceleration + ctx.grabEndLag)
            {
                ctx.grabTimer = 0;
                return Parent;
            }
        }
        return null;
    }
}

// Behaviour when player is spotted
public class GrabberChasing : State
{
    readonly GrabberContext ctx;
    public GrabberChasing(StateMachine m, State parent, GrabberContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }
    protected override void OnEnter()
    {
        ctx.agent.speed = ctx.maxSpeed;
        ctx.agent.acceleration = ctx.acceleration;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.agent.SetDestination(ctx.player.transform.position);
    }

    protected override State GetTransition(float deltaTime)
    {
        if (Vector3.Distance(ctx.rb.transform.position, ctx.player.transform.position) <= ctx.grabTriggerDistance)
        {
            return ((GrabberRoot)Parent).grabbing;
        }
        if (!ctx.playerInZone)
        {
            return ((GrabberRoot)Parent).idle;
        }
        return null;
    }

}

// Behaviour when not chasing player
public class GrabberIdle : State
{
    readonly GrabberContext ctx;

    private Vector3 destination = Vector3.zero;

    public GrabberIdle(StateMachine m, State parent, GrabberContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }

    protected override void OnEnter()
    {
        ctx.agent.speed = ctx.walkSpeed;
        ctx.agent.acceleration = ctx.walkAccel;
        destination = ctx.activeZone.RandomPointInZone();
        ctx.agent.SetDestination(destination);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (destination == Vector3.zero || (ctx.rb.transform.position.x == destination.x && ctx.rb.transform.position.z == destination.z))
        {
            destination = ctx.activeZone.RandomPointInZone();
            ctx.agent.SetDestination(destination);
        }
    }

    protected override State GetTransition(float deltaTime)
    {
        if (ctx.playerInZone)
        {
            return ((GrabberRoot)Parent).chasing;
        }
        else { return null; }
    }
}

public class GrabberRoot : State
{
    readonly GrabberContext ctx;
    public GrabberIdle idle;
    public GrabberChasing chasing;
    public GrabberGrabbing grabbing;
    public GrabberRoot(StateMachine m, GrabberContext ctx) : base(m)
    {
        this.ctx = ctx;
        idle = new(m, this, ctx);
        chasing = new(m, this, ctx);
        grabbing = new(m, this, ctx);
    }

    protected override State GetInitialState() => idle;
}
