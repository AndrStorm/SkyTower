using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasButtons : MonoBehaviour
{
    [SerializeField] private Sprite soundOn, soundOff;

    private void Start()
    {

        if (PlayerPrefs.GetString("music") != "on" && gameObject.name=="Music")
            GetComponent<Image>().sprite = soundOff;
        else if (PlayerPrefs.GetString("music") == "on" && gameObject.name == "Music")
            GetComponent<Image>().sprite = soundOn;
        
        else if (PlayerPrefs.GetString("sound") != "on" && gameObject.name=="Sound")
            GetComponent<Image>().sprite = soundOff;
        else if (PlayerPrefs.GetString("sound") == "on" && gameObject.name == "Sound")
            GetComponent<Image>().sprite = soundOn;

        else if (gameObject.name == "Score")
            GetComponent<TextMeshProUGUI>().text = $"Score: {PlayerPrefs.GetInt("lastScore")}";
        else if (gameObject.name == "Best Score")
            GetComponent<TextMeshProUGUI>().text = $"Best Score: {PlayerPrefs.GetInt("bestScore")}";
    }

    public void Shop()
    {
        SoundManager.Instance?.PlayMusicOnTransition(SoundManager.Instance.shopTheme);
        SoundManager.Instance?.PlaySound("ButtonClick");
        SceneLoader.LoadScene("Shop");
    }
    public void ReturnToMain()
    {
        SoundManager.Instance?.PlayMusicOnTransition(SoundManager.Instance.mainTheme);
        SoundManager.Instance?.PlaySound("ButtonClick");
        SceneLoader.LoadScene("Main Scene");
    }

    public void RestartGame()
    {
        SoundManager.Instance?.PlaySound("ButtonClick");
        SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundSwitch()
    {
        if (PlayerPrefs.GetString("sound") == "off")
        {
            PlayerPrefs.SetString("sound", "on");
            GetComponent<Image>().sprite = soundOn;
            SoundManager.Instance?.PlaySound("ButtonClick");
            SoundManager.Instance?.ResetMusicVolume();
        }
        else
        {
            PlayerPrefs.SetString("sound", "off");
            GetComponent<Image>().sprite = soundOff;
            SoundManager.Instance?.SetMusicVolume(0f,0.5f);
        }

    }
    
    public void MusicSwitch()
    {
        Image btnImage = GetComponent<Image>();
        if (PlayerPrefs.GetString("music") != "on")
        {
            PlayerPrefs.SetString("music", "on");
            btnImage.sprite = soundOn;
            SoundManager.Instance?.StartMusic();
        }
        else
        {
            PlayerPrefs.SetString("music", "off");
            btnImage.sprite = soundOff;
            SoundManager.Instance?.StopMusic();
        }

        if (PlayerPrefs.GetString("sound") != "on") return;
        
        SoundManager.Instance?.PlaySound("ButtonClick");

    }

}
