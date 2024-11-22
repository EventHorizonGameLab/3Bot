using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUpdateManager : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    private readonly List<Action> _updateActions = new();

    private static GlobalUpdateManager _instance;

    public static GlobalUpdateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("GlobalUpdateManager");
                _instance = obj.AddComponent<GlobalUpdateManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void Register(Action updateAction)
    {
        if (!_updateActions.Contains(updateAction))
            _updateActions.Add(updateAction);
    }

    public void Unregister(Action updateAction)
    {
        if (_updateActions.Contains(updateAction))
            _updateActions.Remove(updateAction);
    }

    private void Update()
    {
        foreach (var action in _updateActions)
            action?.Invoke();
    }
}
