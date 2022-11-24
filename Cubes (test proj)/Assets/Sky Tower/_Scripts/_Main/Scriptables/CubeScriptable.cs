using UnityEngine;

[CreateAssetMenu(fileName = "Cube Example", menuName = "ScriptableObjects/Cube")]
public class CubeScriptable : ScriptableObject
{
    public int cubeId;
    public GameObject cube;
    public GameObject vfx;
    public GameObject vfxImpulse;
    public int scoreToAchieve;

    public bool active;
}
