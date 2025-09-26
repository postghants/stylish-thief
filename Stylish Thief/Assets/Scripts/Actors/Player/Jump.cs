using UnityEngine;

// Contains movement math stuff
public class Jump
{

    public static void SetPhysics(PlayerContext ctx)
    {
        //Determine the character's gravity scale, using the stats provided. Multiply it by a gravMultiplier, used later
        Vector2 newGravity = new(0, (-2 * ctx.currentJumpData.jumpHeight) / (ctx.currentJumpData.timeToJumpApex * ctx.currentJumpData.timeToJumpApex));
        ctx.baseGrav = (newGravity.y / ctx.rb.gravity.y) * ctx.gravMultiplier;
    }

    public static void PerformJump(PlayerContext ctx)
    {
        if ((ctx.rb.isGrounded && ctx.rb.velocity.y > -0.1) || (ctx.coyoteTimeCounter > 0.03f && ctx.coyoteTimeCounter < ctx.coyoteTime)) //If grounded or if you still have coyote time
        {
            ctx.desiredJump = false;
            ctx.jumpBufferCounter = 0;
            ctx.currentVelocity.y = 0; //Very brute force fix for super jump I guess...
            CalculateJump(ctx);
            ctx.currentVelocity.y += ctx.jumpSpeed; //Swaps Y speed for the newly calculated one in CalculateJump()
        }
        if (ctx.jumpBuffer == 0)
        {
            ctx.desiredJump = false;
        }
    }

    public static void CalculateJump(PlayerContext ctx)
    {
        ctx.jumpSpeed = Mathf.Sqrt(-2f * ctx.rb.gravity.y * ctx.currentJumpData.jumpHeight);
        // was causing issues with coyote jump
        //if (velocity.y > 0f)
        //{
        //    jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        //}
        //else if (velocity.y < 0f)
        //{
        //    jumpSpeed += Mathf.Abs(velocity.y);
        //}
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

        //If Kit is going up...
        if (ctx.rb.velocity.y > 0.01f)
        {
            if (ctx.rb.isGrounded)
            {
                //Don't change it if Kit is stood on something (such as a moving platform)
                ctx.gravMultiplier = 1;
            }
            else
            {
                //Apply upward multiplier if player is rising and holding jump
                if (ctx.pressingJump && ctx.currentlyJumping)
                {
                    ctx.gravMultiplier = ctx.currentJumpData.upwardMovementMultiplier;
                }
                //But apply a special downward multiplier if the player lets go of jump
                else
                {
                    ctx.gravMultiplier = ctx.currentJumpData.jumpCutOff;
                }
            }
        }

        //Else if going down...
        else if (ctx.rb.velocity.y < -0.01f)
        {

            if (ctx.rb.isGrounded)
            //Don't change it if Kit is stood on something (such as a moving platform)
            {
                ctx.gravMultiplier = 1;
                ctx.rb.velocity.y = 0f;
            }
            else
            {
                //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                ctx.gravMultiplier = ctx.currentJumpData.downwardMovementMultiplier;
            }

        }
        //Else not moving vertically at all
        else
        {
            if (ctx.rb.isGrounded)
            {
                ctx.currentlyJumping = false;
                ctx.rb.velocity.y = 0f;
            }

            ctx.gravMultiplier = 1;
        }

        //Set the character's Rigidbody's velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
        //rb.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -speedLimit, 100));
    }
}
