using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using UnityEngine;
using System;

public class LeaderboardManager : Singleton<LeaderboardManager>
{
    
    public static event Action<bool> OnLeaderboardFetched;

    
    [SerializeField] private int leaderboardId = 9370;
    [SerializeField] private int minTopScoreToShow = 10;
    [SerializeField] private int maxTopScoreToShow = 50;
    [SerializeField] private int minRankToShowMax = 45;
    [SerializeField] private int nearRankToShow = 30;

    
    
    private List<LeaderboardRecord> leaderboardRecords;
    private bool isBestScroreUpdated;


    private void OnEnable()
    {
        GameController.OnBestScoreIncrised += OnbestScoreUpdated;
    }

    private void OnDisable()
    {
        GameController.OnBestScoreIncrised -= OnbestScoreUpdated;
    }
    
    
    
    protected override void Awake()
    {
        base.Awake();
        leaderboardRecords = new List<LeaderboardRecord>();
        
#if UNITY_EDITOR
        for (int i = 1; i < 5; i++)
        {
            leaderboardRecords.Add(new LeaderboardRecord()
            {
                name = "person " + i,
                place = i,
                score = i*2
            });
        }
#endif
        
    }

    private void Start()
    {
        OnLeaderboardFetched?.Invoke(false);
    }


    
    private IEnumerator SubmitScoreCoroutine()
    {
        if(!PlayerManager.Instance.IsSessionStarted()) yield break;
        
        bool done = false;
        string playerID = PlayerManager.Instance.GetPlayerID();
        int score = PlayerPrefs.GetInt("bestScore");
        
        LootLockerSDKManager.SubmitScore(playerID, score, leaderboardId, (response) =>
        {
            if (response.success) {
                Debug.Log("Successful submit score");
                done = true;
            } else {
                Debug.Log("failed to submit score: " + response.Error);
                done = true;
            }
        });
        
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        yield return FetchLeaderboardTopRecords();
        yield return FetchLeaderboardNearRecords();
        
        OnLeaderboardFetched?.Invoke(true);
    }

    private IEnumerator FetchLeaderboardTopRecords()
    {
        if(!PlayerManager.Instance.IsSessionStarted()) yield break;
        
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardId,minTopScoreToShow,0, (response) =>
        {
            if (response.success)
            {
                leaderboardRecords = new List<LeaderboardRecord>();
                LootLockerLeaderboardMember[] members = response.items;
                foreach (var member in members)
                {
                    LeaderboardRecord record = new LeaderboardRecord();
                    if (member.player.name != "")
                    {
                        record.name = member.player.name;
                    }
                    else
                    {
                        record.name = member.player.public_uid;
                    }

                    record.place = member.rank;
                    record.score = member.score;

                    leaderboardRecords.Add(record);
                }
                
                Debug.Log("Leaderboard top is Fetched");
                done = true;
            }
            else
            {
                Debug.Log("Failed to fetch top Leaderboard" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        
    }

    private IEnumerator FetchLeaderboardNearRecords()
    {
        if(!PlayerManager.Instance.IsSessionStarted()) yield break;
        
        bool done = false;
        
        LootLockerSDKManager.GetMemberRank(leaderboardId,PlayerManager.Instance.GetPlayerID(), response =>
        {
            if (response.success)
            {
                Debug.Log("meta - " + response.metadata);
                
                int rank = response.rank;
                int count = 30;
                Debug.Log("Got member rank - " + rank);

                if (rank <= minRankToShowMax)
                {
                    rank = 0;
                    count = maxTopScoreToShow;
                }
                else
                {
                    LeaderboardRecord record = new LeaderboardRecord
                    {
                        name = "...",
                        place = 0,
                        score = 0
                    };
                    leaderboardRecords.Add(record);
                    
                    rank -= nearRankToShow/2;
                    count = nearRankToShow;
                }
                
                LootLockerSDKManager.GetScoreList(leaderboardId,count,rank, response2 =>
                {
                    if (response2.success)
                    {
                        LootLockerLeaderboardMember[] members = response2.items;
                        foreach (var member in members)
                        {
                            LeaderboardRecord record = new LeaderboardRecord();
                            if (member.player.name != "")
                            {
                                record.name = member.player.name;
                            }
                            else
                            {
                                record.name = member.player.public_uid;
                            }

                            record.place = member.rank;
                            record.score = member.score;

                            leaderboardRecords.Add(record);
                        }
                
                        Debug.Log("Leaderboard near is Fetched");
                    }
                    else
                    {
                        Debug.Log("Failed to fetch near Leaderboard" + response2.Error);
                    }
                });
                
                done = true;
            }
            else
            {
                Debug.Log("Failed to get member rank");
                done = true;
            }

        });
        yield return new WaitWhile(() => done == false);
        
    }
    

    public void UpdateScore()
    {
        if (isBestScroreUpdated)
        {
            isBestScroreUpdated = false;
        }
    }
    
    public Coroutine SubmitScore()
    {
        return StartCoroutine(SubmitScoreCoroutine());
    }

    public Coroutine FetchLeaderboard()
    {
        return StartCoroutine(FetchLeaderboardCoroutine());
    }

    public List<LeaderboardRecord> GetLeaderboardList()
    {
        return leaderboardRecords;
    }

    

    private void OnbestScoreUpdated(int score)
    {
        isBestScroreUpdated = true;
    }

    
}


public class LeaderboardRecord
{
    public string name;
    public int place;
    public int score;

}
