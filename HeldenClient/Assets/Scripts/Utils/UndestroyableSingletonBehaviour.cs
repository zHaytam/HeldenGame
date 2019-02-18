using System;
using UnityEngine;

public abstract class UndestroyableSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    #region Fields

    private static T _instance;

    #endregion

    #region Properties

    public static T Instance => _instance;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        _instance = GetComponent<T>();
        if (_instance == null)
            throw new Exception($"Component {typeof(T).Name} wasn't find on the gameobject.");

        DontDestroyOnLoad(this);
        OnAwake();
    }

    protected virtual void OnAwake() { }

    #endregion

}