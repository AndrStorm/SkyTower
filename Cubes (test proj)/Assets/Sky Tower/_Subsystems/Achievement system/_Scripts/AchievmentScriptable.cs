using UnityEngine;

[CreateAssetMenu(fileName = "Achievment Example", menuName = "ScriptableObjects/Achievment")]
public class AchievmentScriptable : ScriptableObject
{
    public string title, description;
    public Sprite image;
    public int scoreСondition;

    bool achieved;
 
    public void SetAchieved(bool state) => achieved = state;
    public bool GetAchived() => achieved;
    
}
