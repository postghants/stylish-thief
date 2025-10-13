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

        Vector2 horizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);
        if (horizontalVel.sqrMagnitude < ctx.grabSpeed * ctx.grabSpeed) { horizontalVel = horizontalVel.normalized * ctx.grabSpeed; }
        ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
        ctx.rb.velocity.y = 0;
    }

    protected override void OnExit()
    {
        ctx.grabTimer = 0;
        isDecelerating = false;
        initialVelocity = Vector2.zero;
        targetVelocity = Vector2.zero;
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
}

// Behaviour when not chasing player
public class GrabberIdle : State
{
    readonly GrabberContext ctx;
    public GrabberIdle(StateMachine m, State parent, GrabberContext ctx) : base(m)
    {
        this.ctx = ctx;
        Parent = parent;
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
