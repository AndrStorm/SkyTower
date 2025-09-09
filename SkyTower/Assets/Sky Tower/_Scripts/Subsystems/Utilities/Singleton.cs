using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
       if (Instance == null) 
            Instance = this as T;
    }

    protected void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

}

public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);    //without (if (Instance == null)) sound source doesn't work when switching scene back (obj destroy)
            return;                 //work fine with this, probably, scripts continue after GO destroyed
        }
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
