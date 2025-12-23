using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RickVendingMachine : MonoBehaviour
{
    PlayerStateDriver player;
    public float tamperScore;
    public string tamperCrime;
    public float breakScore;
    public string breakCrime;
    public float totalItems;
    float remainingItems;
    public GameObject canPrefab;
    GameObject can;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        remainingItems = totalItems;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            
            //Detect player
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            Debug.Log("PISS " + player.Root.Leaf());

            //Check if current state is something specific
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing" || player.Root.Leaf().ToString().Contains("tunned") || player.Root.Leaf().ToString().Contains("lid"))
            {
                Debug.Log("aaaaaaaaaa");
                if (remainingItems > 1)
                {
                    can = Instantiate(canPrefab, transform.parent.transform.position, Quaternion.identity);
                    remainingItems--;
                    CrimeSpreeManager.instance.DoMinorCrime(tamperScore, tamperCrime);
                }
                else if (remainingItems == 1)
                {
                    can = Instantiate(canPrefab, transform.parent.transform.position, Quaternion.identity);
                    can = Instantiate(canPrefab, transform.parent.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
                    can = Instantiate(canPrefab, transform.parent.transform.position - new Vector3(0, 0, 1), Quaternion.identity);
                    remainingItems--;
                    CrimeSpreeManager.instance.DoMinorCrime(tamperScore, tamperCrime);
                }
                else
                {
                    can = Instantiate(canPrefab, transform.parent.transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
                    can = Instantiate(canPrefab, transform.parent.transform.position + new Vector3(-1, 0, 1), Quaternion.identity);
                    can = Instantiate(canPrefab, transform.parent.transform.position - new Vector3(1, 0, 1), Quaternion.identity);
                    CrimeSpreeManager.instance.DoMinorCrime(breakScore, breakCrime);
                    Destroy(gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}
