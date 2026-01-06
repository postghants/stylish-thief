using UnityEngine;

namespace HSM
{
    // Locks the player out of doing anything at all. Cannot be transitioned out of without an external ChangeState.
    public class PlayerFrozen : State
    {
        readonly PlayerContext ctx;

        public PlayerFrozen(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }
    // Entered when you hit the ground when stunned. Transitions to its parent when done.
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
            ctx.stunTimer = 0;
            ctx.currentMoveMult = 0;
            ctx.currentlyJumping = false;
            ctx.playerMat.color = ctx.stunnedColor;
        }

        protected override void OnExit()
        {
            ctx.currentMoveMult = 1;
            ctx.isStunned = false;
            ctx.playerMat.color = ctx.baseColor;
        }

        protected override State GetTransition(float deltaTime)
        {
            ctx.stunTimer += deltaTime;
            if (ctx.stunTimer >= ctx.stunDuration)
            {
                ctx.stunTimer = 0;
                return Parent;
            }
            return null;
        }
    }

    // Always entered first when stunned. Transitions to its parent when done.
    public class PlayerStunnedAirborne : State
    {
        readonly PlayerContext ctx;
        public PlayerStunnedAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.playerMat.color = ctx.stunnedColor;
            ctx.particleManager.StartGroup("Stun");
            ctx.anim.SetBool("Sliding", false);
            ctx.currentMoveMult = 0;
        }

        protected override void OnExit()
        {
            ctx.currentMoveMult = 1;
            ctx.playerMat.color = ctx.baseColor;
        }

        protected override void OnUpdate(float deltaTime)
        {
            ctx.stunTimer += deltaTime;
        }
    }

    // Entered when you hit the ground when sliding. Transitions to SlidingAirborne when you leave the ground.
    public class PlayerSliding : State
    {
        readonly PlayerContext ctx;

        public PlayerSliding(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
        protected override void OnEnter()
        {
            ctx.currentFriction = ctx.slideFriction;
            ctx.currentMoveMult = ctx.slideMoveMult;
            ctx.playerMat.color = ctx.slidingColor;

            ctx.rb.onCollision += OnCollision;
        }

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            if (Leaf() != this) { return; }
            Collision(hit, impactVelocity, ctx, Machine);
        }

        public static void Collision(RaycastHit hit, Vector3 impactVelocity, PlayerContext ctx, StateMachine machine)
        {
            if (ctx.isStunned) { return; }
            if (hit.normal.y > 0.1)
            {
                return;
            }
            Vector3 horizontalVel = impactVelocity; horizontalVel.y = 0;
            if (Vector3.Angle(horizontalVel, hit.normal) > ctx.maxSlideBonkAngle)
            {
                // velocity reflection code. busted! don't try at home
                //Vector3 newVel = Vector3.Reflect(horizontalVel, hit.normal) * ctx.stunDeceleration;
                //if (newVel == Vector3.zero)
                //{
                //    newVel = hit.normal * ctx.stunMinSpeed;
                //}
                //if (newVel.magnitude < ctx.stunMinSpeed)
                //{
                //    newVel = newVel.normalized * ctx.stunMinSpeed;
                //}

                Vector3 newVel = hit.normal * ctx.rb.velocity.magnitude * ctx.stunDeceleration;

                ctx.rb.velocity = newVel;
                ctx.rb.velocity.y += ctx.stunUpwardSpeed;
                ctx.isStunned = true;
                ctx.currentlyJumping = true;
                ctx.anim.SetBool("Sliding", false);
                machine.ChangeState(machine.Root.Leaf(), ((PlayerRoot)machine.Root).airborne.stunnedAirborne);
            }
        }

        protected override void OnExit()
        {
            ctx.currentMoveMult = 1;
            ctx.currentFriction = ctx.currentMoveData.friction;
            ctx.rb.onCollision -= OnCollision;
            ctx.hasGrabbed = false;
            ctx.playerMat.color = ctx.baseColor;
        }
        protected override State GetTransition(float deltaTime)
        {
            ctx.slideTimer += deltaTime;
            if (!ctx.pressingGrab && ctx.slideTimer >= ctx.minSlideTime)
            {
                ctx.slideTimer = 0;
                ctx.anim.SetBool("Sliding", false);
                return Parent;
            }
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

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            if (Leaf() != this) { return; }
            PlayerSliding.Collision(hit, impactVelocity, ctx, Machine);
        }

        protected override void OnEnter()
        {
            ctx.currentFriction = ctx.slideFriction;
            ctx.currentMoveMult = ctx.slideMoveMult;
            ctx.playerMat.color = ctx.slidingColor;

            ctx.anim.SetBool("Sliding", true);

            ctx.rb.onCollision += OnCollision;
        }
        protected override void OnExit()
        {
            ctx.currentMoveMult = 1;
            ctx.rb.onCollision -= OnCollision;
        }

        protected override State GetTransition(float deltaTime)
        {
            ctx.slideTimer += deltaTime;
            if (!ctx.pressingGrab && ctx.slideTimer >= ctx.minSlideTime)
            {
                ctx.slideTimer = 0;
                ctx.anim.SetBool("Sliding", false);
                return Parent;
            }
            return null;
        }
    }

    // Performing a grab.
    public class PlayerGrabbing : State
    {
        readonly PlayerContext ctx;
        private bool isDecelerating;
        private Vector2 initialVelocity;
        private Vector2 targetVelocity;
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
            ctx.playerMat.color = ctx.grabColor;

            ctx.anim.Play("grab");

            var grabbables = Physics.OverlapSphere(ctx.rb.transform.position, ctx.maxGrabTargetDistanceHorizontal);
            float bestAngle = 360;
            foreach (var grabbable in grabbables)
            {
                if (grabbable.TryGetComponent(out IGrabbable i))
                {
                    float angle = Vector3.Angle(ctx.facing, grabbable.transform.position - ctx.rb.transform.position);
                    float yDist = grabbable.transform.position.y - ctx.rb.transform.position.y;
                    Vector3 horizontalDist = new Vector3(grabbable.transform.position.x, 0, grabbable.transform.position.z) - new Vector3(ctx.rb.transform.position.x, 0, ctx.rb.transform.position.z);
                    if (angle < ctx.maxGrabTargetAngle
                       && yDist > ctx.maxGrabTargetDistanceDown && yDist < ctx.maxGrabTargetDistanceUp
                       && horizontalDist.magnitude < ctx.maxGrabTargetDistanceHorizontal)
                    {
                        bestAngle = angle;
                        ctx.facing = horizontalDist;
                    }
                }
            }

            Vector2 horizontalVel = new(ctx.facing.x, ctx.facing.z);
            if (horizontalVel.sqrMagnitude < ctx.grabSpeed * ctx.grabSpeed) { horizontalVel = horizontalVel.normalized * ctx.grabSpeed; }
            ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
            ctx.rb.velocity.y = 0;

            ctx.rb.onCollision += OnCollision;
        }

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            PlayerSliding.Collision(hit, impactVelocity, ctx, Machine);
        }

        protected override void OnUpdate(float deltaTime)
        {

        }

        protected override void OnExit()
        {
            ctx.grabTimer = 0;
            ctx.rb.onCollision -= OnCollision;
            ctx.useGravity = true;
            isDecelerating = false;
            initialVelocity = Vector2.zero;
            targetVelocity = Vector2.zero;
            ctx.playerMat.color = ctx.airColor;
        }

        protected override State GetTransition(float deltaTime)
        {
            ctx.grabTimer += deltaTime;
            if (ctx.grabTimer > ctx.grabDuration)
            {
                if (ctx.pressingGrab && !isDecelerating && (ctx.enableAirborneSlide || ctx.rb.isGrounded))
                {
                    if (!ctx.enableAirborneSlide && ctx.isStunned)
                        ctx.grabTimer = 0;
                    return ((PlayerAirborne)Parent).slidingAirborne;
                }
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

    // On the ground and standing still.
    public class PlayerIdle : State
    {
        readonly PlayerContext ctx;
        public PlayerIdle(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.anim.SetInteger("GroundSpeed", 0);
        }
        protected override State GetTransition(float deltaTime)
        {
            if (ctx.rb.velocity != Vector3.zero)
            {
                return ((PlayerGrounded)Parent).moving;
            }
            return null;
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

        protected override void OnEnter()
        {
            ctx.particleManager.StartGroup("Run");
        }

        protected override void OnExit()
        {
            ctx.particleManager.StopGroup("Run");
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ctx.moveInputValue != Vector2.zero)
            {
                float angle = Vector3.Angle(ctx.moveDirection, ctx.rb.velocity);
                ctx.rb.velocity *= 1 - (ctx.currentMoveData.turnDeceleration.Evaluate(angle / 180) * ctx.currentMoveData.turnDecelerationMult * deltaTime);
            }

            if (ctx.rb.velocity.magnitude > ctx.animRunSpeed)
            {
                ctx.anim.SetInteger("GroundSpeed", 2);
            }
            else
            {
                ctx.anim.SetInteger("GroundSpeed", 1);
            }
        }

        protected override State GetTransition(float deltaTime)
        {
            if (ctx.rb.velocity == Vector3.zero)
            {
                return ((PlayerGrounded)Parent).idle;
            }
            return null;
        }
    }

    // All grounded states are children of this state.
    public class PlayerGrounded : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerSliding sliding;
        public readonly PlayerMoving moving;
        public readonly PlayerIdle idle;
        public readonly PlayerStunned stunned;

        public PlayerGrounded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            moving = new(m, this, ctx);
            sliding = new(m, this, ctx);
            idle = new(m, this, ctx);
            stunned = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.hasGrabbed = false;
            ctx.currentMoveData = ctx.groundMoveData;
            ctx.currentFriction = ctx.currentMoveData.friction;
            ctx.currentMoveMult = 1;
            ctx.playerMat.color = ctx.baseColor;
            // Do animations or whatever
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.currentMoveData.acceleration * ctx.currentMoveMult * deltaTime * ctx.moveDirection;
            }
            else if (Leaf() != sliding)
            {
                ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.currentMoveData.deceleration;
            }

        }

        protected override State GetInitialState()
        {
            if (ctx.rb.velocity != Vector3.zero) { return moving; }
            else { return idle; }
        }
        protected override State GetTransition(float deltaTime)
        {
            if (ctx.desiredGrab && !ctx.hasGrabbed)
            {
                return ((PlayerRoot)Parent).airborne.grabbing;
            }
            if (!ctx.rb.isGrounded)
            {
                if (Leaf() == sliding)
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
        public readonly PlayerStunnedAirborne stunnedAirborne;

        public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            falling = new(m, this, ctx);
            grabbing = new(m, this, ctx);
            slidingAirborne = new(m, this, ctx);
            stunnedAirborne = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.currentMoveData = ctx.airMoveData;
            ctx.currentFriction = ctx.currentMoveData.friction;
            ctx.playerMat.color = ctx.airColor;
        }

        protected override void OnUpdate(float deltaTime)
        {
            ctx.coyoteTimeCounter += deltaTime;
            if (ctx.moveInputValue != Vector2.zero)
            {
                ctx.rb.velocity += ctx.currentJumpMoveMult * ctx.currentMoveData.acceleration * deltaTime * ctx.moveDirection;
            }

            if (ctx.useGravity)
            {
                ctx.rb.velocity.y += deltaTime * -ctx.baseGrav;
            }
        }

        protected override void OnExit()
        {
            ctx.coyoteTimeCounter = 0;
        }

        protected override State GetInitialState() => falling;
        protected override State GetTransition(float deltaTime)
        {
            if (!ctx.isStunned)
            {
                if (ctx.desiredGrab && !ctx.hasGrabbed)
                {
                    return grabbing;
                }
                if (ctx.grabTimer > 0)
                {
                    return null;
                }
            }
            if (ctx.rb.isGrounded)
            {
                if (Leaf() == slidingAirborne)
                {
                    return ((PlayerRoot)Parent).grounded.sliding;
                }
                if (Leaf() == stunnedAirborne)
                {
                    if (ctx.stunTimer > 0.1)
                    {
                        return ((PlayerRoot)Parent).grounded.stunned;
                    }
                    return null;
                }

                ctx.particleManager.StartGroup("Land");
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
        public readonly PlayerFrozen frozen;

        public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m)
        {
            grounded = new PlayerGrounded(m, this, ctx);
            airborne = new PlayerAirborne(m, this, ctx);
            frozen = new PlayerFrozen(m, this, ctx);
            this.ctx = ctx;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ctx.regenTimer >= ctx.regenDelay)
            {
                if (ctx.currentHealth < ctx.maxHealth)
                {
                    ctx.currentHealth = Mathf.Clamp(ctx.currentHealth + ctx.regenRate * deltaTime, 0, ctx.maxHealth);
                    ctx.healthBar.SetFill(ctx.currentHealth / ctx.maxHealth);
                }
            }
            else
            {
                ctx.regenTimer += deltaTime;
            }

            ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.currentFriction;

            Vector2 horizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);
            if (horizontalVel.magnitude > ctx.currentMoveData.maxSpeed && (Leaf() == grounded || Leaf() == grounded.moving || Leaf() == grounded.idle))
            {
                horizontalVel *= ctx.currentMoveData.maxSpeed / horizontalVel.magnitude;
                horizontalVel *= ctx.currentMoveData.speedCapMult;
                ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
            }


            if (ctx.rb.velocity.sqrMagnitude < 0.001f) { ctx.rb.velocity = Vector3.zero; }

            bool doGravityPass = !ctx.currentlyJumping;

            ctx.rb.Move(deltaTime * ctx.rb.velocity, doGravityPass);
        }

        protected override State GetInitialState() => airborne;
        protected override State GetTransition(float deltaTime)
        {
            ctx.currentVelocity = ctx.rb.velocity; //Reads the current speed we're shmoving at to make new calculations with
            if (ctx.desiredJump && Leaf() != airborne.grabbing)
            {
                if (Leaf() == airborne.slidingAirborne || Leaf() == grounded.sliding)
                {
                    if (ctx.disableSlideJump)
                    {
                        return null;
                    }
                    ctx.currentJumpData = ctx.slideJumpData;
                }
                else
                {
                    ctx.currentJumpData = ctx.baseJumpData;
                }
                Jump.PerformJump(ctx); //Resets jump preparations and calculates a new Y speed to jump with

                ctx.rb.velocity = ctx.currentVelocity; //Applies new Y speed as well as the X that was read earlier
            }
            Jump.CalculateGravity(ctx);

            return null;
        }
    }
}
