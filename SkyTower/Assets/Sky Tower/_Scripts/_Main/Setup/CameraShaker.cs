using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShaker : Singleton<CameraShaker>
{
    [Header("Base shake property")] 
    [SerializeField]private float shakeDurationMul = 1.2f;
    [SerializeField]private float shakeAmountMul=0.05f;
    [SerializeField]private Vector2 shakeSphereMinMax = new Vector2(0.3f,1f);
    
    [Header("Tower height influence property")] 
    [SerializeField]private AnimationCurve towerHeightCurve;
    [SerializeField]private float towerHeightMul=3f;
    [SerializeField]private int towerMaxHeightAffect = 100;


    public static bool isShakerOn = true;
    
    
    private float startYPos = 7f;
    private Transform camTransform;
    private float shakeDur, shakeAmount;
    private Vector3 originPos;

    void Start()
    {
        camTransform = GetComponent<Transform>();
        startYPos = camTransform.position.y;
        originPos = camTransform.localPosition;
        
    }

    private void Update()
    {
        if (GameController.isGamePause || !isShakerOn) return;
        

        if(shakeDur == 0f) return;
        if (shakeDur > 0)
        {
            //camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
            camTransform.localPosition = originPos + GetRandomInsideUnitSphere(shakeSphereMinMax.x,shakeSphereMinMax.y) * shakeAmount;
            shakeDur -= Time.unscaledDeltaTime;
        }
        else
        {
            shakeDur = 0f;
            camTransform.localPosition = originPos;
        }
    }

    public void ShakeCamera()
    {
        originPos = camTransform.localPosition;
        var position = camTransform.position;
        /*shakeAmount = shakeAmountMul * (1 + (position.y - startYPos) / 50);
        shakeDur = shakeDurationMul * (1 + (position.y - startYPos) / 50);*/
        
        shakeAmount = shakeAmountMul * (1f + towerHeightCurve.Evaluate((position.y - startYPos) / towerMaxHeightAffect) * towerHeightMul);
        shakeDur = shakeDurationMul * (1f + towerHeightCurve.Evaluate((position.y - startYPos) / towerMaxHeightAffect) * towerHeightMul);
    }
    
    public void ShakeCamera(float shakeIntensity, float duration )
    {
        originPos = camTransform.localPosition;
        shakeAmount = shakeIntensity;
        shakeDur = duration;
    }

    private Vector3 GetRandomInsideUnitSphere(float minRad,float maxRad)
    {
        var vector = new Vector3();
        vector.x = Random.Range(minRad, maxRad) * Random.value < 0.5 ? 1 : -1;
        vector.y = Random.Range(minRad, maxRad) * Random.value < 0.5 ? 1 : -1;
        vector.z = Random.Range(minRad, maxRad) * Random.value < 0.5 ? 1 : -1;

        return vector;
    }
    
    


}
