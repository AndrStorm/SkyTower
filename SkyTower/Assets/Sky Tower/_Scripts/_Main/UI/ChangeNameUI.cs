using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ChangeNameUI : MonoBehaviour
{

    [SerializeField] private int maxNicknameLength = 13;
    
    [SerializeField] private TMP_InputField changeNameInput;
    [SerializeField] private RectTransform ChangeNameDialog;
    [SerializeField] private RectTransform ConfirmNameDialog;
    [SerializeField] private RectTransform WarningDialog;

    
    private bool isOpen;
    private string userNickname;
    

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
            CloseChangeNameScreen();
            return;
        }
        
        transform.GetChild(0).gameObject.SetActive(true);
    }
    
    public void CloseChangeNameScreen()
    {
        isOpen = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }


    public void ValidateNickname(string nickname)
    {
        bool validation = !(nickname.Length > maxNicknameLength || nickname.Length <= 0);
        
        if (validation) validation = !CheckBadWord(nickname);
        
        
        if (validation)
        {
            userNickname = nickname;
            ConfirmNameDialog.gameObject.SetActive(true);
            ChangeNameDialog.gameObject.SetActive(false);

            var tmpText = ConfirmNameDialog.GetChild(0).GetComponent<TMP_Text>();
            TextLocalizer.Instance.SetLocalizedText(tmpText, TextLocalizer.BASE_TABLE,"Change your Nickname to: userNickname?",userNickname);
        }
        else
        {
            WarningDialog.gameObject.SetActive(true);
            ChangeNameDialog.gameObject.SetActive(false);
        }
    }

    public void ReturnToChangeNickname()
    {
        WarningDialog.gameObject.SetActive(false);
        ConfirmNameDialog.gameObject.SetActive(false);
        ChangeNameDialog.gameObject.SetActive(true);
    }
    
    public void ChangePlayerName()
    {
        ConfirmNameDialog.gameObject.SetActive(false);
        ChangeNameDialog.gameObject.SetActive(true);
        
        if (string.CompareOrdinal(userNickname, "") != 0)
        {
            PlayerManager.Instance.SetPlayerName(userNickname);
        }
        CloseChangeNameScreen();
    }


    

    private void OnLeaderboardOpened(bool isLeaderboardOpen)
    {
        if (!isLeaderboardOpen)
        {
            CloseChangeNameScreen();
            return;
        }
        if (string.CompareOrdinal(PlayerManager.Instance.GetPlayerName(), "") == 0)
        {
            OpenChangeNameDialog();
        }
    }
    
    
    
    private static bool CheckBadWord(string nickname)
    {
        int minNicknameCheckLength = 3;
        if (nickname.Length < minNicknameCheckLength) return false;

        var banWords = GetBanWords();
        
        foreach (var word in banWords)
        {
            //индексация по никнейму
            for (int i = 0; i <= nickname.Length - minNicknameCheckLength; i++)
            {
                if (string.Compare(nickname, i, word, 0, word.Length, true) == 0)
                {
                    return true;
                }
            }
            
            
        }
        
        return false;
    }
    
    private static List<string> GetBanWords()
    {
        var data = SaveSystem.LoadBanWords();
        if (data != null)
        {
            return data.banWords;
        }

        var banWords = CreateBanWordsList();
        
        SaveSystem.SaveBanWords(new BanWordsData(banWords));

        return banWords;
    }
    
    private static List<string> CreateBanWordsList()
    {
        #region banWordsString
        string banWordsString = "badword,anal,anus,arse,assfucker,asshole,assshole,bastard,bitch,boong,cock,cockfucker,cocksuck,cocksucker,coon,coonnass,crap,cunt,cyberfuck,damn,darn,dick,dirty,douche,dummy,erect,erection,erotic,escort,fag,faggot,fuck,fuckass,fuckhole,gook,homoerotic,hore,lesbian,lesbians,negro,nigger,orgasim,orgasm,penis,penisfucker,piss,porn,pussy,retard,sadist,sex,shit,slut,suck,tits,viagra,whore$";
        #endregion
        
        var banWords = new List<string>();
        
        char separator = ',';
        char endLine = '$';
        var currentString = new List<char>();

        for (int i = 0; i < banWordsString.Length; i++)
        {
            char currentChar = banWordsString[i];
            if (currentChar.CompareTo(endLine) == 0) break;
            if (currentChar.CompareTo(separator) != 0)
            {
                currentString.Add(currentChar);
            }
            else
            {
                banWords.Add(new string(currentString.ToArray()));
                currentString.Clear();
            }
            
        }

        return banWords;
    }


}
