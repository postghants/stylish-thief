using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RickBarSwing : MonoBehaviour
{
    PlayerStateDriver player;
    public float forwardVelocity;
    public float upVelocity;
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
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            //player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
            //player.SetVelocity(Vector3.forward * forwardVelocity + new Vector3(0, upVelocity, 0));

            player.SetVelocity(Vector3.forward * forwardVelocity);
            player.Machine.ChangeState(player.Root.Leaf(), player.Root.fixedSpeed);
        }
    }
}
