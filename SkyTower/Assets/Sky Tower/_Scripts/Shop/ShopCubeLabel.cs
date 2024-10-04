using UnityEngine;
using UnityEngine.Localization.Components;

public class ShopCubeLabel : MonoBehaviour
{
    
    public delegate void Action(GameObject button);
    public static event Action OnActivatorPressed;


    public int needScore;
    
    //[SerializeField] private CubeScriptable cubeScriptableObject;
    
    public void Activate() => OnActivatorPressed?.Invoke(gameObject);

    private void Awake()
    {
        needScore = 666;
    }

    private void Start()
    {
        int cubeIndex = ShopManager.Instance.GetLabelRefCubeID(gameObject.name);
        needScore = ShopManager.Instance.GetNeedScoreToUnlock(cubeIndex);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<LocalizeStringEvent>().RefreshString();
        }
    }
}
