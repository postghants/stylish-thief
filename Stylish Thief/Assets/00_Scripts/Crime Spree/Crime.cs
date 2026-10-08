using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Crime : MonoBehaviour
{
    private NeoCrimeSpreeManager manager;
    private float resetTimer = 0;
    public UnityEvent resetCrime;
    private bool count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = NeoCrimeSpreeManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (resetTimer > 0 && count)
        {
            resetTimer -= Time.deltaTime;
        }
        else if (count)
        {
            resetCrime?.Invoke();
            Debug.Log("Invoking");
            count = false;
        }
    }
    public void DoMiniCrime(string crimeName, int crimeValue)
    {
        manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
        manager.AddPoints(crimeValue);
        manager.currentSpreeTime += manager.crimeData.miniGainedTime / 100 * manager.maxSpreeTime;
        resetTimer = manager.crimeData.miniResetTime; count = true;
        manager.ResetComboTimer(manager.crimeData.miniComboAddition);
        //Mini UI response
    }
    public void DoMinorCrime(string crimeName, int crimeValue)
    {
        manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
        manager.AddPoints(crimeValue);
        manager.currentSpreeTime += manager.crimeData.minorGainedTime / 100 * manager.maxSpreeTime;
        resetTimer = manager.crimeData.minorResetTime; count = true;
        manager.ResetComboTimer(manager.crimeData.minorComboAddition);
        //Minor UI response
    }
    public void DoMiddleCrime(string crimeName, int crimeValue)
    {
        manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
        manager.AddPoints(crimeValue);
        manager.currentSpreeTime += manager.crimeData.middleGainedTime / 100 * manager.maxSpreeTime;
        resetTimer = manager.crimeData.middleResetTime; count = true;
        manager.ResetComboTimer(manager.crimeData.middleComboAddition);
        if (!manager.activeSpree) { manager.activeSpree = true; }
        //Middle UI response
    }
    public void DoMajorCrime(string crimeName, int crimeValue)
    {
        manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
        manager.AddPoints(crimeValue);
        manager.currentSpreeTime += manager.crimeData.majorGainedTime / 100 * manager.maxSpreeTime;
        resetTimer = manager.crimeData.majorResetTime; count = true;
        manager.ResetComboTimer(manager.crimeData.majorComboAddition);
        if (!manager.activeSpree) { manager.activeSpree = true; }
        //Major UI response
    }
    public void DoMegaCrime(string crimeName, int crimeValue)
    {
        manager.RegisterCrimeName(crimeName); //Also to be logged later in a Spree record
        manager.AddPoints(crimeValue);
        manager.currentSpreeTime = manager.maxSpreeTime;
        manager.ResetComboTimer(manager.crimeData.megaComboAddition);
        if (!manager.activeSpree) { manager.activeSpree = true; }
        //Comically bombastic UI response!
    }
}
