using UnityEngine;

public class StickerCrime : MonoBehaviour
{
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Tooltip("GameObject that represents the sticker")] [SerializeField] private GameObject sticker;
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
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                sticker.SetActive(true);
                CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime);
            }
        }
    }
}
