using UnityEngine;

public class CheckIfVisible : MonoBehaviour
{
    public TutorialPrompt prompt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnBecameVisible()
    {
        prompt.EndPrompt();
    }
}
