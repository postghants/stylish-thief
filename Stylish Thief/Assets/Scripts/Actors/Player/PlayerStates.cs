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
            ctx.currentMoveMult = ctx.slideMoveMult;
            ctx.playerMat.color = ctx.slidingColor;

            ctx.rb.onCollision += OnCollision;
        }

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            //if (Leaf() != this)
            //{
            //    ctx.rb.onCollision -= OnCollision;
            //    return;
            //}
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
            //if (Leaf() != this)
            //{
            //    ctx.rb.onCollision -= OnCollision;
            //    return;
            //}
            PlayerSliding.Collision(hit, impactVelocity, ctx, Machine);
        }

        protected override void OnEnter()
        {
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

    // Holds the player still while vaulting before exiting into jump or ground
    public class PlayerVaulting : State
    {
        readonly PlayerContext ctx;
        private float timer;
        private Vector3 startVel;
        public PlayerVaulting(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            startVel = ctx.rb.velocity;
            ctx.rb.velocity = Vector3.zero;
            ctx.anim.Play("Vault");
        }

        protected override void OnExit()
        {
            if (ctx.pressingGrab && !ctx.disableVaultJump)
            {
                ctx.rb.velocity = startVel;
                ctx.currentJumpData = ctx.vaultJump;
                Jump.PerformJump(ctx);
                ctx.anim.Play("GrabEndAerial");
                ctx.particleManager.StartGroup("Vault");
            }
            else
            {
                ctx.anim.Play("GrabEndGround");
            }
            timer = 0;
        }

        protected override State GetTransition(float deltaTime)
        {
            timer += deltaTime;
            if (ctx.pressingGrab || timer >= ctx.vaultMaxDuration)
            {
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
        private bool addedCollisionEvent = false;
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
            ctx.playerMat.color = ctx.grabColor;
            addedCollisionEvent = false;

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
                    if (angle < ctx.maxGrabTargetAngle && angle < bestAngle
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


            ctx.particleManager.StartGroup("grab");
        }

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            //if (Leaf() != ((PlayerAirborne)Parent).grabbing)
            //{
            //    ctx.rb.onCollision -= OnCollision;
            //    return;
            //}
            PlayerSliding.Collision(hit, impactVelocity, ctx, Machine);
        }

        protected override void OnUpdate(float deltaTime)
        {

            //Find ledge for vaulting
            Vector3 origin = ctx.rb.transform.position;
            Bounds bounds = ctx.rb.environmentCollider.bounds;
            bounds.Expand(-ctx.rb.skinWidth * 2);

            if (!ctx.disableVault && Physics.BoxCast(origin, bounds.extents, ctx.rb.velocity, out RaycastHit checkHit, Quaternion.identity, ctx.ledgeCheckDistance, ctx.rb.groundMask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(checkHit.normal, -ctx.rb.velocity) < ctx.rb.maxSlopeAngle)
                {
                    origin = checkHit.point;
                    origin.y += ctx.maxLedgeHeight;
                    if (Physics.OverlapSphere(origin, 0.1f, ctx.rb.collisionLayerMask, QueryTriggerInteraction.Ignore).Length == 0)
                    {
                        if (Physics.BoxCast(origin, bounds.extents, Vector3.down, out RaycastHit heightHit, Quaternion.identity, ctx.maxLedgeHeight, ctx.rb.groundMask, QueryTriggerInteraction.Ignore))
                        {
                            origin = heightHit.point;
                            origin.y += ctx.rb.environmentCollider.bounds.extents.y + ctx.rb.skinWidth;
                            ctx.rb.transform.position = origin;

                            Machine.ChangeState(this, ((PlayerAirborne)Parent).vaulting);
                            return;
                        }
                    }
                }
            }

            if (!addedCollisionEvent)
            {
                addedCollisionEvent = true;
                ctx.rb.onCollision += OnCollision;
            }
        }

        protected override void OnExit()
        {
            ctx.particleManager.StopGroup("grab");
            ctx.grabTimer = 0;
            try
            {
                ctx.rb.onCollision -= OnCollision;
            }
            catch { }
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
                if (ctx.pressingGrab && !isDecelerating && !ctx.disableSlide && (!ctx.disableAirborneSlide || ctx.rb.isGrounded))
                {
                    if (ctx.disableAirborneSlide && ctx.isStunned)
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

    public class PlayerRolling : State
    {
        readonly PlayerContext ctx;
        private bool isDecelerating;
        private Vector2 initialVelocity;
        private Vector2 targetVelocity;
        private bool addedCollisionEvent = false;
        public PlayerRolling(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.airMoveData;
            ctx.useGravity = false;
            ctx.rollTimer = 0.001f;
            addedCollisionEvent = false;
            ctx.blockJump = 1;

            ctx.anim.Play("grabEndAerial");

            Vector2 horizontalVel = new(ctx.facing.x, ctx.facing.z);
            if (horizontalVel.sqrMagnitude < ctx.rollSpeed * ctx.rollSpeed) { horizontalVel = horizontalVel.normalized * ctx.rollSpeed; }
            ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
            ctx.rb.velocity.y = 0;
        }

        private void OnCollision(RaycastHit hit, Vector3 impactVelocity)
        {
            //if (Leaf() != ((PlayerAirborne)Parent).rollbing)
            //{
            //    ctx.rb.onCollision -= OnCollision;
            //    return;
            //}
            PlayerSliding.Collision(hit, impactVelocity, ctx, Machine);
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (!addedCollisionEvent)
            {
                addedCollisionEvent = true;
                ctx.rb.onCollision += OnCollision;
            }
        }

        protected override void OnExit()
        {
            ctx.blockJump--;
            ctx.rollTimer = 0;
            try
            {
                ctx.rb.onCollision -= OnCollision;
            }
            catch { }
            ctx.useGravity = true;
            isDecelerating = false;
            initialVelocity = Vector2.zero;
            targetVelocity = Vector2.zero;
            ctx.playerMat.color = ctx.airColor;
        }

        protected override State GetTransition(float deltaTime)
        {
            ctx.rollTimer += deltaTime;

            if (ctx.pressingJump && !ctx.disableRollJump)
            {
                ctx.currentJumpData = ctx.rollJump;
                Jump.PerformJump(ctx);
            }
            if (ctx.rollTimer > ctx.rollDuration)
            {
                if (!isDecelerating)
                {
                    isDecelerating = true;
                    Vector2 horizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);
                    initialVelocity = horizontalVel;
                    targetVelocity = horizontalVel.normalized * ctx.rollEndSpeed;
                }
                if (ctx.rollTimer <= ctx.rollDuration + ctx.rollDeceleration)
                {
                    var newVel = Vector2.Lerp(initialVelocity, targetVelocity, (ctx.rollTimer - ctx.rollDuration) / ctx.rollDeceleration);
                    ctx.rb.velocity.x = newVel.x; ctx.rb.velocity.z = newVel.y;
                }
                if (ctx.rollTimer > ctx.rollDuration + ctx.rollDeceleration + ctx.rollEndLag)
                {
                    ctx.rollTimer = 0;
                    return Parent;
                }
            }
            return null;
        }
    }

    public class PlayerHarshLanded : State
    {
        readonly PlayerContext ctx;
        private float timer;
        public PlayerHarshLanded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }

        protected override void OnEnter()
        {
            timer = 0;
            ctx.landingSpeed = 0;
            ctx.blockJump++;
            ctx.pressingJump = false;

            if (!ctx.disableRoll && ctx.jumpBufferCounter > 0 && ctx.jumpBufferCounter < ctx.rollTiming)
            {
                Machine.ChangeState(this, ((PlayerGrounded)Parent).rolling);
                return;
            }

            ctx.player.TakeDamage(ctx.harshLandingDamage);
            ctx.cmd = ctx.harshLandingData;
        }

        protected override void OnExit()
        {
            ctx.blockJump = 0;
        }

        protected override State GetTransition(float deltaTime)
        {
            timer += deltaTime;
            if (timer >= ctx.harshLandingDuration)
            {
                ctx.blockJump--;
                ctx.cmd = ctx.groundMoveData;
                return ((PlayerGrounded)Parent).moving;
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
            if (ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("Run 2") || ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") || ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("JumpUp"))
            {
                ctx.anim.Play("Idle");
            }
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
                ctx.anim.Play("Run 2");
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
        public readonly PlayerHarshLanded harshLanded;
        public readonly PlayerRolling rolling;

        public PlayerGrounded(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            moving = new(m, this, ctx);
            sliding = new(m, this, ctx);
            idle = new(m, this, ctx);
            stunned = new(m, this, ctx);
            harshLanded = new(m, this, ctx);
            rolling = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.hasGrabbed = false;
            ctx.cmd = ctx.groundMoveData;
            ctx.currentMoveMult = 1;
            ctx.playerMat.color = ctx.baseColor;
            // Do animations or whatever
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ctx.moveInputValue != Vector2.zero)
            {
                float angle = Vector3.Angle(ctx.moveDirection, ctx.rb.velocity);

                Vector2 currentHorizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);

                currentHorizontalVel -= ctx.cmd.turnDeceleration.Evaluate(angle / 180) * ctx.cmd.turnDecelerationMult * deltaTime * currentHorizontalVel;
                ctx.rb.velocity.x = currentHorizontalVel.x; ctx.rb.velocity.z = currentHorizontalVel.y;

                Vector3 acceleration = ctx.currentJumpMoveMult * ctx.cmd.acceleration * deltaTime * ctx.moveDirection;
                ctx.rb.velocity += acceleration;

                Vector3 turnSpeed = ctx.cmd.turnSpeedMult * acceleration;
                Vector2 newHorizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);

                newHorizontalVel.x += turnSpeed.x; newHorizontalVel.y += turnSpeed.z;

                if (newHorizontalVel.magnitude > ctx.cmd.maxSpeed)
                {
                    newHorizontalVel = newHorizontalVel.normalized * Mathf.Clamp(currentHorizontalVel.magnitude, ctx.cmd.maxSpeed, Mathf.Infinity);
                }
                ctx.rb.velocity.x = newHorizontalVel.x; ctx.rb.velocity.z = newHorizontalVel.y;
            }
            else
            {
                ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.cmd.deceleration * deltaTime;
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

        protected override void OnUpdate(float deltaTime)
        {
            if (ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("Run 2") || ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") || ctx.anim.GetCurrentAnimatorStateInfo(0).IsName("JumpUp"))
                if (ctx.rb.velocity.y > 0)
                {
                    ctx.anim.Play("JumpUp");
                }
                else
                {
                    ctx.anim.Play("Fall");
                }
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
        public readonly PlayerVaulting vaulting;

        public PlayerAirborne(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;

            falling = new(m, this, ctx);
            grabbing = new(m, this, ctx);
            slidingAirborne = new(m, this, ctx);
            stunnedAirborne = new(m, this, ctx);
            vaulting = new(m, this, ctx);
        }

        protected override void OnEnter()
        {
            ctx.cmd = ctx.airMoveData;
            ctx.playerMat.color = ctx.airColor;
        }

        protected override void OnUpdate(float deltaTime)
        {
            ctx.coyoteTimeCounter += deltaTime;

            if (ctx.moveInputValue != Vector2.zero && Leaf() != vaulting)
            {
                float angle = Vector3.Angle(ctx.moveDirection, ctx.rb.velocity);

                Vector2 currentHorizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);


                currentHorizontalVel -= ctx.cmd.turnDeceleration.Evaluate(angle / 180) * ctx.cmd.turnDecelerationMult * deltaTime * currentHorizontalVel;
                ctx.rb.velocity.x = currentHorizontalVel.x; ctx.rb.velocity.z = currentHorizontalVel.y;

                Vector3 acceleration = ctx.currentJumpMoveMult * ctx.cmd.acceleration * deltaTime * ctx.moveDirection;
                ctx.rb.velocity += acceleration;

                Vector3 turnSpeed = ctx.cmd.turnSpeedMult * acceleration;
                Vector2 newHorizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);

                newHorizontalVel.x += turnSpeed.x; newHorizontalVel.y += turnSpeed.z;

                if (newHorizontalVel.magnitude > ctx.cmd.maxSpeed)
                {
                    newHorizontalVel = newHorizontalVel.normalized * Mathf.Clamp(currentHorizontalVel.magnitude, ctx.cmd.maxSpeed, Mathf.Infinity);
                }
                ctx.rb.velocity.x = newHorizontalVel.x; ctx.rb.velocity.z = newHorizontalVel.y;
            }
            else
            {
                ctx.rb.velocity += new Vector3(-ctx.rb.velocity.x, 0, -ctx.rb.velocity.z) * ctx.cmd.deceleration * deltaTime;
            }

            if (ctx.useGravity)
            {
                ctx.rb.velocity.y += deltaTime * -ctx.baseGrav;
                if (ctx.rb.velocity.y < -ctx.currentJumpData.maxFallSpeed)
                {
                    ctx.rb.velocity.y = -ctx.currentJumpData.maxFallSpeed;
                }
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
                if (ctx.grabTimer > 0 || Leaf() == vaulting)
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

                if (ctx.landingSpeed <= -ctx.currentJumpData.fastFallSpeed)
                {
                    Debug.Log("Harsh landing");
                    return ((PlayerRoot)Parent).grounded.harshLanded;
                }

                return ((PlayerRoot)Parent).grounded;

            }
            return null;
        }
    }

    public class PlayerFixedSpeed : State
    {
        readonly PlayerContext ctx;
        public PlayerFixedSpeed(StateMachine m, State parent, PlayerContext ctx) : base(m)
        {
            this.ctx = ctx;
            Parent = parent;
        }
    }

    // Root class. Does important physics stuff at the end of every update cycle.
    public class PlayerRoot : State
    {
        readonly PlayerContext ctx;
        public readonly PlayerGrounded grounded;
        public readonly PlayerAirborne airborne;
        public readonly PlayerFrozen frozen;
        public readonly PlayerFixedSpeed fixedSpeed;

        public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m)
        {
            grounded = new PlayerGrounded(m, this, ctx);
            airborne = new PlayerAirborne(m, this, ctx);
            frozen = new PlayerFrozen(m, this, ctx);
            this.ctx = ctx;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (Leaf() == frozen) { return; }
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

            Vector2 horizontalVel = new(ctx.rb.velocity.x, ctx.rb.velocity.z);
            if (horizontalVel.magnitude > ctx.cmd.maxSpeed + 0.01f)
            {
                horizontalVel = horizontalVel.normalized * (horizontalVel.magnitude - ctx.cmd.maxSpeedDeceleration * deltaTime);
                ctx.rb.velocity.x = horizontalVel.x; ctx.rb.velocity.z = horizontalVel.y;
            }

            if (ctx.rb.velocity.sqrMagnitude < 0.001f) { ctx.rb.velocity = Vector3.zero; }

            bool doGravityPass = !ctx.currentlyJumping;

            ctx.rb.Move(deltaTime * ctx.rb.velocity, doGravityPass);
        }

        protected override State GetInitialState() => airborne;
        protected override State GetTransition(float deltaTime)
        {

            if (Leaf() == frozen) { return null; }
            ctx.currentVelocity = ctx.rb.velocity; //Reads the current speed we're shmoving at to make new calculations with
            if (ctx.desiredJump && Leaf() != airborne.grabbing && ctx.blockJump == 0)
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
            }
            Jump.CalculateGravity(ctx);

            return null;
        }
    }
}
