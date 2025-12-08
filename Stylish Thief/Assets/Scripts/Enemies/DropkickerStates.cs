using HSM;
using UnityEngine;

public class DropkickerKicking : State
{
    readonly DropkickerContext ctx;
    private float timer = 0;
    private float recoveryTimer = 0;
    public DropkickerKicking(StateMachine m, State parent, DropkickerContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }
    protected override void OnEnter()
    {
        ctx.animator.SetTrigger("Kick");
        ctx.agent.enabled = false;
        ctx.currentJumpData = ctx.dropkickData;
        EnemyJump.PerformJump(ctx);
        ctx.rb.velocity += ctx.currentVelocity;
        ctx.gravMultiplier = ctx.currentJumpData.upwardMovementMultiplier;

        Vector3 dist = ctx.player.transform.position - ctx.rb.transform.position;
        dist.y = 0;
        ctx.rb.velocity += ctx.dropkickSpeed * dist.normalized;

        Vector3 lookPos = ctx.player.transform.position;
        lookPos.y = ctx.agent.transform.position.y;
        ctx.animator.transform.LookAt(lookPos);
    }

    protected override void OnExit()
    {
        timer = 0;
        recoveryTimer = 0;

        ctx.animator.SetTrigger("GetUp");
        ctx.agent.enabled = true;
        ctx.rb.velocity = Vector3.zero;
        ctx.rb.isGrounded = false;
        ctx.grabHitbox.gameObject.SetActive(false);
    }

    protected override void OnUpdate(float deltaTime)
    {
        EnemyJump.SetPhysics(ctx); 
        EnemyJump.CalculateGravity(ctx);
        ctx.rb.velocity += ctx.baseGrav * ctx.gravMultiplier * Time.deltaTime * ctx.rb.gravity;
        ctx.rb.Move(ctx.rb.velocity * Time.deltaTime, false);

        if(timer > ctx.hitboxDelay)
        {
            if (timer < ctx.hitboxDelay + ctx.hitboxActiveTime)
            {
                ctx.grabHitbox.gameObject.SetActive(true);
            }
            else
            {
                ctx.grabHitbox.gameObject.SetActive(false);
            }
        }

        if (ctx.rb.isGrounded)
        {
            recoveryTimer += deltaTime;
            ctx.rb.velocity = Vector3.zero;
        }
        ctx.rb.isGrounded = ctx.rb.IsGrounded();
    }

    protected override State GetTransition(float deltaTime)
    {
        timer += deltaTime;
        if(recoveryTimer > ctx.recoveryTime - 0.6)
        {
            ctx.animator.SetTrigger("GetUp");
        }
        if(recoveryTimer > ctx.recoveryTime)
        {
            return ((DropkickerRoot)Parent).chasing;
        }
        return null;
    }

}

public class DropkickerChasing : State
{
    readonly DropkickerContext ctx;
    public DropkickerChasing(StateMachine m, State parent, DropkickerContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }

    protected override void OnEnter()
    {
        ctx.agent.speed = ctx.maxSpeed;
        ctx.agent.acceleration = ctx.acceleration;
        ctx.animator.SetBool("Chasing", true);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.agent.SetDestination(ctx.player.transform.position);
        Vector3 lookPos = ctx.player.transform.position;
        lookPos.y = ctx.agent.transform.position.y;
        ctx.animator.transform.LookAt(lookPos);
    }

    protected override State GetTransition(float deltaTime)
    {
        if (Vector3.Distance(ctx.rb.transform.position, ctx.player.transform.position) <= ctx.dropkickDistance)
        {
            return ((DropkickerRoot)Parent).kicking;
        }
        if (!ctx.playerInZone)
        {
            return ((DropkickerRoot)Parent).idle;
        }
        return null;
    }

}

public class DropkickerIdle : State
{
    readonly DropkickerContext ctx;

    private Vector3 destination = Vector3.zero;

    public DropkickerIdle(StateMachine m, State parent, DropkickerContext ctx) : base(m)
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
        Vector3 lookPos = destination;
        lookPos.y = ctx.agent.transform.position.y;
        ctx.animator.transform.LookAt(lookPos);
        ctx.animator.SetBool("Chasing", false);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (destination == Vector3.zero || (ctx.rb.transform.position.x == destination.x && ctx.rb.transform.position.z == destination.z))
        {
            destination = ctx.activeZone.RandomPointInZone();
            ctx.agent.SetDestination(destination);
            Vector3 lookPos = destination;
            lookPos.y = ctx.agent.transform.position.y;
            ctx.animator.transform.LookAt(lookPos);
        }
    }

    protected override State GetTransition(float deltaTime)
    {
        if (ctx.playerInZone)
        {
            return ((DropkickerRoot)Parent).chasing;
        }
        else { return null; }
    }
}

public class DropkickerRoot : State
{
    readonly DropkickerContext ctx;

    public DropkickerIdle idle;
    public DropkickerChasing chasing;
    public DropkickerKicking kicking;
    public DropkickerRoot(StateMachine m, DropkickerContext ctx) : base(m)
    {
        this.ctx = ctx;
        idle = new(m, this, ctx);
        chasing = new(m, this, ctx);
        kicking = new(m, this, ctx);
    }

    protected override State GetInitialState() => idle;

}
