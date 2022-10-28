using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;


public class AchievementManager : Singleton<AchievementManager>
{
    public float popupWindowDelay = 3f;
    public Transform popupWindow;
    public List<AchievmentScriptable> achievments;


    AchievmentScriptable raisingthestakes;
    AchievmentScriptable youarestartingimpress;
    AchievmentScriptable chiefEngineer;
    AchievmentScriptable skyscraper;
    AchievmentScriptable allInclusive;

    private void OnEnable()
    {
        GameController.OnDifficultyChanged += AchieveByDifficulty;
        GameController.OnScoreIncrised += AchieveByScore;
    }

    private void OnDisable()
    {
        GameController.OnDifficultyChanged -= AchieveByDifficulty;
        GameController.OnScoreIncrised -= AchieveByScore;
    }

    

    private void Start()
    {
        for (int i = 0; i < achievments.Count; i++)
        {
            if (PlayerPrefs.GetInt(achievments[i].title) != 0)
                achievments[i].SetAchieved(true);


            if (achievments[i].title == "Raising the stakes")
                raisingthestakes = achievments[i];
            if (achievments[i].title == "You are starting impress!")
                youarestartingimpress = achievments[i];
            if (achievments[i].title == "Chief Engineer")
                chiefEngineer = achievments[i];
            if (achievments[i].title == "Skyscraper")
                skyscraper = achievments[i];
            if (achievments[i].title == "All Inclusive!")
                allInclusive = achievments[i];

        }     
    }

    public void AchieveByScore(int score)
    {
        if (score >= 300 && !allInclusive.GetAchived())
        {
            Achieve(allInclusive);
        }
    }

    private void AchieveByDifficulty(int difficulty)
    {
        if (difficulty >= 1 && !raisingthestakes.GetAchived())
        {
            Achieve(raisingthestakes);
        }
        if (difficulty >= 2 && !youarestartingimpress.GetAchived())
        {
            Achieve(youarestartingimpress);
        }
        if (difficulty >= 3 && !chiefEngineer.GetAchived())
        {
            Achieve(chiefEngineer);
        }
        if (difficulty >= 4 && !skyscraper.GetAchived())
        {
            Achieve(skyscraper);
        }

    }

    private void Achieve(AchievmentScriptable achievment)
    {

#if UNITY_EDITOR
        
        Debug.Log($"{achievment.title} achieved");
        
#endif
        
        
        achievment.SetAchieved(true);
        PlayerPrefs.SetInt(achievment.title, 1);


        var popupAchievment = popupWindow.GetChild(0);

        for (int i = 0; i < popupAchievment.childCount; i++)
        {
            var child = popupAchievment.GetChild(i);
            if (child.name == "Sprite")
            {
                child.GetComponent<Image>().sprite = achievment.image;
            }
            if (child.name == "Title")
            {
                SetText(child, achievment.title);
            }
        }
        StartCoroutine(PopUpWindowEnable());
        
    }

    IEnumerator PopUpWindowEnable()
    {
        popupWindow.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupWindowDelay);
        popupWindow.gameObject.SetActive(false);
    }
    
    public static void SetText(Transform transform, string text)
    {
        var tmpro = transform.GetComponent<TextMeshProUGUI>();
        if (tmpro != null)
        {
            tmpro.text = text;
        }
        else
        {
            transform.GetComponent<Text>().text = text;
        }
    }


    [MenuItem("Developer/AchievmentsClear")]
    public static void ClearAchievmentsSave()
    {
        if (Instance == null) return;
        for (int i = 0; i < Instance.achievments.Count; i++)
        {
            Instance.achievments[i].SetAchieved(false);
            PlayerPrefs.SetInt(Instance.achievments[i].title, 0);
        }
    }
}
