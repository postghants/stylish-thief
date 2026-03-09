using TMPro;
using UnityEngine;

public class RickNumberUpdater : MonoBehaviour
{
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        CrimeSpreeManager manager = FindFirstObjectByType<CrimeSpreeManager>();
        text.text = manager.Score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
