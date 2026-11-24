using UnityEngine;

public class StickerCrime : MonoBehaviour
{
    [Tooltip("GameObject that represents the sticker")] [SerializeField] private GameObject sticker;
    PlayerStateDriver player;
    [SerializeField] private Crime crime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            player = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing")
            {
                sticker.SetActive(true);
                crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
            }
        }
    }
    public void ResetCrime()
    {
        sticker.SetActive(false);
    }
}
