using UnityEngine;

[CreateAssetMenu(fileName = "Achievment Example")]
public class AchievmentScriptable : ScriptableObject
{
    public string title, description;
    public Sprite image;

    bool achieved;

    public void SetAchieved(bool state) => achieved = state;
    public bool GetAchived() => achieved;
    
}
