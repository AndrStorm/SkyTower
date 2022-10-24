using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Material cubeLock;
    public int[] needScore;
    public List<Text> scoreText;

    public const string pressToSelect = "Press\n to select";
    public const string alreadySelected = " ";

    string bestScore = "bestScore";

    void Start()
    {
        if (needScore.Length != scoreText.Count)
            Debug.LogError("needScore.Length != scoreText.Count");

        for (int i = 0; i < needScore.Length; i++)
        {

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
                transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = cubeLock;

                if (PlayerPrefs.GetInt(bestScore) >= needScore[i])
                    scoreText[i].text = pressToSelect;
            }
            
        }      
    }
}
