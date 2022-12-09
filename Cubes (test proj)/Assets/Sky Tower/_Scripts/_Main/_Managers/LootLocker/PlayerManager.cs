using System;
using System.Collections;
using LootLocker.Requests;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{

    public static event Action<bool> OnSessionStarted;

    private string playerId;
    private string playerUId;
    private string playerName;

    private bool isSessionStarted;
    
    
    private void Start()
    {
        OnSessionStarted?.Invoke(false);
        StartCoroutine(SetUpCoroutine());
    }
    
    
    private IEnumerator SetUpCoroutine()
    {
        yield return StartCoroutine(LoginCouroutine());
        if (!isSessionStarted) yield break;
        OnSessionStarted?.Invoke(true);
        
        StartCoroutine(GetPlayerNameCoroutine());
        yield return LeaderboardManager.Instance.SubmitScore();
        //yield return LeaderboardManager.Instance.FetchLeaderboard();
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
        if(!isSessionStarted) yield break;
        
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

    private IEnumerator SetPlayerNameCoroutine(string nickname)
    {
        if(!isSessionStarted) yield break;
        
        bool done = false;
        LootLockerSDKManager.SetPlayerName(nickname, response =>
        {
            if (response.success)
            {
                playerName = response.name;
                Debug.Log("Name changed to " + playerName);
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


    
    public void SetPlayerName(string nickname)
    {
        StartCoroutine(SetPlayerNameCoroutine(nickname));
    }
    
    public string GetPlayerID()
    {
        return playerId;
    }
    
    public string GetPlayerName()
    {
        return playerName;
    }

    public string GetPlayerNameOrUiD()
    {
        if (playerName != "")
        {
            return GetPlayerName();
        }
        return playerUId;
    }
    
    public bool IsSessionStarted()
    {
        return isSessionStarted;
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
