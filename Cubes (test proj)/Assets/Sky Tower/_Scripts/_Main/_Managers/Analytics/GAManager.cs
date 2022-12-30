using GameAnalyticsSDK;
using UnityEngine;

public class GAManager : Singleton<GAManager>
{

    private bool isInitialized;
    
    
    private void OnEnable()
    {
        AchievementManager.OnGetAchievment += OnGettingAchievment;
        PlayerManager.OnSessionStarted += OnSessionStarted;
    }

    private void OnDisable()
    {
        AchievementManager.OnGetAchievment -= OnGettingAchievment;
        PlayerManager.OnSessionStarted -= OnSessionStarted;
    }

    
    private void Start()
    {
        
    }
    
    
    

    private void SetUserId(string userId)
    {
        if (string.CompareOrdinal(userId,"") != 0)
        {
            GameAnalytics.SetCustomId(userId);
            Debug.Log("GA set id to - " + userId);
        }
    }
    

    private void OnSessionStarted(bool isStarted)
    {
        if (!isStarted) return;
        
        string userId = PlayerManager.Instance.GetPlayerID();
        SetUserId(userId);
        
        GameAnalytics.Initialize();
        isInitialized = true;
    }

    private void OnGettingAchievment(string title)
    {
        if(!isInitialized) return;
        
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, title);
        //GameAnalytics.NewDesignEvent ("Achievement:Killing:Neutral:10_Kills", 123);
        
        Debug.Log("GA Send data about achievment - " + title);
    }
}
