using UnityEngine;

public class EventCubeActivator : MonoBehaviour
{
    
    public delegate void Action(GameObject button);
    public static event Action OnActivatorPressed;
    
    public void Activate() => OnActivatorPressed?.Invoke(gameObject);

}
