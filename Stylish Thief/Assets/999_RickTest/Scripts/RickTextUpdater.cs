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
            text.text = "That's all, folks!";
        }
        else
        {
            text.text = "That's all, folks!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
