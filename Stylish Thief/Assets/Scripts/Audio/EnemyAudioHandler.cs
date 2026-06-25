using FMODUnity;
using UnityEngine;

public class EnemyAudioHandler : MonoBehaviour
{
    [SerializeField] EventReference fmodEvent1;
    [SerializeField] EventReference fmodEvent2;
    [SerializeField] EventReference fmodEvent3;
    [SerializeField] EventReference fmodEvent4;
    [SerializeField] EventReference fmodEvent5;

    public void Event1()
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent1, gameObject);
    }

    public void Event2()
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent2, gameObject);
    }

    public void Event3()
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent3, gameObject);
    }

    public void Event4()
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent4, gameObject);
    }

    public void Event5()
    {
        RuntimeManager.PlayOneShotAttached(fmodEvent5, gameObject);
    }
}
