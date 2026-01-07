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
    [SerializeField] private float multPerCombo;


    [Header("References")]
    [SerializeField] private List<Transform> valuableLocations;
    [SerializeField] private GameObject valuablePrefab;
    [HideInInspector] public ChaseUI chaseUI;


    [Header("Current stats")]
    public int AggressionLevel = 0;
    public float Score;
    public float ChaseTimer = 0;
    public int ComboCount;
    public float Multiplier = 1;
    public List<Valuable> Valuables;

    private void Start()
    {
        PlayerStateDriver player = FindAnyObjectByType<PlayerStateDriver>();

        SpawnNewValuables();
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
        ComboCount = 0;
        Multiplier = 1;
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
        AddScore(collected.Value * Multiplier, "Grand Theft");
        chaseUI.crimeReact.DoReaction(true, 2, .25f);
        chaseUI.rewardReact.DoReaction(true, 2, 1);
        AddComboCount();

        if (collected.transform.parent == null) { return; }

        List<Transform> taken = new() { collected.transform.parent };

        SpawnNewValuables(taken, valuablesToSpawn);
    }

    public void DoCrime(float score)
    {
        if (ChaseTimer == 0) { return; }
        ChaseTimer = maxChaseTime;
        AddScore(score * Multiplier, "Another Crime");
        AddComboCount();
    }
    public void DoMinorCrime(float score, string crimeName)
    {
        if (ChaseTimer == 0) { return; }
        AddScore(score * Multiplier, crimeName);
        AddComboCount();
        chaseUI.crimeReact.DoReaction(true, 2, .25f);
        chaseUI.multReact.DoReaction(true, 0, 0);
        chaseUI.rewardReact.DoReaction(true, 2, 1);
    }

    public void AddScore(float _score, string crimeName)
    {
        Score += _score;
        chaseUI.scoreText.text = Score.ToString("C");
        chaseUI.crimeText.text = crimeName;
        chaseUI.rewardText.text = _score.ToString();
    }

    public void RemoveScore(float _score)
    {
        Score -= _score;
        chaseUI.scoreText.text = Score.ToString("C");
    }

    public void AddComboCount()
    {
        ComboCount++;
        Multiplier += multPerCombo;
        chaseUI.comboText.text = "COMBO " + ComboCount.ToString();
        chaseUI.multText.text = Multiplier.ToString("0.0") + "x";
    }

    public void SpawnNewValuables(List<Transform> taken, int spawnCount)
    {
        foreach (Valuable v in Valuables)
        {
            Destroy(v.gameObject);
        }
        Valuables.Clear();

        if(spawnCount >= valuableLocations.Count) { spawnCount = valuableLocations.Count - 1; }

        if (valuableLocations.Count < spawnCount + 1) { return; }
        while (spawnCount > 0)
        {
            Transform loc = valuableLocations[Random.Range(0, valuableLocations.Count)];
            if (taken.Contains(loc)) { continue; }

            taken.Add(loc);
            Valuables.Add(Instantiate(valuablePrefab, loc).GetComponent<Valuable>());
            spawnCount--;
        }
    }

    public void SpawnNewValuables()
    {
        SpawnNewValuables(new(), valuablesToSpawn);
    }

}
