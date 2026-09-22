using UnityEngine;

public class Card_Bot : MonoBehaviour
{
    [SerializeField] public float countdown;
    public float starttime;

    public GameObject dealerPrefab;
    public GameObject cardPrefab;
    public Transform SpawnPosition;

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            SpawnCard();
            countdown = starttime;
        }
    }

    private void SpawnCard()
    {
        Instantiate(cardPrefab, SpawnPosition.position, SpawnPosition.rotation);
    }


}
