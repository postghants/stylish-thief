using System;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RickBingo : MonoBehaviour
{
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
    [Tooltip("What did you get X out of Y of?")] public string unfinishedText;
    string unfinishedCrime;

    [Header("Stats stuff")]
    float currentItemCount;
    [Tooltip("How long until they all come back when you get them all?")] public float respawnTime;
    float timer;
    bool count;

    [SerializeField] private List<RickBingoBall> bingoBalls;
    
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RickBingoBall>() != null)
            {
                bingoBalls.Add(child.GetComponent<RickBingoBall>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (count)
        {
            timer += Time.deltaTime;
            if (timer >= respawnTime)
            {
                count = false;
                timer = 0;
                ResetBingo();
            }
        }
    }

    public void CheckCompletion(GameObject obj)
    {
        currentItemCount++;
        

        if (currentItemCount == bingoBalls.Count)
        {
            Debug.Log(currentItemCount.ToString() + "/" + bingoBalls.Count.ToString());
            GrantScore();
            count = true;
        }
        else
        {
            Debug.Log(currentItemCount.ToString() + "/" + bingoBalls.Count.ToString());
            unfinishedCrime = currentItemCount.ToString() + "/" + bingoBalls.Count.ToString() + unfinishedText;
            CrimeSpreeManager.instance.DoMinorCrime(0, unfinishedCrime, obj);
        }
    }
    public void GrantScore()
    {
        CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime, gameObject);
    }

    public void ResetBingo()
    {
        currentItemCount = currentItemCount - bingoBalls.Count;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RickBingoBall>() != null)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    [Serializable]
    public class BingoBalls
    {
        public RickBingoBall ball;
        //public int weight;
    }
}
