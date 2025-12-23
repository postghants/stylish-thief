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
            ctx.landParticles.Play();
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
        //We change the character's gravity based on her Y direction
        if (ctx.pressingJump && ctx.currentlyJumping && !ctx.rb.isGrounded)
        {
            ctx.jumpTimer += Time.deltaTime;

            float totalTime = 0;
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

        if (!ctx.pressingJump && !ctx.rb.isGrounded)
        {
            if (ctx.rb.velocity.y <= 0)
            {
                ctx.gravMultiplier = ctx.currentJumpData.standardGravMult;
            }
            else
            {
                ctx.gravMultiplier = ctx.currentJumpData.jumpCutOff;
            }
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
