using UnityEngine;

public class VfxManager : Singleton<VfxManager>
{
    [SerializeField] private GameObject[] cubesVfx;
    [SerializeField] private GameObject[] onSpawnVfx;


    [SerializeField] private float timeToDestroy = 3f;

    public void playSpawnVfx(Vector3 pos, int cubeId)
    {
        PlayVFX(cubesVfx[cubeId-1],pos,timeToDestroy);
        
        foreach (var vfx in onSpawnVfx)
        {
            PlayVFX(vfx,pos,timeToDestroy);
        }
    }
    
    private void PlayVFX(GameObject vfxSample, Vector3 pos, float timeToDestroy)
    {
        GameObject vfx = Instantiate(vfxSample, pos, Quaternion.identity) as GameObject;
        Destroy(vfx, timeToDestroy);
    }
}
