using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null) //audiosource don't work without this if
            Instance = this as T;
        //else
        //    Destroy(gameObject);

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
        if (Instance != null) Destroy(gameObject); //don't work (reloading)
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
