using System;
using UnityEngine;

public class TowerCollision : MonoBehaviour
{
    public static event Action<Collision> OnColide; 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            OnColide?.Invoke(collision);  
        }
        
    }
}
