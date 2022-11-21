using UnityEngine;

public class ImpactScaler : MonoBehaviour
{
    
    [SerializeField] private int stepScaleTowerHeght = 100;
    [SerializeField] private int maxScaleTowerHeght = 2;
    
    
    private void Start()
    {
        transform.localScale *= Mathf.Clamp(1 + GameController.Instance.GetLastCubePosition().y / stepScaleTowerHeght, 1, maxScaleTowerHeght);
    }
    
}
