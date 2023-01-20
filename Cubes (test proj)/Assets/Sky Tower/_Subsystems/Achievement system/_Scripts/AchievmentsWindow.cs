using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class AchievmentsWindow : MonoBehaviour
{
    public static event Action<bool> OnAchievmentsWindowOpen; 
    
    
    [SerializeField] private Transform gameCanvas, achievmentCanvas;
    
    [SerializeField] private GameObject achievmentPrefab;
    [SerializeField] private RectTransform gridWindow;
    [SerializeField] private Scrollbar scrollbar;

    private List<AchievmentScriptable> achievments;
    
    private bool isOpen;
    private Animator achievmentsAnimator;
    private static readonly int AnimationOpen = Animator.StringToHash("OpenAchievments");
    private static readonly int AnimationClose = Animator.StringToHash("CloseAchievments");


    void Awake()
    {
        achievmentsAnimator = gameObject.GetComponent<Animator>();
        achievments = AchievementManager.Instance.GetAchievmentsList();

        InitAchievments();
    }

    private void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChange;
    }

    private void OnEnable()
    {
        StartCoroutine(ResetScrollValue(scrollbar));
        SetUpAchievmentsMask();
    }
    

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChange;;
    }


    private IEnumerator ResetScrollValue(Scrollbar scroll)
    {
        yield return null;
        scroll.value = 1f;
    }
    
    
    
    
    public void OpenAchievements()
    {
        gameCanvas.gameObject.SetActive(false);
        achievmentCanvas.gameObject.SetActive(true);
        //achievmentsAnimator.SetTrigger(AnimationOpen);
        
        SoundManager.Instance?.PlaySound("ButtonClick");
        OnAchievmentsWindowOpen?.Invoke(true);
    }
    
    public void CloseAchievements()
    {
        gameCanvas.gameObject.SetActive(true);
        achievmentCanvas.gameObject.SetActive(false);
        //achievmentsAnimator.SetTrigger(AnimationClose);
        
        SoundManager.Instance?.PlaySound("ButtonClick");
        OnAchievmentsWindowOpen?.Invoke(false);
    }

    /*public void OnAchievmentsClosed()
    {
        achievmentCanvas.gameObject.SetActive(false);
    }*/
    
    
    
    private void OnSelectedLocaleChange(Locale obj)
    {
        SetUpAchievments();
    }
    
    private void InitAchievments()
    {
        for (int i = 0; i < achievments.Count; i++)
        {
            var achievment = Instantiate(achievmentPrefab, gridWindow);
            achievment.name = $"Achievment {i} ";
        }
        
        SetUpAchievments();
    }

    private void SetUpAchievments()
    {
        for (int i = 0; i < achievments.Count; i++)
        {
            var achievment = gridWindow.GetChild(i);

            for (int j = 0; j < achievment.transform.childCount; j++)
            {
                var child = achievment.transform.GetChild(j);
                TMP_Text tmpText = child.GetComponent<TMP_Text>();

                if (child.name =="Sprite")
                {
                    child.GetComponent<Image>().sprite = achievments[i].image;
                }
                else if (child.name == "Title")
                {
                    TextLocalizer.Instance.SetLocalizedText(tmpText, TextLocalizer.BASE_TABLE, achievments[i].title);
                    //AchievementManager.SetText(child,$"{achievments[i].title}");
                }
                else if (child.name == "Description")
                {
                    TextLocalizer.Instance.SetLocalizedText(tmpText, TextLocalizer.BASE_TABLE, achievments[i].description);
                    //AchievementManager.SetText(child,achievments[i].description);
                }
            }
                      
        }
    }

    private void SetUpAchievmentsMask()
    {
        for (int i = 0; i < achievments.Count; i++)
        {
            var achievment = gridWindow.GetChild(i);

            for (int j = 0; j < achievment.childCount; j++)
            {
                var child = achievment.GetChild(j);
                
                if (child.name == "Mask")
                {
                    if (achievments[i].GetAchived())
                        child.gameObject.SetActive(false);
                    else
                        child.gameObject.SetActive(true);
                }
            }
        }
    }

}
