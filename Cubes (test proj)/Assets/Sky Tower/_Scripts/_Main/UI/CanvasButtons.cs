using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasButtons : MonoBehaviour
{
    public Sprite soundOn, soundOff;

    private void Start()
    {
        if (PlayerPrefs.GetString("sound") == "off" && gameObject.name=="Sound")
            GetComponent<Image>().sprite = soundOff;
        else if (PlayerPrefs.GetString("sound") == "on" && gameObject.name == "Sound")
            GetComponent<Image>().sprite = soundOn;

        if (gameObject.name == "Score")
            GetComponent<TextMeshProUGUI>().text = $"Score: {PlayerPrefs.GetInt("lastScore")}";
        if (gameObject.name == "Best Score")
            GetComponent<TextMeshProUGUI>().text = $"Best Score: {PlayerPrefs.GetInt("bestScore")}";
    }

    public void Shop()
    {
        SoundManager.Instance?.PlayButtonSound();
        SceneLoader.LoadScene("Shop");
    }
    public void ReturnToMain()
    {
        SoundManager.Instance?.PlayButtonSound();
        SceneLoader.LoadScene("Main Scene");
    }

    public void RestartGame()
    {
        SoundManager.Instance?.PlayButtonSound();
        SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoundSwitch()
    {
        if (PlayerPrefs.GetString("sound") == "off")
        {
            PlayerPrefs.SetString("sound", "on");
            GetComponent<Image>().sprite = soundOn;
            SoundManager.Instance?.PlayButtonSound();
        }
        else
        {
            PlayerPrefs.SetString("sound", "off");
            GetComponent<Image>().sprite = soundOff;
        }

    }

}
