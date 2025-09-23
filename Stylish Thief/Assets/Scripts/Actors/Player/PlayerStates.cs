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

    public class PlayerIdle : State
    {
        readonly PlayerContext ctx;
        public PlayerIdle(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

    public class PlayerGrounded : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerIdle idle;
        public readonly PlayerMoving moving;

        public PlayerGrounded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
            idle = new(m, this, ctx);
            moving = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            // Do animations or whatever
        }

        protected override void OnUpdate()
        {
            float targetAngle = Mathf.Atan2(ctx.moveInputValue.x, ctx.moveInputValue.y) * Mathf.Rad2Deg + ctx.cam.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * ctx.moveInputValue.magnitude;

            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.acceleration * Time.deltaTime * moveDirection;
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
            if (!ctx.rb.isGrounded)
            {
                Debug.Log("Trying to enter airborne");
                return ((PlayerRoot)Parent).airborne;
            }
            return null;
        }
    }

    public class PlayerAirborne : State
    {
        readonly PlayerContext ctx;
        public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnUpdate()
        {
            float targetAngle = Mathf.Atan2(ctx.moveInputValue.x, ctx.moveInputValue.y) * Mathf.Rad2Deg + ctx.cam.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * ctx.moveInputValue.magnitude;

            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.airAccel * Time.deltaTime * moveDirection;
            }

            ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.airFriction;

            ctx.rb.velocity.y += Time.deltaTime * -ctx.baseGrav;
        }

        protected override State GetTransition()
        {
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

            Vector2 horizontalVel = new Vector2(rb.velocity.x, rb.velocity.z);
            horizontalVel = Vector2.ClampMagnitude(horizontalVel, ctx.maxSpeed);
            rb.velocity.x = horizontalVel.x; rb.velocity.z = horizontalVel.y;
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
