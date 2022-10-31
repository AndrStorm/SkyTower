using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    public Material cubeLock;
    public int[] needScore;
    public List<TextMeshProUGUI> scoreText;
    public List<GameObject> shopCubes;

    public Vector3 cubeOffset;
    
    
    public const string pressToSelect = "Press\n to select";
    public const string alreadySelected = " ";

    private const string bestScore = "bestScore";

    void Start()
    {

#if UNITY_EDITOR
        
        if (needScore.Length != scoreText.Count)
                    Debug.Log("needScore.Length != scoreText.Count");
#endif
        

        for (int i = 0; i < needScore.Length; i++)
        {
            shopCubes[i].transform.position = Helper.CanvasToWorld(scoreText[i].GetComponent<RectTransform>()) + cubeOffset;
            

            if (PlayerPrefs.GetInt(bestScore) < needScore[i])
            {
                PlayerPrefs.SetInt($"Cube{i + 1}", 0);
                scoreText[i].text = $"Score {needScore[i]} \n to unlock";
            }
            else
            {
                scoreText[i].text = alreadySelected;
            }

            if (PlayerPrefs.GetInt($"Cube{i + 1}") == 0)
            {
                shopCubes[i].gameObject.GetComponent<MeshRenderer>().material = cubeLock;

                if (PlayerPrefs.GetInt(bestScore) >= needScore[i])
                    scoreText[i].text = pressToSelect;
            }
            
        }      
    }
}
