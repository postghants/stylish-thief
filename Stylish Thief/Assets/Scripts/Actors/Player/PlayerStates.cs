using UnityEngine;

namespace HSM
{
    public class PlayerStunned : State
    {
        readonly PlayerContext ctx;
        public PlayerStunned(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.currentMoveMult = 0;
        }
    }

    // Entered when you hit the ground when sliding. Transitions to SlidingAirborne when you leave the ground.
    public class PlayerSliding : State
    {
        readonly PlayerContext ctx;

        private bool bonked;
        public PlayerSliding(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
        protected override void OnEnter()
        {
            ctx.currentFriction = ctx.slideFriction;
            ctx.currentMoveMult = ctx.slideMoveMult;

            ctx.rb.onCollision += OnCollision;
        }

        private void OnCollision(RaycastHit hit)
        {
            if(hit.normal.y > 0.1)
            {
                return;
            }

            Vector3 horizontalVel = ctx.rb.velocity; horizontalVel.y = 0;
            if(Vector3.Angle(horizontalVel, hit.normal) > ctx.maxSlideBonkAngle)
            {
                bonked = true;
            }
        }

        protected override void OnExit()
        {
            ctx.currentMoveMult = 1;
        }
        protected override State GetTransition()
        {
            if (bonked) { }
            if (!ctx.pressingGrab) { return Parent; }
            return null;
        }
    }

    // Always entered first when performing a slide out of a grab. Transitions to Sliding when you hit the ground.
    public class PlayerSlidingAirborne : State
    {
        readonly PlayerContext ctx;
        public PlayerSlidingAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.currentFriction = ctx.slideFriction;
            ctx.currentMoveMult = ctx.slideMoveMult;
        }

        protected override State GetTransition()
        {
            if (!ctx.pressingGrab) { return Parent; }
            return null;
        }
    }

    // Performing a grab.
    public class PlayerGrabbing : State
    {
        readonly PlayerContext ctx;
        public PlayerGrabbing(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.useGravity = false;
            ctx.hasGrabbed = true;
            ctx.grabTimer = 0.001f;
            ctx.currentFriction = ctx.grabFriction;

            Vector2 horizontalVel = new(ctx.facing.x, ctx.facing.z);
            if (horizontalVel.sqrMagnitude < ctx.grabSpeed * ctx.grabSpeed) { horizontalVel = horizontalVel.normalized * ctx.grabSpeed; }
            ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
            ctx.rb.velocity.y = 0;
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnExit()
        {
            ctx.useGravity = true;

        }

        protected override State GetTransition()
        {
            ctx.grabTimer += Time.fixedDeltaTime;
            if (ctx.grabTimer > ctx.grabDuration)
            {
                ctx.grabTimer = 0;
                if (ctx.pressingGrab)
                {
                    return ((PlayerAirborne)Parent).slidingAirborne;
                }
                ctx.rb.velocity *= 1 - ctx.grabDeceleration;
                return Parent;
            }
            return null;
        }
    }

    // On the ground and standing still.
    public class PlayerIdle : State
    {
        readonly PlayerContext ctx;
        public PlayerIdle(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

    // State entered when on the ground and walking.
    public class PlayerMoving : State
    {
        readonly PlayerContext ctx;
        public PlayerMoving(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

    // All grounded states are children of this state.
    public class PlayerGrounded : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerSliding sliding;
        public readonly PlayerMoving moving;
        public readonly PlayerIdle idle;

        public PlayerGrounded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            moving = new(m, this, ctx);
            sliding = new(m, this, ctx);
            idle = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.hasGrabbed = false;
            ctx.currentFriction = ctx.groundFriction;
            ctx.currentMoveMult = 1;
            // Do animations or whatever
        }

        protected override void OnUpdate()
        {
            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.acceleration * ctx.currentMoveMult * Time.deltaTime * ctx.moveDirection;
            }
            else if(Leaf() != sliding) 
            {
                ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.groundDeceleration;
            }

        }

        protected override State GetInitialState() => idle;
        protected override State GetTransition()
        {
            if (ctx.desiredGrab && !ctx.hasGrabbed)
            {
                return ((PlayerRoot)Parent).airborne.grabbing;
            }
            if (!ctx.rb.isGrounded)
            {
                if(Leaf() == sliding)
                {
                    return ((PlayerRoot)Parent).airborne.slidingAirborne;
                }
                return ((PlayerRoot)Parent).airborne;
            }
            return null;
        }
    }

    // Initial state entered when airborne.
    public class PlayerFalling : State
    {
        readonly PlayerContext ctx;

        public PlayerFalling(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

    // All airborne states are children of this state.
    public class PlayerAirborne : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerFalling falling;
        public readonly PlayerGrabbing grabbing;
        public readonly PlayerSlidingAirborne slidingAirborne;

        public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            falling = new(m, this, ctx);
            grabbing = new(m, this, ctx);
            slidingAirborne = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.currentFriction = ctx.airFriction;
        }

        protected override void OnUpdate()
        {
            ctx.coyoteTimeCounter += Time.fixedDeltaTime;
            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.airAccel * Time.deltaTime * ctx.moveDirection;
            }

            if (ctx.useGravity)
            {
                ctx.rb.velocity.y += Time.deltaTime * -ctx.baseGrav;
            }
        }

        protected override void OnExit()
        {
            ctx.coyoteTimeCounter = 0;
        }

        protected override State GetInitialState() => falling;
        protected override State GetTransition()
        {
            if (ctx.desiredGrab && !ctx.hasGrabbed)
            {
                return grabbing;
            }
            if (ctx.grabTimer > 0)
            {
                return null;
            }
            if (ctx.rb.isGrounded)
            {
                if(Leaf() == slidingAirborne)
                {
                    return ((PlayerRoot)Parent).grounded.sliding;
                }
                return ((PlayerRoot)Parent).grounded;

            }
            return null;
        }
    }

    // Root class. Does important physics stuff at the end of every update cycle.
    public class PlayerRoot : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerGrounded grounded;
        public readonly PlayerAirborne airborne;

        public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m)
        {
            grounded = new PlayerGrounded(m, this, ctx);
            airborne = new PlayerAirborne(m, this, ctx);
            this.ctx = ctx;
        }

        protected override void OnUpdate()
        {
            ActorPhysics rb = ctx.rb;

            ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.currentFriction;

            if (rb.velocity.sqrMagnitude < 0.001f) { rb.velocity = Vector3.zero; }

            bool doGravityPass = !ctx.currentlyJumping;

            rb.Move(Time.deltaTime * rb.velocity, doGravityPass);
        }

        protected override State GetInitialState() => grounded;
        protected override State GetTransition()
        {
            ctx.currentVelocity = ctx.rb.velocity; //Reads the current speed we're shmoving at to make new calculations with
            if (ctx.desiredJump)
            {
                Jump.PerformJump(ctx); //Resets jump preparations and calculates a new Y speed to jump with
                ctx.rb.velocity = ctx.currentVelocity; //Applies new Y speed as well as the X that was read earlier
                ctx.currentlyJumping = true; //Tells the code we're jumping now. Used for variable height
            }
            Jump.CalculateGravity(ctx);

            return null;
        }
    }
}
