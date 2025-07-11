using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;

public class CanvasButtons : MonoBehaviour
{
    
    [SerializeField] private Sprite soundOn, soundOff;
    
    private void Start()
    {

        if (PlayerPrefs.GetInt("music") != 1 && gameObject.name=="Music")
            GetComponent<Image>().sprite = soundOff;
        else if (PlayerPrefs.GetInt("music") == 1 && gameObject.name == "Music")
            GetComponent<Image>().sprite = soundOn;
        
        else if (PlayerPrefs.GetInt("sound") != 1 && gameObject.name=="Sound")
            GetComponent<Image>().sprite = soundOff;
        else if (PlayerPrefs.GetInt("sound") == 1 && gameObject.name == "Sound")
            GetComponent<Image>().sprite = soundOn;

        else if (gameObject.name == "Score")
            GetComponent<LocalizeStringEvent>().RefreshString();
        else if (gameObject.name == "Best Score")
            GetComponent<LocalizeStringEvent>().RefreshString();
    }

    
    public void Shop()
    {
        SoundManager.Instance.PlayMusicOnTransition(SoundManager.Instance.shopTheme);
        SoundManager.Instance.PlaySound("ButtonClick");
        SceneLoader.LoadScene("Shop");
    }
    
    public void ReturnToMain()
    {
        AdsManager.Instance.HideBannerAd();
        SoundManager.Instance.PlayMusicOnTransition(SoundManager.Instance.mainTheme);
        SoundManager.Instance.PlaySound("ButtonClick");
        SceneLoader.LoadScene("Main Scene");
    }

    public void RestartGame()
    {
        HandleAdsOnRestart();
        SoundManager.Instance.PlaySound("ButtonClick");
        SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundSwitch()
    {
        if (PlayerPrefs.GetInt("sound") != 1)
        {
            PlayerPrefs.SetInt("sound", 1);
            GetComponent<Image>().sprite = soundOn;
            SoundManager.Instance.PlaySound("ButtonClick");
            SoundManager.Instance.ResetMusicVolume();
        }
        else
        {
            PlayerPrefs.SetInt("sound", 0);
            GetComponent<Image>().sprite = soundOff;
            SoundManager.Instance.SetMusicVolume(0f,0.5f);
            SoundManager.Instance.SetWindVolume(0f,0.5f);
        }

    }
    
    public void MusicSwitch()
    {
        Image btnImage = GetComponent<Image>();
        if (PlayerPrefs.GetInt("music") != 1)
        {
            PlayerPrefs.SetInt("music", 1);
            btnImage.sprite = soundOn;
            SoundManager.Instance.StartMusic();
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
            btnImage.sprite = soundOff;
            SoundManager.Instance.StopMusic();
        }

        if (PlayerPrefs.GetInt("sound") != 1) return;
        SoundManager.Instance.PlaySound("ButtonClick");

    }


    private void HandleAdsOnRestart()
    {
        AdsManager.Instance.HideBannerAd();
        int restartCount = PlayerPrefs.GetInt(GameController.RESTART_COUNTER);
        restartCount++;
        PlayerPrefs.SetInt(GameController.RESTART_COUNTER, restartCount);
        PlayerPrefs.SetInt(GameController.IS_ADS_WAS_SHOWN, 0);
    }

}
