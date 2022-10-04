using UnityEngine;

public class EventCubeActivator : MonoBehaviour
{
    public delegate void Action(GameObject button);

    public static event Action activatorPressed;
    public void Activate() => activatorPressed?.Invoke(gameObject);

}
