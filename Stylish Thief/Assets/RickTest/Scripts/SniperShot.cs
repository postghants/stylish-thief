using UnityEngine;

public class SniperShot : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private LayerMask layerMask;

    public float damage;

    public bool aiming;
    public float aimTime;
    public float aimTimer;
    public bool pullingTrigger;
    public float triggerTime;
    public float triggerTimer;
    public bool resetting;
    public float resetTime;
    public float resetTimer;


    [Header("references")]
    public PlayerStateDriver player;
    public GameObject shot;
    private SniperLaser laserTracking;
    public Transform raycastTarget;

 
    void Start()
    {
        laserTracking = GetComponentInChildren<SniperLaser>();
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerStateDriver>(); //Yes I know this is super bad for performance. It's only a test solution
        }
        aiming = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming)
        {
            aimTimer += Time.deltaTime;
            //Done aiming
            if (aimTimer >= aimTime)
            {
                aimTimer = 0;
                laserTracking.enabled = false;
                Vector3 laserSize = new Vector3(.25f, .25f, 0);
                laserSize.z = laserTracking.transform.localScale.z;
                laserTracking.transform.localScale = laserSize;
                aiming = false;
                pullingTrigger = true;
            }
        }
        if (pullingTrigger)
        {
            triggerTimer += Time.deltaTime;
            //Done pulling trigger
            if (triggerTimer >= triggerTime)
            {
                triggerTimer = 0;
                shot.SetActive(true);
                shot.GetComponent<CapsuleCollider>().enabled = true;
                Debug.DrawRay(transform.position, raycastTarget.position, Color.yellow);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, raycastTarget.position, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
                {
                    player.TakeDamage(50);
                    Debug.Log("Hit!");
                }
                pullingTrigger = false;
                resetting = true;
            }
        }
        if (resetting)
        {
            resetTimer += Time.deltaTime;
            //Done on every wait period. Reset everything instantly now!
            if (resetTimer >= resetTime)
            {
                resetTimer = 0;
                shot.SetActive(false);
                Vector3 laserSize = new Vector3(.05f, .05f, 0);
                laserSize.z = laserTracking.transform.localScale.z;
                laserTracking.transform.localScale = laserSize;
                laserTracking.enabled = true;
                resetting = false;
                aiming = true;
                
            }
        }
    }
}
//The laser stays aimed at the head or chest
//After some time, stop the SniperLaser script and increase laser thickness
//After like .2 seconds max, activate the shot, with rotation set to sniperLaser gameObject
//When activating shot, also do raycast to see if hit. The shot is just visuals
//After some more time, reset everything
