using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class TutorialMusicSystem : MonoBehaviour
{
    //make the variables for the FMOD event instances
    private EventInstance musicInstance;
    private EventInstance pausedInstance;
    private bool gamePaused = false;
    private float targetTime = 0.6f;

    void Start()
    {
        //assign de FMOD event variables
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Tutorial_Music");
        pausedInstance = RuntimeManager.CreateInstance("event:/Music/pauseMusic");

        //muziek start zodra attached object wordt ingeladen.
        musicInstance.start();
    }

    public void PauseAudioHandler()
    {
        Debug.Log("Debug action!!!!!!");

        if (gamePaused == false)
        {
            RuntimeManager.StudioSystem.setParameterByName("fmodpause", 1);
            musicInstance.setPaused(true);
            pausedInstance.start();
            gamePaused = true;
        }
        else
        {
            //playbackstate paused instance
            RuntimeManager.StudioSystem.setParameterByName("fmodpause", 0);
            gamePaused = false;
            StartCoroutine(MusicPauseTimer());
        }
    }

    //pauze timer coroutine
    public IEnumerator MusicPauseTimer()
    {
        yield return new WaitForSecondsRealtime(targetTime);
        Debug.Log("Pauze muziek voorbij!!!!");
        //overgang shenanigans
        musicInstance.setPaused(false);
        //pausedInstance.release();
        //pausedInstance.clearHandle();
    }
}






/* CURRENT PROBLEMS!!!
 * There is an audible gap between the pause event ending and the music event unpausing.
 * Using the update function for this is messy.. just look at the debug console messages...
 * 
 * So it looks like we *are* gonna have to use the callback bullshite method.. I DIDN'T SIGN UP FOR THIS!!!
 * 
 * 
 * 
 * WACHT NEE NEE NEEE!!!!! DIT KAN WAARSCHIJNLIJK OPGELOST WORDEN MET getParameterByName !!!!
 * 
 * Nu is het probleem dat ik niet op de juiste manier de parameter value kan zetten in fmod? fsr wordt het een oranje balletje???
 * Dus wat er nu staat is dat het in fmod de music system weer vanaf het begin start.
 */