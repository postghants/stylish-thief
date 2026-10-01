using UnityEngine;
using System.Collections;

public class Step_Flip_Plat : MonoBehaviour
{
    public GameObject FlipPlat;
    public float FlipSpeed = 300f;
    public Vector3 rotate;
    public float upLaunchSpeed;
    public float fwdLaunchSpeed;

    private Collider flippy;
    public PlayerStateDriver player;
    public bool PonP;
    public bool islaunched;
    private Quaternion start;

    void Start()
    {
        flippy = GetComponent<Collider>();
        start = transform.localRotation;
    }


    public IEnumerator Flipping()
    {
        while (FlipPlat.transform.rotation.eulerAngles.x > -180 && FlipPlat.transform.rotation.eulerAngles.x < 180)
        {
            FlipPlat.transform.Rotate(new Vector3(FlipSpeed * Time.deltaTime, 0f, 0f));
            flippy.isTrigger = true;
            
            yield return null;
        }

        FlipPlat.transform.rotation = start;
        flippy.isTrigger = false;
        yield return new WaitForSeconds(3f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.GetComponentInParent<PlayerStateDriver>();
            PonP = true;
            StartCoroutine(Flipping());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            PonP = false;
        }
    }
}
