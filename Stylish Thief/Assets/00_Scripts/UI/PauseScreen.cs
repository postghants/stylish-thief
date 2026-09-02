using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseScreen;
    public bool stopMusicOnPauseScreens;
    private InputAction pause;
    public MusicSystem musicSystemReference;
    private float targetTime = 0.6f;
    public bool lockMouseOnExit;
    public bool setFirstObject;
    public EventSystem eventSystem;
    public GameObject button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause = InputSystem.actions.FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseScreen.activeSelf)
        {
            if (setFirstObject)
            {
                if (button != null && eventSystem != null)
                {
                    eventSystem.SetSelectedGameObject(button);
                }
            }
        }
            
        if (pause.WasPerformedThisFrame())
        {
            if (pauseScreen.activeSelf)
            {
                if (stopMusicOnPauseScreens)
                {
                    musicSystemReference.PauseAudioHandler();
                    StartCoroutine(PauseTimer());
                    if (lockMouseOnExit)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
            else
            {

                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
                if (stopMusicOnPauseScreens)
                {
                    musicSystemReference.PauseAudioHandler();

                }
                
            }
        }
    }

    public IEnumerator PauseTimer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSecondsRealtime(targetTime);
        pauseScreen.SetActive(false);

        
        Time.timeScale = 1;
    }
    public void ActivateSelf()
    {
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}