using System.Collections;
using UnityEngine;

// Contains movement math stuff
public class Jump
{
    public static void SetPhysics(PlayerContext ctx)
    {
        ctx.baseGrav = -ctx.rb.gravity.y * ctx.gravMultiplier;
    }

    public static void PerformJump(PlayerContext ctx)
    {
        if ((ctx.rb.isGrounded && ctx.rb.velocity.y > -0.1) || (ctx.coyoteTimeCounter > 0.03f && ctx.coyoteTimeCounter < ctx.coyoteTime && !ctx.currentlyJumping)) //If grounded or if you still have coyote time
        {
            Debug.Log("Jumpin");
            ctx.particleManager.StartGroup("Jump");
            ctx.currentlyJumping = true;
            ctx.desiredJump = false;
            ctx.jumpBufferCounter = 0;
            ctx.landingSpeed = 0;
            ctx.currentVelocity.y = 0; //Very brute force fix for super jump I guess...
            if (ctx.currentJumpData.setSpeed)
            {
                ctx.currentVelocity = ctx.facing * ctx.currentJumpData.setSpeedSpeed;
            }
            ctx.currentVelocity += ctx.currentVelocity.normalized * ctx.currentJumpData.horizontalBoost;
            ctx.currentVelocity.y += ctx.currentJumpData.jumpImpulse;
            ctx.rb.StartCoroutine(SetMovementMult(ctx));
        }
        if (ctx.jumpBuffer == 0)
        {
            ctx.desiredJump = false;
        }

        ctx.rb.velocity = ctx.currentVelocity; //Applies new Y speed as well as the X that was read earlier
    }

    public static void JumpBuffer(PlayerContext ctx)
    {
        if (ctx.desiredJump)
        {
            ctx.jumpBufferCounter += Time.deltaTime;
            if (ctx.jumpBufferCounter > ctx.jumpBuffer)
            {
                ctx.desiredJump = false;
                ctx.jumpBufferCounter = 0;
            }
        }
    }
    public static void CalculateGravity(PlayerContext ctx)
    {
        // Change the character's gravity while jumping up
        if (!ctx.rb.isGrounded && ctx.rb.velocity.y > 0)
        {
            if (ctx.currentlyJumping)
            {
                ctx.jumpTimer += Time.deltaTime;

                if (ctx.jumpTimer < ctx.currentJumpData.maxMaxSpeedTime)
                {
                    ctx.gravMultiplier = 0;
                }
                else
                {
                    if (ctx.rb.velocity.y >= ctx.currentJumpData.upwardDecelApexThreshold)
                    {
                        ctx.gravMultiplier = ctx.currentJumpData.upwardDeceleration;
                    }
                    else
                    {
                        ctx.gravMultiplier = ctx.currentJumpData.upwardDecelApex;

                        if (ctx.rb.velocity.y + ctx.gravMultiplier * Time.fixedDeltaTime * ctx.rb.gravity.y <= 0)
                        {
                            ctx.rb.velocity.y = 0;
                        }
                    }
                }
            }
            else
            {
                ctx.gravMultiplier = ctx.currentJumpData.upwardDeceleration;
            }
        }

        if (ctx.rb.velocity.y < 0)
        {
            if(ctx.rb.velocity.y >= Time.deltaTime * ctx.rb.gravity.y * 30 && ctx.jumpApexTimer > 0 && ctx.jumpApexTimer < ctx.currentJumpData.hangtimeDuration && ctx.pressingJump)
            {
                ctx.rb.velocity.y = 0;
            }
            ctx.gravMultiplier = ctx.currentJumpData.downwardAccel;
        }

        //Check for hangtime
        if (ctx.currentlyJumping && !ctx.rb.isGrounded && ctx.rb.velocity.y == 0 && ctx.jumpApexTimer < ctx.currentJumpData.hangtimeDuration)
        {
            if (ctx.pressingJump)
            {
                ctx.gravMultiplier = 0;
                ctx.jumpApexTimer += Time.deltaTime;

                if (ctx.jumpApexTimer >= ctx.currentJumpData.hangtimeDuration)
                {
                    ctx.gravMultiplier = ctx.currentJumpData.downwardAccel;
                }
            }
            else
            {
                ctx.gravMultiplier = ctx.currentJumpData.downwardAccel;
            }
        }

        

        // Check for jump cutoff
        if (!ctx.pressingJump && ctx.currentlyJumping && !ctx.rb.isGrounded && ctx.rb.velocity.y > 0 && ctx.currentJumpData.cuttable)
        {
            if (ctx.jumpTimer > ctx.currentJumpData.minMaxSpeedTime && ctx.jumpTimer < ctx.currentJumpData.maxMaxSpeedTime)
            {
                if (ctx.currentJumpData.cutJump)
                {
                    ctx.rb.velocity.y -= ctx.currentJumpData.cutSpeed;
                }
                ctx.jumpTimer = ctx.currentJumpData.maxMaxSpeedTime;
            }
        }

        if (ctx.rb.isGrounded)
        {
            ctx.jumpTimer = 0;
            ctx.jumpApexTimer = 0;
            if (ctx.rb.velocity.y < 0)
            {
                if (ctx.rb.velocity.y < -1)
                {
                    ctx.landingSpeed = ctx.rb.velocity.y;
                }
                ctx.gravMultiplier = 1;
                ctx.rb.velocity.y = 0f;
                ctx.currentlyJumping = false;
            }

        }
        else
        {
            //if (!ctx.currentlyJumping)
            //{
            //    ctx.currentlyJumping = true;
            //}
        }
    }

    private static IEnumerator SetMovementMult(PlayerContext ctx)
    {
        float timer = 0;
        ctx.currentJumpMoveMult = ctx.currentJumpData.jumpMovementMult;
        while (timer < ctx.currentJumpData.jumpMovementMultTime)
        {
            timer += Time.deltaTime;
            if (ctx.rb.isGrounded) { break; }
            yield return null;
        }
        ctx.currentJumpMoveMult = 1;
    }
}
