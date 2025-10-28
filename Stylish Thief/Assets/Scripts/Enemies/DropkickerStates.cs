using HSM;
using UnityEngine;

public class DropkickerKicking : State
{
    readonly DropkickerContext ctx;
    public DropkickerKicking(StateMachine m, State parent, DropkickerContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
    }
    protected override void OnEnter()
    {
        EnemyJump.PerformJump(ctx);
    }

    protected override void OnUpdate(float deltaTime)
    {
        EnemyJump.SetPhysics(ctx);
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
        //ctx.animator.SetInteger("WalkOrRun", 1);
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
        //if (Vector3.Distance(ctx.rb.transform.position, ctx.player.transform.position) <= ctx.grabTriggerDistance)
        //{
        //    return ((GrabberRoot)Parent).grabbing;
        //}
        //if (!ctx.playerInZone)
        //{
        //    return ((DropkickerRoot)Parent).idle;
        //}
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
        //ctx.animator.SetInteger("WalkOrRun", 0);
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
    public DropkickerRoot(StateMachine m, DropkickerContext ctx) : base(m)
    {
        this.ctx = ctx;
        idle = new(m, this, ctx);
        chasing = new(m, this, ctx);
    }

    protected override State GetInitialState() => idle;

}
