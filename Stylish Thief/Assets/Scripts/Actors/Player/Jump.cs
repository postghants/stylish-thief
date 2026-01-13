using System.Collections;
using System.Linq;
using Unity.VisualScripting;
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

            ctx.particleManager.StartGroup("Jump");
            ctx.currentlyJumping = true;
            ctx.desiredJump = false;
            ctx.jumpBufferCounter = 0;
            ctx.currentVelocity.y = 0; //Very brute force fix for super jump I guess...
            ctx.currentVelocity.y += ctx.currentJumpData.jumpImpulse;
            ctx.rb.StartCoroutine(SetMovementMult(ctx));
        }
        if (ctx.jumpBuffer == 0)
        {
            ctx.desiredJump = false;
        }
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
        // Change the character's gravity depending on the jump time
        if (ctx.currentlyJumping && !ctx.rb.isGrounded)
        {
            ctx.jumpTimer += Time.deltaTime;

            float totalTime = 0;

            // Cut off the first state if jump is let go
            if (ctx.jumpTimer < ctx.currentJumpData.jumpStates[0].duration && !ctx.pressingJump)
            {
                ctx.jumpTimer = ctx.currentJumpData.jumpStates[0].duration;
            }

            foreach (JumpState state in ctx.currentJumpData.jumpStates)
            {
                totalTime += state.duration;
                if (totalTime > ctx.jumpTimer || state == ctx.currentJumpData.jumpStates.Last())
                {
                    ctx.gravMultiplier = state.gravMult;
                    break;
                }
            }
        }

        // Check for jump cutoff
        if (!ctx.pressingJump && ctx.currentlyJumping && !ctx.rb.isGrounded && ctx.rb.velocity.y > 0)
        {
                ctx.gravMultiplier = ctx.currentJumpData.jumpCutoff;
        }

        if (ctx.rb.isGrounded)
        {
            ctx.jumpTimer = 0;
            if (ctx.rb.velocity.y < 0)
            {
                ctx.gravMultiplier = 1;
                ctx.rb.velocity.y = 0f;
                ctx.currentlyJumping = false;
            }

        }
        else
        {
            if (!ctx.currentlyJumping)
            {
                    ctx.jumpTimer = ctx.currentJumpData.timerValueOnFall;
                    ctx.currentlyJumping = true;
            }
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
