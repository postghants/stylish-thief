using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    public float volume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        if (PlayerPrefs.HasKey("Volume"))
        {
            slider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            slider.value = slider.maxValue / 2;
            volume = slider.value;
            PlayerPrefs.SetFloat("Volume", volume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTheVolume()
    {
        volume = slider.value;
        PlayerPrefs.SetFloat("Volume", volume);
        //Change the volume number in the FMod stuff
        RuntimeManager.StudioSystem.setParameterByName("VolumeSliderValue", volume);
    }
}
