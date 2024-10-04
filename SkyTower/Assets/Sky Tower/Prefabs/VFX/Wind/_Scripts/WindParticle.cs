using System.Collections;
using UnityEngine;

public class WindParticle : MonoBehaviour
{
    [SerializeField] private int maxWindRateHeght = 100;
    [SerializeField] private float maxWindRate = 3.5f;

    private RotateCamera rotator;
    private ParticleSystem windVfx;
    private Coroutine windCoroutine;
    private float spawnRate;

    private void OnDisable()
    {
        StopCoroutine(windCoroutine);
    }

    private void Start()
    {
        rotator = RotateCamera.Instance;
        windVfx = gameObject.GetComponent<ParticleSystem>();
        spawnRate = windVfx.emission.rateOverTime.constant;
        windCoroutine = StartCoroutine(CalculateWindSpawnRate());
    }

    void Update()
    {
        transform.rotation = rotator.transform.rotation * Quaternion.Euler(0, 180, 0);
    }
    

    private IEnumerator CalculateWindSpawnRate()
    {
        ParticleSystem.EmissionModule emission = windVfx.emission;
        while (true)
        {
            var newSpawnRate = Mathf.Lerp(spawnRate, maxWindRate, (GameController.Instance.GetLastCubePosition().y - 1) / maxWindRateHeght);
            emission.rateOverTime = newSpawnRate;
            yield return Helper.GetWait(3f);
        }
    }
    
}
