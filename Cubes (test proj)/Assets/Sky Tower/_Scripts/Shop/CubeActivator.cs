using System.Collections.Generic;
using UnityEngine;

public class CubeActivator : MonoBehaviour
{

    private void OnEnable() => ShopCubeUI.OnActivatorPressed += ActivateCube;

    private void OnDisable() => ShopCubeUI.OnActivatorPressed -= ActivateCube;
    
    
    
    private void ActivateCube(GameObject score)
    {
        int cubeNumber = 1;

        if (score.name == "Score1")
            cubeNumber = 1;
        else if (score.name == "Score2")
            cubeNumber = 2;
        else if (score.name == "Score3")
            cubeNumber = 3;
        else if (score.name == "Score4")
            cubeNumber = 4;
        else if (score.name == "Score5")
            cubeNumber = 5;
        else if (score.name == "Score6")
            cubeNumber = 6;
        else if (score.name == "Score7")
            cubeNumber = 7;
        else if (score.name == "Score8")
            cubeNumber = 8;
        else if (score.name == "Score9")
            cubeNumber = 9;
        else if (score.name == "Score10")
            cubeNumber = 10;
        else if (score.name == "Score11")
            cubeNumber = 11;
        else if (score.name == "Score12")
            cubeNumber = 12;


        if (PlayerPrefs.GetInt($"Cube{cubeNumber}") == 1)
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 0);
            ShopManager.Instance.LockCubeBySelection(cubeNumber);
            
        }
        else
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 1);
            ShopManager.Instance.UnlockCubeByScore(cubeNumber);
        }

        SoundManager.Instance?.PlaySound("ButtonClick");

    }
}
