using UnityEngine;
using System.Collections;

public class Input_Flip_Plat : MonoBehaviour
{
    public GameObject FlipPlat;
    public float FlipSpeed = 1000f;
    public Vector3 rotate;
    public float upLaunchSpeed;
    public float fwdLaunchSpeed;

    private Collider flippy;
    public PlayerStateDriver player;
    public bool PonP;
    public bool islaunched;
    public bool Flipstart;
    private Quaternion start;

    void Start()
    {
        flippy = GetComponent<Collider>();
        Flipstart = false;
        start = transform.localRotation;
    }

    public void Update()
    {
        if (Flipstart == false)
        {
            if (player.ctx.currentlyJumping)
            {
                Flipstart = true;
                StartCoroutine(Flipping());
            }
        }

        if (!player.ctx.currentlyJumping)
        {
            Flipstart= false;
        }

    }
    public IEnumerator Flipping()
    {
        while (FlipPlat.transform.rotation.eulerAngles.x > -180 && FlipPlat.transform.rotation.eulerAngles.x < 180)
        {
            FlipPlat.transform.Rotate(new Vector3(FlipSpeed * Time.deltaTime, 0f, 0f));
            flippy.isTrigger = true;
            if (PonP)
            {
                islaunched = true;
                Vector3 launchvelocity = transform.forward;
                float xSwapped = launchvelocity.z;
                float zSwapped = launchvelocity.x;
                launchvelocity.x = xSwapped * fwdLaunchSpeed;
                launchvelocity.z = zSwapped * fwdLaunchSpeed;
                launchvelocity.y = upLaunchSpeed;

                if (player != null)
                {
                    player.Machine.ChangeState(player.Root.Leaf(), player.Root.airborne);
                    //player.ctx.rb.velocity.x += launchvelocity.x;
                    //player.ctx.rb.velocity.z += launchvelocity.z;
                    player.ctx.rb.velocity.y = launchvelocity.y;
                    PonP = false;
                }
            }
            yield return null;
        }

        FlipPlat.transform.rotation = start;
        flippy.isTrigger = false;
        yield return new WaitForSeconds(1f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.GetComponentInParent<PlayerStateDriver>();
            PonP = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            PonP = false;
            islaunched = false;
        }
    }

}
