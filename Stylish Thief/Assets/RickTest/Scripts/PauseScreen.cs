using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseScreen;
    private InputAction pause;
    public MusicSystem musicSystemReference;
    private float targetTime = 0.6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = InputSystem.actions.FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        if (pause.WasPerformedThisFrame())
        {
            if (pauseScreen.activeSelf)
            {
                musicSystemReference.PauseAudioHandler();
                StartCoroutine(PauseTimer());
            }
            else
            {
                musicSystemReference.PauseAudioHandler();
                pauseScreen.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public IEnumerator PauseTimer()
    {
        yield return new WaitForSecondsRealtime(targetTime);
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Pauze voorbij!!!!");
    }
    
}