using UnityEngine;

public class CubeActivator : MonoBehaviour
{

    private void OnEnable() => ShopCubeLabel.OnActivatorPressed += ActivateCube;

    private void OnDisable() => ShopCubeLabel.OnActivatorPressed -= ActivateCube;
    
    
    
    private void ActivateCube(GameObject score)
    {
        int cubeIndex = ShopManager.Instance.GetLabelRefCubeID(score.name);


        if (PlayerPrefs.GetInt($"Cube{cubeIndex}") == 1)
        {
            PlayerPrefs.SetInt($"Cube{cubeIndex}", 0);
            ShopManager.Instance.LockCubeBySelection(cubeIndex);
        }
        else
        {
            PlayerPrefs.SetInt($"Cube{cubeIndex}", 1);
            ShopManager.Instance.UnlockCubeByScore(cubeIndex);
        }

        SoundManager.Instance?.PlaySound("ButtonClick");

    }
}
