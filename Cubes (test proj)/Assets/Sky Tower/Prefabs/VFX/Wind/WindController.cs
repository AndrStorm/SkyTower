using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class WindController : MonoBehaviour
{
    [SerializeField] private int maxWindRateHeght = 100;
    [SerializeField] private float maxWindRate = 3.5f;
    
    private VisualEffect windVfx;
    private Quaternion rot = Quaternion.identity;
    private Coroutine windCoroutine;
    private float spawnRate;

    private void OnDisable()
    {
        StopCoroutine(windCoroutine);
    }

    private void Start()
    {
        windVfx = gameObject.GetComponent<VisualEffect>();
        spawnRate = windVfx.GetFloat("Spawn Rate");
        windCoroutine = StartCoroutine(CalculateWindSpawnRate());
    }

    void Update()
    {
        
        //transform.rotation *= Quaternion.Euler(new Vector3(0, RotateCamera.Instance.speed * Time.deltaTime, 0));
        //transform.rotation = RotateCamera.Instance.transform.rotation * Quaternion.Euler(0, 180, 0);


        //rot *= Quaternion.Euler(new Vector3(0, RotateCamera.Instance.speed * Time.deltaTime, 0));
        rot = RotateCamera.Instance.transform.rotation * Quaternion.Euler(0, 180, 0);
        var y = rot.eulerAngles.y;
        windVfx.SetFloat("Rotation", -y);
    }
    

    private IEnumerator CalculateWindSpawnRate()
    {
        while (true)
        {
            var newSpawnRate = Mathf.Lerp(spawnRate, maxWindRate, (GameController.Instance.GetLastCubePosition().y - 1) / maxWindRateHeght);
            windVfx.SetFloat("Spawn Rate", newSpawnRate);
            yield return Helper.GetWait(3f);
        }
    }
}
