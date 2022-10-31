using UnityEngine;

public class EventCubeActivator : MonoBehaviour
{
    private Vector3 sceneCubePos;
    private RectTransform rectTransform;
    
    
    public delegate void Action(GameObject button);

    public static event Action activatorPressed;
    public void Activate() => activatorPressed?.Invoke(gameObject);

}
