using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private Transform camTransform;
    private float shakeDur = 1.2f, shakeAmount = 0.05f;
    private Vector3 originPos;

    void Start()
    {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;
        shakeAmount = shakeAmount * (1 + (camTransform.position.y - 7)/50);
        shakeDur = shakeDur * (1 + (camTransform.position.y - 7) / 50);
    }

    private void Update()
    {
        if (shakeDur > 0)
        {
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
            shakeDur -= Time.deltaTime;
        }
        else
        {
            shakeDur = 0f;
            camTransform.localPosition = originPos;
        }
    }


}
