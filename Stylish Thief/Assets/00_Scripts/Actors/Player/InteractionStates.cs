using UnityEngine;
using UnityEngine.InputSystem;

namespace HSM
{
    //The upward motion when grabbing an umbrella.
    public class IntUmbrellaLaunch : State
    {
        readonly PlayerContext ctx;
        float timer = 0f;
        //public AnimationCurve turnDeceleration;

        public IntUmbrellaLaunch(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.intUmbrellaMoveData;
            ctx.useGravity = false;
            ctx.hasGrabbed = true;
            ctx.player.SetTrigger("IntStartUmbrella");
        }
        protected override void OnUpdate(float deltaTime)
        {
            if (timer < ctx.umbrellaDuration)
            {
                timer += deltaTime;
            }
            Vector3 flightSpeed = ctx.rb.velocity;
            flightSpeed.y = ctx.umbrellaAcceleration.Evaluate(timer / ctx.umbrellaDuration) * ctx.umbrellaSpeed;
            ctx.rb.velocity = flightSpeed;
        }
        protected override void OnExit()
        {
            timer = 0;
            ctx.useGravity = true;
        }

        protected override State GetTransition(float deltaTime)
        {
            if (timer >= ctx.umbrellaDuration)
            {
                return ((PlayerAirborne)Parent).umbrellaGlide;
            }
            return null;
        }
    }

    //The glide following IntUmbrellaLaunch.
    public class IntUmbrellaGlide : State
    {
        readonly PlayerContext ctx;
        float timer = 0f;

        public IntUmbrellaGlide(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.intUmbrellaMoveData;
            ctx.useGravity = false;
            ctx.hasGrabbed = false;
            ctx.currentlyJumping = true;
            //start glide animation
        }
        protected override void OnUpdate(float deltaTime)
        {
            if (timer < ctx.umbrellaFloatAccelDuration)
            {
                timer += deltaTime;
            }
            Vector3 floatSpeed = ctx.rb.velocity;
            floatSpeed.y = ctx.umbrellaFloatAcceleration.Evaluate(timer / ctx.umbrellaFloatAccelDuration) * -ctx.umbrellaFloatSpeed;
            ctx.rb.velocity = floatSpeed;
        }
        protected override void OnExit()
        {
            timer = 0;
            ctx.useGravity = true;
            ctx.disableJump = false;
            ctx.player.SetTrigger("IntEndUmbrella");
        }

        protected override State GetTransition(float deltaTime)
        {
            if (timer >= ctx.umbrellaMaxDuration || ctx.desiredJump || ctx.rb.isGrounded)
            {
                return ctx.player.Root;
            }
            return null;
        }
    }

    public class IntPoleSpin : State
    {
        readonly PlayerContext ctx;
        float timer = 0f;
        Vector3 speed = Vector3.zero;
        float speedMagnitude;

        public IntPoleSpin(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.intPoleMoveData;
            ctx.useGravity = false;
            ctx.hasGrabbed = true;
            ctx.currentlyJumping = false;
            //start pole spin animation
            speed = ctx.rb.velocity;
            speed.y = 0;
            speedMagnitude = speed.magnitude;
            ctx.rb.velocity = Vector3.zero;

            if (speedMagnitude > ctx.groundMoveData.maxSpeed)
            {
                ctx.player.SetTrigger("StartPoleSpinFast");
            }
            else
            {
                ctx.player.SetTrigger("StartPoleSpinSlow");
            }
        }
        protected override void OnUpdate(float deltaTime)
        {
            if (speedMagnitude > ctx.groundMoveData.maxSpeed)
            {
                if (timer < ctx.speedPreservationTime)
                {
                    timer += deltaTime;
                }
                else
                {
                    speedMagnitude = ctx.groundMoveData.maxSpeed;
                    ctx.player.SetTrigger("StartPoleSpinSlow");
                }
            }
            speed = ctx.moveDirection * speedMagnitude;
        }
        protected override void OnExit()
        {
            timer = 0;
            ctx.useGravity = true;
            ctx.disableJump = false;
            ctx.hasGrabbed = false;
            ctx.rb.velocity = speed;
            ctx.facing = ctx.moveDirection;
            ctx.player.SetTrigger("EndPoleSpin");
        }

        protected override State GetTransition(float deltaTime)
        {
            if (ctx.desiredJump || ctx.desiredGrab)
            {
                ctx.desiredGrab = false;
                return ctx.player.Root;
            }
            return null;
        }
    }

    public class IntParry : State
    {
        readonly PlayerContext ctx;
        float entrySpeed;
        bool jumped;

        public IntParry(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.parryCount++;
            ctx.savedVelocity = ctx.rb.velocity;
            ctx.savedVelocity.y = 0;
            entrySpeed = ctx.savedVelocity.magnitude;
            ctx.cmd = ctx.intParryMoveData;
            ctx.currentJumpData = ctx.intParryJumpData;
            ctx.hasGrabbed = false;
            //ANIMAITON TRIGGER
            ctx.disablePound = true;
            ctx.disableRoll = true;
            ctx.rb.isGrounded = true;
            Jump.PerformJump(ctx);
            ctx.rb.isGrounded = false;
            jumped = true;
            Vector3 velocity = ctx.rb.velocity;
            velocity.x = 0;
            velocity.z = 0;
            ctx.rb.velocity = velocity;
        }
        protected override void OnUpdate(float deltaTime)
        {
            ctx.rb.IsGrounded();
        }
        protected override void OnExit()
        {
            ctx.disablePound = false;
            ctx.disableRoll = false;
            jumped = false;

            if (ctx.rb.isGrounded)
            {
                Vector3 velocity = ctx.facing.normalized * (entrySpeed + (ctx.parryCount * ctx.parryBoost));
                velocity.y = 0;
                ctx.rb.velocity = velocity;
                Debug.Log(ctx.rb.velocity);
                ctx.parryCount = 0;
            }
            if (ctx.desiredGrab)
            {
                Debug.Log(ctx.rb.velocity);
            }
        }

        protected override State GetTransition(float deltaTime)
        {
            return null;
        }
    }

    public class IntDrowning : State
    {
        readonly PlayerContext ctx;
        private float timer;

        public IntDrowning(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.intDrowningMovement;
            ctx.currentJumpData = ctx.intDrowningJumpData;
        }
        protected override void OnUpdate(float deltaTime)
        {
            if (timer < ctx.drowningTime)
            {
                timer += deltaTime;
            }
        }
        protected override void OnExit()
        {
            timer = 0;
        }

        protected override State GetTransition(float deltaTime)
        {
            if (timer >= ctx.drowningTime)
            {
                timer = 0;
                //REPLACE BELOW WITH TELEPORTING TO THE LAST SAFE LOCATION
                ctx.rb.transform.position = new Vector3(0, 10, 0);
                return ctx.player.Root;
            }
            return null;
        }
    }

    public class IntBarSwing : State
    {
        readonly PlayerContext ctx;
        private InputAction jump;
        private InputAction trick;
        private InputAction grab;
        private Vector3 entryVelocity;
        private float timer;

        public IntBarSwing(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.useGravity = false;
            entryVelocity = ctx.rb.velocity;
            if (!ctx.convertYSpeed)
            {
                entryVelocity.y = 0;
            }
            ctx.rb.velocity = Vector3.zero;
            ctx.cmd = ctx.intBarSwingMovement;
            ctx.hasGrabbed = true;
            ctx.player.SetTrigger("StartBarSwing");

            jump = InputSystem.actions.FindAction("Jump");
            trick = InputSystem.actions.FindAction("Trick");
            grab = InputSystem.actions.FindAction("Grab");
        }
        protected override void OnUpdate(float deltaTime)
        {
            if (trick.WasPerformedThisFrame() || grab.WasPerformedThisFrame())
            {
                Vector3 flippedRotation = ctx.anim.transform.rotation.eulerAngles;
                flippedRotation.y += 180;
                ctx.anim.transform.rotation = Quaternion.Euler(flippedRotation);
            }
            if (timer < ctx.barSpeedPreservationTime)
            {
                timer += deltaTime;
            }
            else
            {
                entryVelocity = Vector3.zero;
            }
        }
        protected override void OnExit()
        {
            if (ctx.desiredJump)
            {
                ctx.useGravity = true;
                ctx.rb.isGrounded = true;
                ctx.currentJumpData = ctx.intBarSwingJumpData;
                Jump.PerformJump(ctx);
                Vector3 velocity = ctx.anim.transform.forward * ctx.barMinimumSpeed;
                if (entryVelocity.magnitude > ctx.barMinimumSpeed)
                {
                    velocity = ctx.anim.transform.forward * entryVelocity.magnitude;
                }
                Debug.Log(entryVelocity.magnitude);
                velocity.y = 0;
                ctx.rb.velocity += velocity;
            }
            if (ctx.rb.isGrounded)
            {
                ctx.useGravity = true;
            }
            timer = 0;
            ctx.hasGrabbed = false;
        }

        protected override State GetTransition(float deltaTime)
        {
            if (ctx.rb.isGrounded)
            {
                return ctx.player.Root;
            }
            else if (ctx.desiredJump)
            {
                return ctx.player.Root.airborne;
            }
            return null;
        }
    }
}
