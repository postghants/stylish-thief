using UnityEngine;

public class RickUplauncher : MonoBehaviour
{
    public float launchForce;
    PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();

            //Changing the player's state (probably don't need this but handy to have anyway)
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);

            //Changing the player's speed
            player.SetVelocity(new Vector3(player.ctx.rb.velocity.x, launchForce, player.ctx.rb.velocity.z));

            //Reset the Grab
            player.ctx.hasGrabbed = false;
        }
    }
}
