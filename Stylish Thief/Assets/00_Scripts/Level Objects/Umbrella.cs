using UnityEngine;

public class Umbrella : MonoBehaviour
{
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
            Debug.Log("Touched!");
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                player.ctx.hasGrabbed = false;
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.umbrellaLaunch);
                Debug.Log("Grabbed!");
            }
        }
    }
}
