using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeActivator : MonoBehaviour
{
    
    public Material cubeLock;
    public Material[] cubesMatUnlock;

    
    private List<GameObject> shopCubes;
    

    private void OnEnable() => EventCubeActivator.activatorPressed += ActivateCube;

    private void OnDisable() => EventCubeActivator.activatorPressed -= ActivateCube;

    private void Start()
    {
        shopCubes = ShopManager.Instance.shopCubes;
        if (shopCubes.Count != cubesMatUnlock.Length)
            Debug.Log("allCubes.transform.childCount != cubesUnlock.Length");
    }
    public void ActivateCube(GameObject score)
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


        bool unlockedByScore = (score.GetComponent<TextMeshProUGUI>().text == ShopManager.pressToSelect);

        if (PlayerPrefs.GetInt($"Cube{cubeNumber}") != 0)
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 0);
            shopCubes[cubeNumber-1].GetComponent<MeshRenderer>().material = cubeLock;
            score.GetComponent<TextMeshProUGUI>().text = ShopManager.pressToSelect;
        }
        else if (unlockedByScore)
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 1);
            shopCubes[cubeNumber - 1].GetComponent<MeshRenderer>().material = cubesMatUnlock[cubeNumber-1];
            score.GetComponent<TextMeshProUGUI>().text = ShopManager.alreadySelected;
        }

        SoundManager.Instance?.PlayButtonSound();

    }
}
