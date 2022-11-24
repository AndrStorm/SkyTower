using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField]private Material cubeLock;
    
    [SerializeField]private CubeScriptable[] cubeScriptableObjects;
    [SerializeField]private List<TextMeshProUGUI> scoreText;
    [SerializeField]private List<Transform> shopCubes;
    
    [SerializeField]private Vector3 cubeOffset;
    
    
    
    //private int[] needScore;
    
    
    
    public const string PressToSelect = "Press\n to select";
    public const string AlreadySelected = " ";

    private const string BestScore = "bestScore";

    private void Start()
    {
        int[] needScore = new int[cubeScriptableObjects.Length];
        
        int j = 0;
        foreach (var cube in cubeScriptableObjects)
        {
            needScore[j++] = cube.scoreToAchieve;
        }

        
#if UNITY_EDITOR
        
        if (needScore.Length != scoreText.Count)
                    Debug.Log("needScore.Length != scoreText.Count");
#endif
        

        for (int i = 0; i < needScore.Length; i++)
        {
            shopCubes[i].position = Helper.CanvasToWorld(scoreText[i].GetComponent<RectTransform>()) + cubeOffset;
            

            if (PlayerPrefs.GetInt(BestScore) < needScore[i])
            {
                PlayerPrefs.SetInt($"Cube{i + 1}", 0);
                scoreText[i].text = $"Score {needScore[i]} \n to unlock";
            }
            else
            {
                scoreText[i].text = AlreadySelected;
            }

            if (PlayerPrefs.GetInt($"Cube{i + 1}") == 0)
            {
                shopCubes[i].gameObject.GetComponent<MeshRenderer>().material = cubeLock;

                if (PlayerPrefs.GetInt(BestScore) >= needScore[i])
                    scoreText[i].text = PressToSelect;
            }
            
        }      
    }

    public List<Transform> GetShopCubesList()
    {
        return shopCubes;
    }
}
