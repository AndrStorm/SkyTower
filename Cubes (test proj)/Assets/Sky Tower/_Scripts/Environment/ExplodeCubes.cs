using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public GameObject exploseOnCollision;
    public GameController controller;
    private bool collisionDestroyed=false;
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Cube" && !collisionDestroyed)
        {
            for (int i = collision.transform.childCount-1; i >=0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100f, Vector3.up, 5f);
                child.SetParent(null);
            }
            Destroy(collision.gameObject);
            collisionDestroyed = true;

            GameObject explosion = Instantiate(exploseOnCollision, collision.GetContact(0).point, Quaternion.identity) as GameObject;
            Destroy(explosion, 2f);
            
            if(!controller.IsLoose())
                controller.LoseGame();

            SoundManager.Instance.PlayExplodeSound();
            GameController.playerCam.gameObject.AddComponent<ShakeCamera>();
        }
    }
}
