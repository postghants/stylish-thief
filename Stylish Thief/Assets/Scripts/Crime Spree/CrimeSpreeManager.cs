using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrimeSpreeManager : Singleton<CrimeSpreeManager>
{
    [Header("Chase")]
    [SerializeField] private float maxChaseTime;
    [SerializeField] private int valuablesToSpawn;
    [SerializeField] private float uiLingerTime;

    [Header("References")]
    [SerializeField] private List<Transform> valuableLocations;
    [SerializeField] private GameObject valuablePrefab;
    [HideInInspector] public ChaseUI chaseUI;


    [Header("Current stats")]
    public int AggressionLevel = 0;
    public float Score;
    public float ChaseTimer = 0;

    private void Start()
    {
        PlayerStateDriver player = FindAnyObjectByType<PlayerStateDriver>();
    }

    private void Update()
    {
        if (ChaseTimer == 0) { return; }

        if (ChaseTimer >= 60)
        {
            chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("mm':'ss':'f");
        }
        else
        {
            chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("ss':'fff");
        }
        ChaseTimer -= Time.deltaTime;


        if (ChaseTimer < 0) { EndSpree(); }
    }

    public void StartSpree()
    {
        chaseUI.gameObject.SetActive(true);
    }

    public void EndSpree()
    {
        ChaseTimer = 0;
        chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("ss':'fff");
        StartCoroutine(UILinger());
    }

    private IEnumerator UILinger()
    {
        yield return new WaitForSeconds(uiLingerTime);
        chaseUI.gameObject.SetActive(false);
        Score = 0;

    }

    public void CollectedValuable(Valuable collected)
    {
        if (ChaseTimer == 0)
        {
            StartSpree();
        }

        ChaseTimer = maxChaseTime;
        AddScore(collected.Value);

        if(collected.transform.parent == null) { return; }

        int spawnCount = valuablesToSpawn;
        List<Transform> taken = new() { collected.transform.parent };

        while (spawnCount > 0)
        {
            Transform loc = valuableLocations[Random.Range(0, valuableLocations.Count)];
            if (taken.Contains(loc)) { continue; }

            taken.Add(loc);
            Instantiate(valuablePrefab, loc);
            spawnCount--;
        }
    }


    public void AddScore(float _score)
    {
        Score += _score;
        chaseUI.scoreText.text = Score.ToString("C");
    }

    public void RemoveScore(float _score)
    {
        Score -= _score;
    }

}
