using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public static event Action<bool> OnLeaderboardOpen;


    [SerializeField] private int minLabelsFilling = 10;
    
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private RectTransform leaderboardLayoutGroup;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private TMP_Text profileName;
    [SerializeField] private Scrollbar leaderboardScrollbar;

    private bool isOpen;


    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChange;
        PlayerManager.OnSessionStarted += ActivateLeaderboardButton;
        GameSettings.OnSettingsWindowOpen += OnOtherWindowOpened;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChange;
        PlayerManager.OnSessionStarted -= ActivateLeaderboardButton;
        GameSettings.OnSettingsWindowOpen -= OnOtherWindowOpened;
    }

    
    

    private IEnumerator ResetScrollValue(Scrollbar scrollbar)
    {
        yield return null;
        yield return null;
        scrollbar.value = 1f;
    }

    private IEnumerator OpenLeaderboardCoroutine()
    {
        isOpen = !isOpen;
        if (!isOpen)
        {
            CloseLeaderboard();
            yield break;
        }
        
        //settingsAnimator.SetTrigger(AnimationOpen);
        transform.GetChild(0).gameObject.SetActive(true);
        SoundManager.Instance.PlaySound("ButtonClick");
        OnLeaderboardOpen?.Invoke(true);

        
        LeaderboardManager.Instance.UpdateScore();
        yield return LeaderboardManager.Instance.FetchLeaderboard();


        ShowPlayerName();
        
        
        
        var records = LeaderboardManager.Instance.GetLeaderboardList();
        foreach (var record in records)
        {
            CreateRecordEntry(scoreEntryPrefab,leaderboardLayoutGroup,record);
        }

        if (records.Count < minLabelsFilling)
        {
            for (int i = 0; i < minLabelsFilling - records.Count; i++)
            {
                CreateRecordEntry(scoreEntryPrefab,leaderboardLayoutGroup,"","","");
            }
        }
        
        
        StartCoroutine(ResetScrollValue(leaderboardScrollbar));
    }

    


    public void OpenLeaderboard()
    {
        StartCoroutine(OpenLeaderboardCoroutine());
    }
    
    public void CloseLeaderboard()
    {
        isOpen = false;
        
        transform.GetChild(0).gameObject.SetActive(false);
        Helper.DestroyAllChilds(leaderboardLayoutGroup);

        SoundManager.Instance?.PlaySound("ButtonClick");
        OnLeaderboardOpen?.Invoke(false);
    }
    
    
    
    private void OnSelectedLocaleChange(Locale obj)
    {
        ShowPlayerName();
    }
    
    private void ShowPlayerName()
    {
        var playerName = PlayerManager.Instance.GetPlayerNameOrUiD();
        //profileName.text = "Name:<br>" + playerName;
        TextLocalizer.Instance.SetLocalizedText(profileName,TextLocalizer.BASE_TABLE,"Name: playerName", playerName);
    }
    
    
    
    private void ActivateLeaderboardButton(bool isActive)
    {
        leaderboardButton.interactable = isActive;
    }

    private void OnOtherWindowOpened(bool isWindowOpen)
    {
        if (!isOpen) return;
        if (isWindowOpen) CloseLeaderboard();
    }

    

    private void CreateRecordEntry(GameObject prefab, RectTransform layout, LeaderboardRecord record)
    {
        string place = record.place == 0 ? "..." : record.place.ToString();
        string score = record.score == 0 ? "..." : record.score.ToString();

        CreateRecordEntry(prefab, layout, place, record.name, score);
    }
    
    
    private void CreateRecordEntry(GameObject prefab, RectTransform layout, string place, string nickName, string score)
    {
        var scoreEntry = Instantiate(prefab, layout);
        scoreEntry.name = place;


        for (int i = 0; i < scoreEntry.transform.childCount; i++)
        {
            var child = scoreEntry.transform.GetChild(i);

            if (child.name =="Place")
            {
                child.GetComponent<TMP_Text>().text = place;
            }
            else if (child.name == "Name")
            {
                child.GetComponent<TMP_Text>().text = nickName;
            }
            else if (child.name == "Score")
            {
                child.GetComponent<TMP_Text>().text = score;
            }
        }
    }
    
}
