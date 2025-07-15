using System.Collections;
using UnityEngine;
using RuStore.Review;
using System;



public class RSReviewHandler : IReviewHandler
{
    //https://apps.rustore.ru/app/com.AndrStormGames.SkyTower
    //private const string _gameStoreLink = "https://apps.rustore.ru/app/com.AndrStormGames.SkyTower";
    
    private const string CATALOG_APP = "https://www.rustore.ru/catalog/app/";
    //[SerializeField] private MessageBox _messageBox;
    
    private bool _isInit;
    private bool _isRequestSucced;
    private bool _isRequestFailed;
    private bool _isLaunchSucced;
    private bool _isLaunchFailed;
    
    public IEnumerator MakeReview()
    {
        if(!_isInit) Init();
        
        RequestReviewFlow();

        int i = 0;
        while (_isRequestSucced == false && _isRequestFailed == false && i < 10)
        {
            i++;
            yield return Helper.GetUnscaledWait(0.2f);
            Helper.Log("RuStore Review is not ready yet");
        }

        if (_isRequestSucced == false)
        {
            OpenReviewInRuStore();
            yield break;
        }
        
        LaunchReviewFlow();
        
        i = 0;
        while (_isLaunchSucced == false && _isLaunchFailed == false && i < 10)
        {
            i++;
            yield return Helper.GetUnscaledWait(0.2f);
            Helper.Log("RuStore Review is not lanched yet");
        }

        if (_isLaunchSucced == false)
        {
            OpenReviewInRuStore();
            yield break;
        }
        
        Helper.Log("RuStore Review complete");
        //Application.OpenURL(@"market://details?id=" + Application.identifier);
        yield return null;
    }

    private void Init()
    {
        _isInit = true;
        RuStoreReviewManager.Instance.Init();
    }
    
    private void RequestReviewFlow() 
    {
        RuStoreReviewManager.Instance.RequestReviewFlow(
            onFailure: (error) =>
            {
                _isRequestFailed = true;
                ShowMessage("Error", string.Format("{0}: {1}", error.name, error.description));
            },
            onSuccess: () =>
            {
                _isRequestSucced = true;
                ShowMessage("Success", "");
            });
    }
    
    private void LaunchReviewFlow() 
    {
        RuStoreReviewManager.Instance.LaunchReviewFlow(
            onFailure: (error) =>
            {
                _isLaunchFailed = true;
                ShowMessage("Error", string.Format("{0}: {1}", error.name, error.description));
            },
            onSuccess: () => {
                _isLaunchSucced = true;
            });
    }
    
    private void OpenReviewInRuStore() 
    {
        var url = CATALOG_APP + Application.identifier;
        Application.OpenURL(url);
    }
    
    private void ShowMessage(string title, string message, Action onClose = null) 
    {
        Helper.Log($"Title: {title}, Messege {message}");
        
        /*_messageBox.Show(
            title: title,
            message: message,
            onClose: onClose);*/
    }
}
