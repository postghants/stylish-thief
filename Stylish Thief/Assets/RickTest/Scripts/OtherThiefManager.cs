using System.Collections.Generic;
using UnityEngine;

public class OtherThiefManager : MonoBehaviour
{

    [Header("Stats stuff")]
    public float respawnTime;
    float timer;
    public bool count;

    [SerializeField] private List<OtherThief> thiefLocations;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<OtherThief>() != null)
            {
                thiefLocations.Add(child.GetComponent<OtherThief>());
            }
        }
        count = true;
    }
    public void SpawnThief()
    {
        thiefLocations[Random.Range(0, thiefLocations.Count)].gameObject.SetActive(true);
        Debug.Log("They're back!");
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
                SpawnThief();
            }
        }
    }
}
