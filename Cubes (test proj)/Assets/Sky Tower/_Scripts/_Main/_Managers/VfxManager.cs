using UnityEngine;
using UnityEngine.VFX;

public class VfxManager : Singleton<VfxManager>
{

    [SerializeField] private Transform wind;
    [SerializeField] private GameObject[] onSpawnVfx;
    [SerializeField] private GameObject[] onExplodeVfx;
    [SerializeField] private float timeToDestroy = 3f;


    private GameObject[,] cubesVfx;
    private VisualEffect windVfx;


    private void Start()
    {
        windVfx = wind.gameObject.GetComponent<VisualEffect>();


        var cubesDict = CubesManager.Instance.GetCubesDictionary();
        cubesVfx = new GameObject[cubesDict.Count, 2];
        foreach (var cube in cubesDict)
        {
            cubesVfx[cube.Key - 1, 0] = cube.Value.vfx;
            cubesVfx[cube.Key - 1, 1] = cube.Value.vfxImpulse;
        }
        
    }
    

    public void EnableWind(bool val)
    {
        wind.gameObject.SetActive(val);
    }

    public void MoveWindVFX(Vector3 pos)
    {
        wind.transform.position = pos;
    }
    
    
    public void PlayExplodeVfx(Vector3 pos, Quaternion rot)
    {
        foreach (var vfx in onExplodeVfx)
        {
            PlayVFX(vfx, pos, rot, timeToDestroy*2f);
        }
    }
    
    
    public void PlayCubeVfx(Vector3 pos, Quaternion rot, int cubeId)
    {
        PlayVFX(cubesVfx[cubeId - 1, 0], pos, rot, timeToDestroy);
    }
    

    public void PlaySpawnVfx(Vector3 pos, Quaternion rot, int cubeId)
    {
        PlayVFX(cubesVfx[cubeId-1, 0],pos, rot, timeToDestroy);
        PlayVFX(cubesVfx[cubeId-1, 1],pos, rot, timeToDestroy);
        
        foreach (var vfx in onSpawnVfx)
        {
            PlayVFX(vfx, pos, rot, timeToDestroy);
        }
    }
    
    
    
    private void PlayVFX(GameObject vfxSample, Vector3 pos, Quaternion rot, float time)
    {
     
        GameObject vfx = Instantiate(vfxSample, pos, rot) as GameObject;
        Destroy(vfx, time);
    }
    
    private void PlayVFX(VisualEffect vfxSample, Vector3 pos, Quaternion rot, float time)
    {
        GameObject vfx = Instantiate(new GameObject(), pos, rot) as GameObject;
        var vfxComp = vfx.AddComponent<VisualEffect>();
        vfxComp.visualEffectAsset = vfxSample.visualEffectAsset; 
        Destroy(vfx, time);
    }
}
