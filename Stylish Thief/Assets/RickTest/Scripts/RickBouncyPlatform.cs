using UnityEngine;

public class RickBouncyPlatform : MonoBehaviour
{
    [Tooltip("How many points does the player get from this?")] public float score;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Tooltip("How hard does this launch you?")] public float launchForce;
    //public float oneToOneConstant = 1.5f;
    //public float minimumHeight;
    //public bool oneToOne;
    //public bool fullSpeedReplacement;
    Vector3 directionalForce;
    PlayerStateDriver player;

    private void OnTriggerEnter(Collider other)
    {
        directionalForce = transform.up * launchForce;
        if (other.gameObject.layer == 6)
        {
            CrimeSpreeManager.instance?.DoMinorCrime(score, crime);
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            
            /*if (oneToOne)
            {
                
                if (player.ctx.rb.velocity.y > -minimumHeight * (1 + player.ctx.baseJumpData.downwardMovementMultiplier / 10))
                {
                    player.ctx.rb.velocity = transform.up * minimumHeight;
                }
                else
                {
                    player.ctx.rb.velocity = new Vector3(player.ctx.rb.velocity.x, -player.ctx.rb.velocity.y * (1 + player.ctx.baseJumpData.downwardMovementMultiplier / 10), player.ctx.rb.velocity.z);
                    //player.ctx.rb.velocity = transform.up * (-player.ctx.rb.velocity.y * (1 + player.ctx.baseJumpData.downwardMovementMultiplier / 10));
                }
            }*/
            //if (oneToOne)
            //{
            //    //Check for launch magnitude here as the check for minimum launch
            //    if (player.ctx.rb.velocity.y > -minimumHeight)
            //    {
            //        player.SetVelocity(transform.up * minimumHeight);
            //    }
            //    else
            //    {
            //        //player.ctx.rb.velocity = new Vector3(player.ctx.rb.velocity.x, -player.ctx.rb.velocity.y * (1 + player.ctx.baseJumpData.downwardMovementMultiplier / 10), player.ctx.rb.velocity.z);
            //        player.SetVelocity(transform.up * (-player.ctx.rb.velocity.y * oneToOneConstant));
            //    }
            //}
            //else if (fullSpeedReplacement)
            //{
            //    player.SetVelocity(directionalForce);
            //}
            //else
            //{
                player.SetVelocity(player.ctx.rb.velocity - new Vector3(0, player.ctx.rb.velocity.y, 0) + directionalForce);
            //}
            player.transform.position = player.transform.position + new Vector3(0, .01f, 0);
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
            player.ctx.hasGrabbed = false;
        }
    }


    /* RICK'S LITTLE CHEAT SHEET
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();

            //Check if current state is something specific
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                Debug.Log("trhunetoigjoieoigeaoi");
            }

            //Toggling controls
            player.DisableControls();
            player.EnableControls();

            //Teleporting the player
            player.transform.position = player.transform.position + new Vector3(0, 10, 0);

            //Changing the player's speed
            player.SetVelocity(transform.up * minimumHeight);

            //Changing the player's state (probably don't need this but handy to have anyway)
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);

            //Reset the Grab
            player.ctx.hasGrabbed = false;
        }
    }*/
}


//On colision find the player
//Figure out which vector 3 direction is up for the platform
//Multiply that direction with a set launchforce
//Apply the force to the player