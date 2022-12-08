using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public static event Action<bool> OnLeaderboardOpen;


    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private RectTransform leaderboardLayoutGroup;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private TextMeshProUGUI profileName;
    [SerializeField] private Scrollbar leaderboardScrollbar;

    private bool isOpen;


    private void OnEnable()
    {
        PlayerManager.OnLeaderboardFetched += ActivateLeaderboardButton;
        GameSettings.OnSettingsWindowOpen += OnOtherWindowOpened;
    }

    private void OnDisable()
    {
        PlayerManager.OnLeaderboardFetched -= ActivateLeaderboardButton;
        GameSettings.OnSettingsWindowOpen -= OnOtherWindowOpened;
    }


    private IEnumerator ResetScrollValue(Scrollbar scrollbar)
    {
        yield return null;
        scrollbar.value = 1f;
    }
    
    
    
    
    public void OpenLeaderboard()
    {
        isOpen = !isOpen;
        if (!isOpen)
        {
            CloseLeaderboard();
            return;
        }


        var playerName = PlayerManager.Instance.GetPlayerName();
        profileName.text = "Name:<br>" + playerName;
        

        var records = LeaderboardManager.Instance.GetLeaderboardList();
        foreach (var record in records)
        {
            CreateRecordEntry(scoreEntryPrefab,leaderboardLayoutGroup,record);
        }

        if (records.Count < 15)
        {
            for (int i = 0; i < 15 - records.Count; i++)
            {
                CreateRecordEntry(scoreEntryPrefab,leaderboardLayoutGroup,"","","");
            }
        }
        
        
        //LeaderboardManager.Instance.FetchLeaderboard();
        
        StartCoroutine(ResetScrollValue(leaderboardScrollbar));

        //settingsAnimator.SetTrigger(AnimationOpen);
        transform.GetChild(0).gameObject.SetActive(true);
        
        SoundManager.Instance?.PlaySound("ButtonClick");
        OnLeaderboardOpen?.Invoke(true);

    }
    
    public void CloseLeaderboard()
    {
        isOpen = false;
        
        transform.GetChild(0).gameObject.SetActive(false);
        Helper.DestroyAllChilds(leaderboardLayoutGroup);

        SoundManager.Instance?.PlaySound("ButtonClick");
        OnLeaderboardOpen?.Invoke(false);
    }
    
    
    
    
    private void ActivateLeaderboardButton()
    {
        leaderboardButton.interactable = true;
    }

    private void OnOtherWindowOpened(bool isWindowOpen)
    {
        if (!isOpen) return;
        if (isWindowOpen) CloseLeaderboard();
    }

    

    private void CreateRecordEntry(GameObject prefab, RectTransform layout, LeaderboardRecord record)
    {
        CreateRecordEntry(prefab, layout, record.place.ToString(), record.name, record.score.ToString());
    }
    
    private void CreateRecordEntry(GameObject prefab, RectTransform layout, string place, string name, string score)
    {
        var scoreEntry = Instantiate(prefab, layout);
        scoreEntry.name = place;


        for (int i = 0; i < scoreEntry.transform.childCount; i++)
        {
            var child = scoreEntry.transform.GetChild(i);

            if (child.name =="Place")
            {
                child.GetComponent<TextMeshProUGUI>().text = place;
            }
            else if (child.name == "Name")
            {
                child.GetComponent<TextMeshProUGUI>().text = name;
            }
            else if (child.name == "Score")
            {
                child.GetComponent<TextMeshProUGUI>().text = score;
            }
        }
    }
    
}
