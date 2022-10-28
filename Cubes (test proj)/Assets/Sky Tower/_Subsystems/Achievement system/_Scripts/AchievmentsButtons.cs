using System;
using UnityEngine;

public class AchievmentsButtons : MonoBehaviour
{
    public static event Action<bool> OnAchievmentsWindowOpen; 

    [SerializeField] Transform gameCanvas, achieveCanvas;

    public void OpenAchievements()
    {
        achieveCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
        SoundManager.Instance.PlayButtonSound();
        OnAchievmentsWindowOpen?.Invoke(true);
    }
    public void CloseAchievements()
    {
        achieveCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        SoundManager.Instance.PlayButtonSound();
        OnAchievmentsWindowOpen?.Invoke(false);
    }
}
