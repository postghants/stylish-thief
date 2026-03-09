using UnityEngine;
using TMPro;

public class RickTextUpdater : MonoBehaviour
{
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        CrimeSpreeManager manager = FindFirstObjectByType<CrimeSpreeManager>();
        if (manager.ChaseTimer <= 0)
        {
            text.text = "Aw... The guards gave up.";
        }
        else
        {
            text.text = "You've been caught!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
