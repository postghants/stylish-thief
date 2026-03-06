using UnityEngine;

public class RickBumper : MonoBehaviour
{
    PlayerStateDriver player;
    public float startSpeed;
    public bool partOfSequence;
    public GameObject nextInSequence;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (partOfSequence)
        {
            transform.LookAt(nextInSequence.transform);
        }
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
            if (partOfSequence)
            {
                player.gameObject.GetComponent<ActorPhysics>().gravity = Vector3.zero;
                player.DisableControls();
            }
            player.transform.position = transform.position;
            player.SetVelocity(transform.forward * startSpeed);
            player.ctx.hasGrabbed = false;
        }
    }
}
