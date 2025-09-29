using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    public static Singleton<T> Instance;

    protected virtual void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
