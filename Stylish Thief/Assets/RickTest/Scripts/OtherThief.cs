using UnityEngine;

public class OtherThief : MonoBehaviour
{
    PlayerStateDriver player;
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;

    [Tooltip("How long does Robin stand still to greet the person?")] public float greetTime;
    private bool greeting;
    private float currentTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (greeting)
        {
            if (currentTime < greetTime)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                player.EnableControls();
                currentTime = 0;
                greeting = false;
                Debug.Log("OH SHIT GOTTA GO");
                GetComponentInParent<OtherThiefManager>().count = true;
                gameObject.SetActive(false);
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
                greeting = true;
                currentTime = 0;
                player.SetVelocity(Vector2.zero);
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.grounded.idle);
                player.DisableControls();
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
                Debug.Log("HI [NAME OF OTHER THIEF]!");
            }
        }
    }
}
