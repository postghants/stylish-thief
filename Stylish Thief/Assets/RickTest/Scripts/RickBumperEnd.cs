using UnityEngine;

public class RickBumperEnd : MonoBehaviour
{
    public bool justGravity;
    PlayerStateDriver player;
    public GameObject playerPrefab;
    PlayerStateDriver prefabPlayer;
    public float endSpeed;
    [SerializeField] private RickBumper launchingBit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefabPlayer = playerPrefab.GetComponent<PlayerStateDriver>();
        if (launchingBit == null)
        {
            launchingBit = GetComponentInParent<RickBumper>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            //player.EnableControls();

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (justGravity)
            {
                player.gameObject.GetComponent<ActorPhysics>().gravity = -Vector3.up;
            }
            else
            {
                player.EnableControls();
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.falling);
                player.gameObject.GetComponent<ActorPhysics>().gravity = -Vector3.up;
                player.ctx.cmd.deceleration = prefabPlayer.ctx.airMoveData.deceleration;
                player.ctx.cmd.maxSpeedDeceleration = prefabPlayer.ctx.airMoveData.maxSpeedDeceleration;
                //player.SetVelocity(transform.forward * endSpeed);
                player.ctx.hasGrabbed = false;
                player.gameObject.GetComponent<ActorPhysics>().gravity = -Vector3.up;
                launchingBit.fsTimer = 0;
                launchingBit.countFailsafe = false;
            }
        }
    }
}
