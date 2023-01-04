using System;
using UnityEngine;

public class ShopCubeUI : MonoBehaviour
{
    
    public delegate void Action(GameObject button);
    public static event Action OnActivatorPressed;


    public int needScore;

    [SerializeField] private int cubeIndex; 
    
    public void Activate() => OnActivatorPressed?.Invoke(gameObject);

    private void Start()
    {
        needScore = ShopManager.Instance.GetNeedScoreToUnlock(cubeIndex);
    }
}
