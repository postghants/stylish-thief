using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private EventInstance menuMusicInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RuntimeManager.StudioSystem.setParameterByName("MenuButtonClicked", 0);
        menuMusicInstance = RuntimeManager.CreateInstance("event:/Music/Menu_Music");
        menuMusicInstance.start();
    }

    public void MenuTransition()
    {
        RuntimeManager.StudioSystem.setParameterByName("MenuButtonClicked", 1);
    }
}
