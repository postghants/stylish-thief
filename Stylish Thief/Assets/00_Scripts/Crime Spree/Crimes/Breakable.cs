using UnityEngine;

public class Breakable : MonoBehaviour
{
    public bool breakOnHorizontal;
    public bool breakOnVertical;
    public int crimeTier;
    public string crimeName;
    public int crimeValue;
    public bool alwaysBreaks;

    private Collider playerSideCollider;
    public Collider enemySideCollider;
    public GameObject art;
    private Crime crime;
    private void Start()
    {
        playerSideCollider = GetComponent<Collider>();
        crime = GetComponent<Crime>();
        if (alwaysBreaks)
        {
            playerSideCollider.isTrigger = true;
        }
    }

    public void TriggerTrigger(bool horizontal, bool vertical)
    {
        if (!alwaysBreaks)
        {
            if ((horizontal && breakOnHorizontal) || (vertical && breakOnVertical))
            {
                playerSideCollider.isTrigger = true;
            }
            else
            {
                playerSideCollider.isTrigger = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (playerSideCollider.isTrigger)
        {
            if (other.gameObject.layer == 6)
            {
                playerSideCollider.enabled = false;
                enemySideCollider.enabled = false;
                art.SetActive(false);
                crime.DoCrimeOfTier(crimeTier, crimeName, crimeValue);
            }
        }
    }
    public void ResetCrime()
    {
        playerSideCollider.enabled = true;
        enemySideCollider.enabled = true;
        art.SetActive(true);
    }
}
