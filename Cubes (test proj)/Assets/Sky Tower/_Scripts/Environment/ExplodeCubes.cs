using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class ExplodeCubes : MonoBehaviour
{
    public GameObject cubesTower;
   
    private GameController controller;
    private List<GameController.CubeInfo> cubeInfos;
    private bool collisionDestroyed=false;

    
    
    private void Start()
    {
        controller = GameController.Instance;
        cubeInfos = controller.GetCubeInfos();
    }

    private void OnEnable()
    {
        TowerCollision.OnColide += CheckTowerCollision;
    }

    private void OnDisable()
    {
        TowerCollision.OnColide -= CheckTowerCollision;
    }


    private void CheckTowerCollision(Collision collision)
    {
        if (collision.gameObject.tag=="Obstacle" && !collisionDestroyed)
        {
            for (int i = cubesTower.transform.childCount-1; i >=0; i--)
            {
                Transform child = cubesTower.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100f, Vector3.up, 5f);
                child.SetParent(null);
            }
            Destroy(cubesTower.gameObject);
            collisionDestroyed = true;
            
            
            VfxManager.Instance.PlayExplodeVfx(collision.GetContact(0).point, quaternion.identity);
            
            
            int vfxQuanity = Random.Range(1, Mathf.RoundToInt(cubeInfos.Count / 2));
            for (int i = 0; i <= vfxQuanity-1; i++)
            {
                int randCube = Random.Range(0, cubeInfos.Count - 1);
                VfxManager.Instance.PlayCubeVfx(cubeInfos[randCube].cube.position,quaternion.identity, cubeInfos[randCube].cubeId);
                cubeInfos.Remove(cubeInfos[randCube]);
            }
            
            
            
            if(!controller.IsLoose())
                controller.LoseGame();

            SoundManager.Instance.PlayExplodeSound();
            GameController.playerCam.gameObject.AddComponent<ShakeCamera>();
            
            
        }
    }
}
