using TMPro;
using UnityEngine;

public class ChangeNameUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField changeNameInput;


    private bool isOpen;

    private void OnEnable()
    {
        LeaderboardUI.OnLeaderboardOpen += OnLeaderboardOpened;
    }

    private void OnDisable()
    {
        LeaderboardUI.OnLeaderboardOpen -= OnLeaderboardOpened;
    }
    
    

    public void OpenChangeNameDialog()
    {
        isOpen = !isOpen;
        if (!isOpen)
        {
            CloseChangeNameDialog();
            return;
        }
        
        transform.GetChild(0).gameObject.SetActive(true);
    }
    
    public void CloseChangeNameDialog()
    {
        isOpen = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ChangePlayerName(string nickname)
    {
        if (string.CompareOrdinal(nickname, "") != 0)
        {
            PlayerManager.Instance.SetPlayerName(nickname);
        }
        CloseChangeNameDialog();
    }



    private void OnLeaderboardOpened(bool isLeaderboardOpen)
    {
        if (!isLeaderboardOpen)
        {
            CloseChangeNameDialog();
            return;
        }
        if (string.CompareOrdinal(PlayerManager.Instance.GetPlayerName(), "") == 0)
        {
            OpenChangeNameDialog();
        }
    }
}
