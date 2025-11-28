using UnityEngine;

public class RickPoleTop : MonoBehaviour
{
    public PlayerStateDriver player;
    BoxCollider boxCollider;
    bool hasStoodStill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
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

            //Check if current state is something specific
            if (player.Root.Leaf().ToString() == "HSM.PlayerFalling" && player.ctx.rb.velocity.y < 0 && player.transform.position.y > boxCollider.transform.position.y + boxCollider.bounds.extents.y + .5f)
            {


                //Toggling controls
                //player.DisableControls();
                //player.EnableControls();

                //Teleporting the player
                player.transform.position = new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y + boxCollider.bounds.extents.y + .5f, boxCollider.transform.position.z);

                //Changing the player's speed
                player.SetVelocity(Vector3.zero);

                //Changing the player's state (probably don't need this but handy to have anyway)
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.grounded.idle);

                //Reset the Grab
                player.ctx.hasGrabbed = false;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();

            //Check if current state is something specific
            if (!hasStoodStill && player.Root.Leaf().ToString() == "HSM.PlayerFalling" && player.ctx.rb.velocity.y < 0 && player.transform.position.y > boxCollider.transform.position.y + boxCollider.bounds.extents.y + .5f)
            {
                //Toggling controls
                //player.DisableControls();
                //player.EnableControls();

                //Teleporting the player
                player.transform.position = new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y + boxCollider.bounds.extents.y + .5f, boxCollider.transform.position.z);

                //Changing the player's speed
                player.SetVelocity(Vector3.zero);

                //Changing the player's state (probably don't need this but handy to have anyway)
                //player.Machine.ChangeState(player.Root.Leaf(), player.Root.grounded.idle);

                //Reset the Grab
                player.ctx.hasGrabbed = false;
            }
            if (player.ctx.moveInputValue.magnitude == 0)
            {
                hasStoodStill = true;
            }
            
        }
    }
}
