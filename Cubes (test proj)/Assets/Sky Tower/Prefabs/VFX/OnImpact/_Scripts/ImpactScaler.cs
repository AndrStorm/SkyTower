using UnityEngine;

public class ImpactScaler : MonoBehaviour
{
    
    [SerializeField] private int maxScaleTowerHeght = 100;
    [SerializeField] private float maxScaleMul = 2;
    
    
    private void Start()
    {
        transform.localScale *= Mathf.Lerp(1, maxScaleMul, GameController.Instance.GetLastCubePosition().y / maxScaleTowerHeght);
    }
    
}
