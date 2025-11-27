using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;

    protected virtual void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = (T)this;
    }
}
