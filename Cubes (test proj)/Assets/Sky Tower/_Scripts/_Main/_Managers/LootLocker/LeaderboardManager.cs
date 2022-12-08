using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using UnityEngine;

public class LeaderboardManager : Singleton<LeaderboardManager>
{

    [SerializeField] private int leaderboardId = 9370;
    
    
    private List<LeaderboardRecord> leaderboardRecords;

    

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
    
    
    

    private IEnumerator SubmitScoreCoroutine()
    {
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
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardId,10,0, (response) =>
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
                    //Debug.Log(record.place + "  " + record.name + "  " + record.score);
                }
                
                Debug.Log("Leaderboard is Fetched");
                done = true;
            }
            else
            {
                Debug.Log("Failed to fetch Leaderboard" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
    
}



public class LeaderboardRecord
{
    public string name;
    public int place;
    public int score;

}
