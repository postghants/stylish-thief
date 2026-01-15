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

    [Header("Countdown stuff")]
    public bool doCountdown;
    public float startTimer;
    public GameObject patrolZones;
    [Header("Countdown internal")]
    public bool frozen;
    public float startTime = 0;
    PlayerStateDriver playerInstance;

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
        playerInstance = player;
        SpawnNewValuables();
        patrolZones.SetActive(false);
    }

    private void Update()
    {
        Countdown();
        if (!frozen)
        {
            if (ChaseTimer == 0) { return; }

            if (ChaseTimer >= 60)
            {
                chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("mm':'ss");
            }
            else
            {
                chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("mm':'ss");
            }
            ChaseTimer -= Time.deltaTime;

            if (ChaseTimer < 0) { EndSpree(); }
        }
    }
    public void Countdown()
    {
        if (doCountdown)
        {
            if (startTime < startTimer)
            {
                StartSpree();
                startTime += Time.deltaTime;
                //Debug.Log(startTime);
                chaseUI.countDownText.text = Mathf.Round(5 - startTime).ToString();
                playerInstance.Machine.ChangeState(playerInstance.Root.Leaf(), playerInstance.Root.frozen);
                frozen = true;
            }
            else
            {
                if (frozen)
                {
                    playerInstance.Machine.ChangeState(playerInstance.Root.Leaf(), playerInstance.Root.grounded);
                    Debug.Log("GO");
                    ChaseTimer = maxChaseTime;
                    chaseUI.countDownReact.DoReaction(false, 1, 2);
                    chaseUI.joystickImage.DoReaction(false, 1, 2);
                    chaseUI.joystickPrompt.DoReaction(false, 1, 2);
                    patrolZones.SetActive(true);
                }
                frozen = false;
            }
        }
        else
        {
            chaseUI.countDownText.enabled = false;
            chaseUI.joystickImage.gameObject.SetActive(false);
            chaseUI.joystickPrompt.gameObject.SetActive(false);
        }
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
        chaseUI.timerText.text = TimeSpan.FromSeconds(ChaseTimer).ToString("ss");
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
        _score = Mathf.Round(_score);
        Score += _score;
        chaseUI.scoreText.text = Score.ToString("C");
        chaseUI.crimeText.text = crimeName;
        chaseUI.rewardText.text = _score.ToString("C");
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
        List<Transform> toRemove = new List<Transform>();
        foreach(var l in valuableLocations)
        {
            if(l == null) toRemove.Add(l);
        }

        foreach(var l in toRemove)
        {
            valuableLocations.Remove(l);
        }

        if(valuableLocations.Count == 0) { return; }

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
