using UnityEngine;

public class Parryable : MonoBehaviour
{
    PlayerStateDriver player;
    [Tooltip("How high the player teleports to get over this NPC. Typically set to...")] public float parryHeight;
    [Tooltip("How long it takes before you can parry the same enemy again")] public float parryDelay;
    float timer;
    bool parried;
    Collider coll;
    Renderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll = GetComponent<Collider>();
        coll.enabled = true;
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (parried)
        {
            if (timer < parryDelay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                coll.enabled = true;
                renderer.enabled = true;
                parried = false;
                timer = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                Vector3 position = transform.position;
                position.y += parryHeight;
                player.transform.position = position;
                player.ctx.rb.isGrounded = false;
                coll.enabled = false;
                renderer.enabled = false;
                parried = true;

                player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne.parry);
            }
        }
    }
}
