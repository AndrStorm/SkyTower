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
}
