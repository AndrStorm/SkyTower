using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    private static Camera _mainCamer;

    public static Camera MainCamera
    {
        get
        {
            if (_mainCamer == null) _mainCamer = Camera.main;
            {
                return _mainCamer;
            }
        }
    }

    public static Vector3 CanvasToWorld(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var pos);
        return pos;
    }
    
    
    

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
        
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }
    
    
    
    private static readonly Dictionary<float, WaitForSecondsRealtime> UnscaledWaitDictionary = new Dictionary<float, WaitForSecondsRealtime>();
    public static WaitForSecondsRealtime GetUnscaledWait(float time)
    {
        if (UnscaledWaitDictionary.TryGetValue(time, out var wait)) return wait;
        
        UnscaledWaitDictionary[time] = new WaitForSecondsRealtime(time);
        return UnscaledWaitDictionary[time];
    }
}
