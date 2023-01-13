using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class ExplodeCubes : MonoBehaviour
{
    [SerializeField] private float explosionForce = 100f, explosionCubeCountStep = 50f, explosionRad=5f;
    [SerializeField] [Range(0, 1)] private float minExplosionsNumberMul = 0.2f , maxExplosionsNumberMul = 0.5f;
   
    private GameObject cubesTower;
    private GameController controller;
    private List<GameController.CubeInfo> cubeInfos;
    private bool collisionDestroyed=false;

    
    
    private void Start()
    {
        controller = GameController.Instance;
        cubeInfos = controller.GetCubeInfos();
        cubesTower = controller.allCubes;
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
        if (collision.gameObject.CompareTag("Obstacle") && !collisionDestroyed)
        {
            var impactPos = collision.GetContact(0).point;
            explosionForce *= 1 + cubeInfos.Count/explosionCubeCountStep;
            explosionRad *= 1 + cubeInfos.Count/explosionCubeCountStep;
            
            
            for (int i = cubesTower.transform.childCount-1; i >=0; i--)
            {
                Transform child = cubesTower.transform.GetChild(i);
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.AddExplosionForce(explosionForce, impactPos, explosionRad);
                rb.AddExplosionForce(explosionForce/2, Vector3.zero, explosionRad);
                child.SetParent(null);
            }
            Destroy(cubesTower.gameObject);
            collisionDestroyed = true;
            
            
            VfxManager.Instance.PlayExplodeVfx(impactPos, quaternion.identity);
            
            
            int vfxQuanity = Random.Range(Mathf.RoundToInt(cubeInfos.Count * minExplosionsNumberMul), Mathf.RoundToInt(cubeInfos.Count * maxExplosionsNumberMul));
            for (int i = 0; i <= vfxQuanity-1; i++)
            {
                int randCube = Random.Range(0, cubeInfos.Count - 1);
                VfxManager.Instance.PlayCubeVfx(cubeInfos[randCube].cube.position,quaternion.identity, cubeInfos[randCube].cubeId);
                cubeInfos.Remove(cubeInfos[randCube]);
            }
            
            
            
            if(!controller.IsGameLost())
                controller.LoseGame();

            SoundManager.Instance?.PlaySound("Explode");
            CameraShaker.Instance.ShakeCamera();
            controller.SlowDownTheGame();
            
        }
    }
}
