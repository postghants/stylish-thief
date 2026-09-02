using UnityEngine;

public class RickBingoBall : MonoBehaviour
{
    RickBingo rickBingo;
    void Start()
    {
        rickBingo = GetComponentInParent<RickBingo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //Communicate how many its been based on length of parent's list
            rickBingo.CheckCompletion(gameObject);
            gameObject.SetActive(false);
        }
    }
}
