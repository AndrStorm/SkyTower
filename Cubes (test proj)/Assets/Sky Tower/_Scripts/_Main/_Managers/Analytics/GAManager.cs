using GameAnalyticsSDK;
using UnityEngine;

public class GAManager : Singleton<GAManager>
{
    
    
    private void OnEnable()
    {
        AchievementManager.OnGetAchievment += OnGettingAchievment;
    }

    private void OnDisable()
    {
        AchievementManager.OnGetAchievment -= OnGettingAchievment;
    }

    
    private void Start()
    {
        string userId = PlayerManager.Instance.GetPlayerID();
        SetUserId(userId);
        
        GameAnalytics.Initialize();
    }
    
    
    

    private void SetUserId(string userId)
    {
        if (userId != "")
        {
            GameAnalytics.SetCustomId(userId);
            Debug.Log("GA set id to - " + userId);
        }
    }
    
    
    
    private void OnGettingAchievment(string title)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, title);
        Debug.Log("Send data about achievment - " + title);
    }
}
