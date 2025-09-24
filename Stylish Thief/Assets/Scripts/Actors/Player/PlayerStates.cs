using HSM;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.EventSystems;
namespace HSM
{
    public class PlayerMoving : State
    {
        readonly PlayerContext ctx;
        public PlayerMoving(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

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

            ctx.rb.velocity *= 1 - ctx.grabDeceleration;
        }

        protected override State GetTransition()
        {
            ctx.grabTimer += Time.fixedDeltaTime;
            if (ctx.grabTimer > ctx.grabDuration)
            {
                ctx.grabTimer = 0;
                return Parent;
            }
            return null;
        }
    }

    public class PlayerGrounded : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerMoving moving;

        public PlayerGrounded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
            moving = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.hasGrabbed = false;

            // Do animations or whatever
        }

        protected override void OnUpdate()
        {
            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.acceleration * Time.deltaTime * ctx.moveDirection;
            }
            else
            {
                ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.groundDeceleration;
            }

            ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.groundFriction;

        }

        //protected override State GetInitialState() => idle;
        protected override State GetTransition()
        {
            if (ctx.desiredGrab && !ctx.hasGrabbed)
            {
                return ((PlayerRoot)Parent).airborne.grabbing;
            }
            if (!ctx.rb.isGrounded)
            {
                return ((PlayerRoot)Parent).airborne;
            }
            return null;
        }
    }

    public class PlayerAirborne : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerGrabbing grabbing;

        public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            grabbing = new(m, this, ctx);
        }

        protected override void OnUpdate()
        {

            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.airAccel * Time.deltaTime * ctx.moveDirection;
            }

            ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.airFriction;

            if (ctx.useGravity)
            {
                ctx.rb.velocity.y += Time.deltaTime * -ctx.baseGrav;
            }
        }

        protected override State GetTransition()
        {
            if (ctx.desiredGrab && !ctx.hasGrabbed)
            {
                return grabbing;
            }
            if(ctx.grabTimer > 0)
            {
                return null;
            }
            return ctx.rb.isGrounded ? ((PlayerRoot)Parent).grounded : null;
        }
    }

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
            CustomBoxRigidbody rb = ctx.rb;

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
                PlayerMove.PerformJump(ctx); //Resets jump preparations and calculates a new Y speed to jump with
                ctx.rb.velocity = ctx.currentVelocity; //Applies new Y speed as well as the X that was read earlier
                ctx.currentlyJumping = true; //Tells the code we're jumping now. Used for variable height
            }
            PlayerMove.CalculateGravity(ctx);

            return null;
        }
    }
}
