using UnityEngine;
using UnityEngine.VFX;

public class VfxManager : Singleton<VfxManager>
{
    
    [SerializeField] private GameObject[] onSpawnVfx;
    [SerializeField] private GameObject[] onExplodeVfx;
    [SerializeField] private float timeToDestroy = 3f;

    
    private GameObject[] cubesVfx;
    
    
    private void Start()
    {
        cubesVfx = new GameObject[CubesManager.Instance.cubesDict.Count];
        foreach (var cube in CubesManager.Instance.cubesDict)
        {
            cubesVfx[cube.Key - 1] = cube.Value.vfx;
        }
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
        PlayVFX(cubesVfx[cubeId - 1], pos, rot, timeToDestroy);
    }
    

    public void PlaySpawnVfx(Vector3 pos, Quaternion rot, int cubeId)
    {
        PlayVFX(cubesVfx[cubeId-1],pos, rot, timeToDestroy);
        
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
