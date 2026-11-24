using System;
using System.Collections.Generic;
using UnityEngine;

public class RickBingo : MonoBehaviour
{
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public int unfinishedScore;
    [Tooltip("What did you get X out of Y of?")] public string unfinishedText;
    string unfinishedCrime;

    [Header("Stats stuff")]
    float currentItemCount;

    [SerializeField] private List<RickBingoBall> bingoBalls;
    [SerializeField] private Crime crime;

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

    public void CheckCompletion(GameObject obj)
    {
        currentItemCount++;
        

        if (currentItemCount >= bingoBalls.Count)
        {
            crime.DoCrimeOfTier(crime.crimeTier, crime.crimeName, crime.crimeValue);
        }
        else
        {
            unfinishedCrime = currentItemCount.ToString() + "/" + bingoBalls.Count.ToString() + unfinishedText;
            crime.DoCrimeOfTier(crime.crimeTier, unfinishedCrime, unfinishedScore);
        }
    }

    public void ResetCrime()
    {
        if (currentItemCount >= bingoBalls.Count)
        {
            currentItemCount = 0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<RickBingoBall>() != null)
                {
                    child.gameObject.SetActive(true);
                }
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
