using UnityEngine;

/// <summary>
/// A static instance is similar to a singleton, but instead of destroying any new
/// instances, it overrides the current instance. This is handy for resetting the state
/// and saves you doing it manually
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null) { _instance = new GameObject(typeof(T).Name).AddComponent<T>(); }
            }

            return _instance;
        }
    }

    public static bool HasInstance => _instance != null;
    public static T TryGetInstance() => HasInstance ? _instance : null;


    public bool AutoUnparentOnAwake = true;
    protected virtual void Awake() { InitializeSingleton(); }
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        if (AutoUnparentOnAwake) { transform.SetParent(null); }
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else { if (_instance != this) { Destroy(gameObject); } }
    }

    protected virtual void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic singleton. This will destroy any new
/// versions created, leaving the original instance intact
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        //Use protected member to prevent Null Property circular reference 
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();
    }
}

/// <summary>
/// Finally we have a persistent version of the singleton. This will survive through scene
/// loads. Perfect for system classes which require stateful, persistent data. Or audio sources
/// where music plays through loading screens, etc
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}

