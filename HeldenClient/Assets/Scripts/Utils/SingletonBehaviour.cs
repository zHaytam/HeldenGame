using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    private static readonly object MLock = new object();
    protected static T _mInstance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (MLock)
            {
                if (_mInstance == null)
                {
                    // Search for existing instance.
                    _mInstance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if (_mInstance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        _mInstance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T) + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _mInstance;
            }
        }
    }

}