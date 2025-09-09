using UnityEngine;


public class CubeSound : MonoBehaviour
{

    [SerializeField] private AudioClip bonk;
    [SerializeField] private float volume = 1f;
    
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (PlayerPrefs.GetInt("sound") == 0)
        {
            return;
        }
        
        if (collision.gameObject.CompareTag("Obstacle"))
        {
           
            var pos = collision.transform.position;
            AudioSource.PlayClipAtPoint(bonk,pos,volume);
            
        }
        
    }
    
}
