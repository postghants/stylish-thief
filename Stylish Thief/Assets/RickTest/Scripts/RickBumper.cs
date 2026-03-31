using UnityEngine;

public class RickBumper : MonoBehaviour
{
    PlayerStateDriver player;
    public float startSpeed;
    public GameObject target;
    public GameObject stopper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target != stopper)
        {
            //stopper.transform.parent = null;
            Destroy(stopper);
        }
        transform.LookAt(target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BumperLaunch()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
            player.gameObject.GetComponent<ActorPhysics>().gravity = Vector3.zero;
            player.DisableControls();
            player.ctx.cmd.deceleration = 0;
            player.ctx.cmd.maxSpeedDeceleration = 0;
            player.transform.position = transform.position;
            player.SetVelocity(transform.forward * startSpeed);
            player.ctx.hasGrabbed = false;
        }
    }
}


//Todo
//Make this thing standard check for how long the player needs to travel in a straight line, before being let go and being able to do shit again. Probably by replacing Next In Sequence with a general target
    //To do this, I need to set a timer, start it OnTrigger, add a collider as child, and... That's it?