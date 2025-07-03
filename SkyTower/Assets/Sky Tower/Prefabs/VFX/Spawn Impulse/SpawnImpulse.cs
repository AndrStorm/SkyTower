using UnityEngine;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]
public class SpawnImpulse : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxLifeTime;
    /*[SerializeField] private int subVfxsCount;
    [SerializeField] private float subVfxsDelay = 0.1f;*/
    
    //private static bool _isSpawningOn = true;
    
    private GameObject currentVfx;
    
    private MeshRenderer meshRenderer;
    private float startTime;
    private float lifeTime;
    private float vfxSpeed;
    
    
    private static readonly int _time = Shader.PropertyToID("_time");

    
    private void Awake()
    {
        currentVfx = gameObject;
        meshRenderer = currentVfx.GetComponent<MeshRenderer>();
    }
    
    private void OnEnable()
    {
        startTime = Time.time;
        lifeTime = Random.Range(minMaxLifeTime.x, minMaxLifeTime.y);
        vfxSpeed = 1 / lifeTime;
        
        meshRenderer.material.SetFloat(_time, 0f);
        /*if (!_isSpawningOn) return;
        StartCoroutine(SpawnSubVfxs(subVfxsDelay, subVfxsCount));
        _isSpawningOn = false;*/
    }

    private void Update()
    {
        float ageTime = Time.time - startTime;
        if (ageTime < lifeTime)
        {
            meshRenderer.material.SetFloat(_time, ageTime * vfxSpeed);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    
    
    /*private IEnumerator SpawnSubVfxs(float delay, int count)
    {
        yield return Helper.GetWait(delay);
        for (int i = 0; i < count; i++)
        {
            GameObject vfx = Instantiate(currentVfx, currentVfx.transform.position, currentVfx.transform.rotation);
            vfx.transform.SetParent(currentVfx.transform);
            Destroy(vfx,2f);
            yield return Helper.GetWait(delay);
        }

        _isSpawningOn = true;
    }*/
    
    //private Task impulseTask;

    /*public async void StartVFX()
    {
        /*if (impulseTask == null)
        {
            impulseTask = StartImpulse();
        }
        else if (impulseTask.IsCompleted)
        {
            impulseTask = StartImpulse();
        }#1#
        
        //await StartImpulse();

    }*/

    /*private async Task StartImpulse()
    {
        float ageTime = Time.time - startTime;
        Debug.Log("start Impulse");
        
        while (ageTime < lifeTime)
        {
            meshRenderer.sharedMaterial.SetFloat(Time1, ageTime * timeRemap);
            await Task.Delay(200);
            ageTime = Time.time - startTime;
        }
        
        meshRenderer.sharedMaterial.SetFloat(Time1, 0f);
    }*/

}
