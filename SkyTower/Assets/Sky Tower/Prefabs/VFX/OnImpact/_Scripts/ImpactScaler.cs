using UnityEngine;

public class ImpactScaler : MonoBehaviour
{
    
    [SerializeField] private int maxScaleTowerHeght = 200;
    [SerializeField] private float maxScaleMul = 3;
    
    
    private void Start()
    {
        transform.localScale *= Mathf.Lerp(1, maxScaleMul, GameController.Instance.GetLastCubePosition().y / maxScaleTowerHeght);
    }
    
}
