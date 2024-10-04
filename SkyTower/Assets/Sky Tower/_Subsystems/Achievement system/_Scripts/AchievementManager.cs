using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;


public class AchievementManager : Singleton<AchievementManager>
{

    public static event Action<string> OnGetAchievment; 

    [SerializeField] private float popupWindowDelay = 3f;
    [SerializeField] private Transform popupWindow;
    [SerializeField] private List<AchievmentScriptable> achievments;


    private Image popUpWindowImage;
    private TMP_Text popUpWindowText;

    AchievmentScriptable raisingTheStakes;
    AchievmentScriptable youAreStartingImpress;
    AchievmentScriptable chiefEngineer;
    AchievmentScriptable skyscraper;
    AchievmentScriptable allInclusive;
    
    private void OnEnable()
    {
        GameController.OnBestScoreIncrised += AchieveByBestScore;
    }

    private void OnDisable()
    {
        GameController.OnBestScoreIncrised -= AchieveByBestScore;
    }

    

    private void Start()
    {
        InitPopUpWindow();
        
        for (int i = 0; i < achievments.Count; i++)
        {
            if (PlayerPrefs.GetInt(achievments[i].title) != 0)
                achievments[i].SetAchieved(true);


            if (achievments[i].title == "Raising the stakes")
                raisingTheStakes = achievments[i];
            if (achievments[i].title == "You are starting impress!")
                youAreStartingImpress = achievments[i];
            if (achievments[i].title == "Chief Engineer")
                chiefEngineer = achievments[i];
            if (achievments[i].title == "Skyscraper")
                skyscraper = achievments[i];
            if (achievments[i].title == "All Inclusive!")
                allInclusive = achievments[i];

        }     
    }
    
    
    
    IEnumerator PopUpWindowEnable()
    {
        popupWindow.gameObject.SetActive(true);
        yield return Helper.GetWait(popupWindowDelay);
        popupWindow.gameObject.SetActive(false);
    }
    
    
    
    
    public List<AchievmentScriptable> GetAchievmentsList()
    {
        return achievments;
    }

    
    
    
    private void AchieveByBestScore(int score)
    {
        if (score >= allInclusive.scoreСondition && !allInclusive.GetAchived())
        {
            Achieve(allInclusive);
        }
        else if (score >= skyscraper.scoreСondition && !skyscraper.GetAchived())
        {
            Achieve(skyscraper);
        }
        else if (score >= chiefEngineer.scoreСondition && !chiefEngineer.GetAchived())
        {
            Achieve(chiefEngineer);
        }
        else if (score >= youAreStartingImpress.scoreСondition && !youAreStartingImpress.GetAchived())
        {
            Achieve(youAreStartingImpress);
        }
        else if (score >= raisingTheStakes.scoreСondition && !raisingTheStakes.GetAchived())
        {
            Achieve(raisingTheStakes);
        }
    }
    
    private void Achieve(AchievmentScriptable achievment)
    {
        SoundManager.Instance.PlaySound("Achievment");

#if UNITY_EDITOR
        Debug.Log($"{achievment.title} achieved");
#endif
        
        
        achievment.SetAchieved(true);
        PlayerPrefs.SetInt(achievment.title, 1);
        
        SetUpPopUpWindow(achievment.title,achievment.image);

        StartCoroutine(PopUpWindowEnable());
        OnGetAchievment?.Invoke(achievment.title);
    }
    
    
    
    private void InitPopUpWindow()
    {
        var popupAchievment = popupWindow.GetChild(0);

        for (int i = 0; i < popupAchievment.childCount; i++)
        {
            var child = popupAchievment.GetChild(i);
            if (child.name == "Sprite")
            {
                popUpWindowImage = child.GetComponent<Image>();
            }
            if (child.name == "Title")
            {
                popUpWindowText = child.GetComponent<TMP_Text>();
            }
        }
    }
    
    private void SetUpPopUpWindow(string achievmentTitle,Sprite sprite)
    {
        TextLocalizer.Instance.SetLocalizedText(popUpWindowText,TextLocalizer.BASE_TABLE,achievmentTitle);
        popUpWindowImage.sprite = sprite;
    }

    

    
    


    /*public static void SetText(Transform transform, string text)
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
    }*/


    
#if UNITY_EDITOR
    
    [MenuItem("Developer/Clear All Achievments #r")]
    private static void ClearAchievmentsSave()
    {
        if (Instance == null) return;
        for (int i = 0; i < Instance.achievments.Count; i++)
        {
            Instance.achievments[i].SetAchieved(false);
            PlayerPrefs.SetInt(Instance.achievments[i].title, 0);
        }
    }
    
    [MenuItem("Developer/Clear All Scores &#r")]
    private static void ClearScoresSave()
    {
        PlayerPrefs.SetInt("bestScore",0);
        PlayerPrefs.SetInt("lastScore",0);
    }
    
#endif
    
}
