using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

        for (int i = 0; i < achievments.Count; i++)
        {
            var icon = Instantiate(achievmentPrefab, gridWindow) as GameObject;
            icon.name = $"Achievment {i} ";


            for (int j = 0; j < icon.transform.childCount; j++)
            {
                var child = icon.transform.GetChild(j);

                if (child.name =="Sprite")
                {
                    child.GetComponent<Image>().sprite = achievments[i].image;
                }
                else if (child.name == "Title")
                {
                    AchievementManager.SetText(child,$"{achievments[i].title}");
                }
                else if (child.name == "Description")
                {
                    AchievementManager.SetText(child,achievments[i].description);
                }
            }
                      
        }
    }
    private void OnEnable()
    {
        StartCoroutine(ResetScrollValue());
        

        for (int i = 0; i < achievments.Count; i++)
        {
            var icon = gridWindow.GetChild(i);

            for (int j = 0; j < icon.childCount; j++)
            {
                var child = icon.GetChild(j);
                
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

    private IEnumerator ResetScrollValue()
    {
        yield return null;
        scrollbar.value = 1f;
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

    public void OnAchievmentsClosed()
    {
        achievmentCanvas.gameObject.SetActive(false);
    }

}
