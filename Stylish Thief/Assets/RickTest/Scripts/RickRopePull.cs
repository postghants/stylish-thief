using UnityEngine;

public class RickRopePull : MonoBehaviour
{
    public float endSpeed;
    public float startSpeed;
    public Vector3 offset;
    public bool pulling = false;
    public GameObject endPoint;
    PlayerStateDriver player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.LookAt(endPoint.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (pulling)
        {
            player.SetVelocity(transform.forward * startSpeed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);
                player.transform.position = transform.position - offset;
                player.DisableControls();
                player.SetVelocity(transform.forward * startSpeed);
                pulling = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);
                player.transform.position = transform.position - offset;
                player.DisableControls();
                player.SetVelocity(transform.forward * startSpeed);
                pulling = true;
            }
        }
    }
    public void StopPulling()
    {
        pulling = false;
        player.EnableControls();
        player.ctx.hasGrabbed = false;
        player.SetVelocity(transform.forward * endSpeed);
        player = null;
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