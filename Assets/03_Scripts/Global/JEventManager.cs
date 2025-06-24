using System;
using System.Collections.Generic;
using UnityEngine;

public class JEventManager : MonoBehaviour
{
    #region VARIABLES
    private static Dictionary<Type, Delegate> _eventDict = new Dictionary<Type, Delegate>();
    #endregion





    #region FUNCTIONS
    public static void Subscribe<T>(Action<T> callback)
    {
        if(_eventDict.TryGetValue(typeof(T), out Delegate del) == true)
        {
            _eventDict[typeof(T)] = Delegate.Combine(del, callback);
        }
        else
        {
            _eventDict[typeof(T)] = callback;
        }
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        if(_eventDict.ContainsKey(typeof(T)) == true)
        {
            _eventDict[typeof(T)] = Delegate.Remove(_eventDict[typeof(T)], handler);

            if(_eventDict[typeof(T)] == null)
            {
                _eventDict.Remove(typeof(T));
            }
        }
    }

    public static void SendEvent<T>(T eventHandle)
    {
        if(_eventDict.TryGetValue(typeof(T), out Delegate del) == true)
        {
            ((Action<T>)del)?.Invoke(eventHandle);
        }
    }
    #endregion
}
