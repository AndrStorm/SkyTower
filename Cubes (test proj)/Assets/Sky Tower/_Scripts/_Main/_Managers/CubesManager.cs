using System.Collections.Generic;
using UnityEngine;

public class CubesManager : Singleton<CubesManager>
{
    [SerializeField] private List<CubeScriptable> cubesScriptables;
    private Dictionary<int, CubeScriptable> cubesDict;

    protected override void Awake()
    {
        base.Awake();
        
        cubesDict = new Dictionary<int, CubeScriptable>();
        foreach (var cube in cubesScriptables)
        {
            cubesDict.Add(cube.cubeId, cube);
            cube.active = PlayerPrefs.GetInt($"Cube{cube.cubeId}") == 0 ? false : true;
        }
    }

    public Dictionary<int, CubeScriptable> GetCubesDictionary()
    {
        return cubesDict;
    }


}
