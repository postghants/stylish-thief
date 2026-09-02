using UnityEngine;

public class RickMusicTriggerPart : MonoBehaviour
{
    RickMusicTrigger rickMusicTrigger;
    public int musicID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rickMusicTrigger = GetComponentInParent<RickMusicTrigger>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6) { return; }
        if (musicID == 0)
        {
            rickMusicTrigger.inside0 = true;
        }
        else if (musicID == 1)
        {
            rickMusicTrigger.inside1 = true;
        }
        if (musicID == 2)
        {
            rickMusicTrigger.inside2 = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 6) { return; }
        if (musicID == 0)
        {
            rickMusicTrigger.inside0 = true;
        }
        else if (musicID == 1)
        {
            rickMusicTrigger.inside1 = true;
        }
        if (musicID == 2)
        {
            rickMusicTrigger.inside2 = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 6) { return; }
        if (musicID == 0)
        {
            rickMusicTrigger.inside0 = false;
        }
        else if (musicID == 1)
        {
            rickMusicTrigger.inside1 = false;
        }
        if (musicID == 2)
        {
            rickMusicTrigger.inside2 = false;
        }
    }
}
