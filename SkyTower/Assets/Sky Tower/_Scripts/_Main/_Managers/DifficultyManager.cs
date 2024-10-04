using System;
using UnityEngine;

public class DifficultyManager : Singleton<DifficultyManager>
{
    public int maxDifficultyScore = 100;
    [SerializeField] private AnimationCurve difficultyCurve;
    [SerializeField] private DifficultySettings minDifficulty;
    [SerializeField] private DifficultySettings maxDifficulty;
    

    public DifficultySettings GetDifficulty(float t)
    {
        t = difficultyCurve.Evaluate(t);
        var difficulty = new DifficultySettings();
        
        difficulty.timeToPlaceCube =
            Mathf.Lerp(minDifficulty.timeToPlaceCube, maxDifficulty.timeToPlaceCube, t);
        
        difficulty.timeToCubeAutoPlace =
            Mathf.Lerp(minDifficulty.timeToCubeAutoPlace, maxDifficulty.timeToCubeAutoPlace, t);

        return difficulty;
    }
}

[Serializable]
public class DifficultySettings
{
    public float timeToPlaceCube;
    public float timeToCubeAutoPlace;

}
