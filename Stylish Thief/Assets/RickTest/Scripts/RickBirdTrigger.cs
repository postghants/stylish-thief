using UnityEngine;

public class RickBirdTrigger : MonoBehaviour
{
    //public PlayerStateDriver player;
    [Header("Crime stuff")]
    public float givenScore;
    public string crime;
    [Header("Stats stuff")]
    public GameObject player;
    public RickBird birds;
    public float respawnTime;
    float timer;
    bool count;
    public GameObject birdPrefab;
    GameObject birdInstance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count)
        {
            if (timer < respawnTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                count = false;
                birdInstance = Instantiate(birdPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                birdInstance.transform.parent = transform;
                birds = birdInstance.GetComponent<RickBird>();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (birds != null)
        {
            if (birds.verticalSpeed < .1f)
            {
                if (other.gameObject.layer == 6)
                {
                    player = other.gameObject;
                    if (player != null)
                    {
                        birds.FlyAway(player.transform);
                        count = true;
                        CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
                    }
                }
            }
        }
    }
    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject;
            if (player != null)
            {
                birds.FlyAway(player.transform);
            }
        }
    }*/
}
