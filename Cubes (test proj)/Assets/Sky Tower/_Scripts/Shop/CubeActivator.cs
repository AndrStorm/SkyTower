using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeActivator : MonoBehaviour
{
    
    [SerializeField]private Material cubeLock;
    [SerializeField]private Material[] cubesMatUnlock;

    
    private List<MeshRenderer> shopCubesMeshRenderers;
    

    private void OnEnable() => EventCubeActivator.OnActivatorPressed += ActivateCube;

    private void OnDisable() => EventCubeActivator.OnActivatorPressed -= ActivateCube;

    
    private void Start()
    {
        shopCubesMeshRenderers = new List<MeshRenderer>();
        var shopCubes = ShopManager.Instance.GetShopCubesList();
        
        foreach (var cube in shopCubes)
        {
            shopCubesMeshRenderers.Add(cube.GetComponent<MeshRenderer>());
        }

        
#if UNITY_EDITOR
        
        if (shopCubes.Count != cubesMatUnlock.Length)
            Debug.Log("allCubes.transform.childCount != cubesUnlock.Length");
#endif
        
    }
    
    
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


        bool unlockedByScore = (score.GetComponent<TextMeshProUGUI>().text == ShopManager.PressToSelect);

        if (PlayerPrefs.GetInt($"Cube{cubeNumber}") != 0)
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 0);
            shopCubesMeshRenderers[cubeNumber-1].material = cubeLock;
            score.GetComponent<TextMeshProUGUI>().text = ShopManager.PressToSelect;
        }
        else if (unlockedByScore)
        {
            PlayerPrefs.SetInt($"Cube{cubeNumber}", 1);
            shopCubesMeshRenderers[cubeNumber - 1].material = cubesMatUnlock[cubeNumber-1];
            score.GetComponent<TextMeshProUGUI>().text = ShopManager.AlreadySelected;
        }

        SoundManager.Instance?.PlaySound("ButtonClick");

    }
}
