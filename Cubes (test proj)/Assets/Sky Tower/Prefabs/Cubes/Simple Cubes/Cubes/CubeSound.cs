using UnityEngine;


public class CubeSound : MonoBehaviour
{

    [SerializeField] private AudioClip bonk;
    
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerPrefs.GetInt("sound") == 0)
        {
            return;
        }
        
        if (collision.gameObject.CompareTag("Obstacle"))
        {
           
            var pos = collision.transform.position;
            AudioSource.PlayClipAtPoint(bonk,pos);
            
        }
        
    }
    
}
