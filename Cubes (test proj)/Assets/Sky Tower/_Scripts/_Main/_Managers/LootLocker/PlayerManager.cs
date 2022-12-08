using System;
using System.Collections;
using LootLocker.Requests;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public static event Action OnLeaderboardFetched;
    
    
    private string playerId;
    private string playerUId;
    private string playerName;

    private bool isSessionStarted;
    
    private void Start()
    {
        StartCoroutine(SetUpCoroutine());
    }


    public string GetPlayerID()
    {
        return playerId;
    }
    
    public string GetPlayerName()
    {
        if (playerName != "")
        {
            return playerName;
        }
        return playerUId;
    }
    

    public bool IsSessionStarted()
    {
        return isSessionStarted;
    }



    private IEnumerator SetUpCoroutine()
    {
        yield return StartCoroutine(LoginCouroutine());
        if (!isSessionStarted) yield break;
        
        StartCoroutine(GetPlayerNameCoroutine());
        yield return LeaderboardManager.Instance.SubmitScore();
        yield return LeaderboardManager.Instance.FetchLeaderboard();
        OnLeaderboardFetched?.Invoke();
    }
    

    private IEnumerator LoginCouroutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                playerId = response.player_id.ToString();
                playerUId = response.public_uid;
                Debug.Log("Guest Session success, Player ID: " + playerId);
                PlayerPrefs.SetString("PlayerID", playerId);
                done = isSessionStarted = true;
            }
            else
            {
                Debug.Log("Could not start guest session" + response.Error);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator GetPlayerNameCoroutine()
    {
        bool done = false;
        LootLockerSDKManager.GetPlayerName(response =>
        {
            if (response.success)
            {
                playerName = response.name;
                Debug.Log("Get Player Name - " + playerName);
                done = true;
            }
            else
            {
                Debug.Log("Failed to get player name" + response.Error);
                done = true;
            }
        });
        
        yield return new WaitWhile(() => done == false);
    }

    
    /*private async void SetUpMethod()
    {
        await LoginMethod();
        if (!LoginMethod().IsCompleted) await Task.Delay(100);
        
        LeaderboardManager.Instance.FetchLeaderboard();
    }

    private async Task LoginMethod()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                playerId = response.player_id.ToString();
                Debug.Log("Guest Session success, Player ID: " + playerId);
                PlayerPrefs.SetString("PlayerID",playerId);
                done = true;
            }
            else
            {
                Debug.Log("Could not start guest session");
                done = true;
            }
        });
        if (!done) await Task.Delay(100);
        
    }*/
}
