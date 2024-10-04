using UnityEngine;

public class RotateCamera : Singleton<RotateCamera>
{
    public float speed = 12f;
    [SerializeField]private float loseSpeedMul = 0.75f;
    [SerializeField]private float loseCamRotOffset = 167f, loseCamRotSpeed = 1.3f;
    [SerializeField]private float rotatorMoveSpeed=1.5f;
    


    private Vector3 lastCubePos, curCubePos, rotatorStartPos, rotatorTargetPos;
    private GameController controller;
    private Transform rotator;
    private Rigidbody allCubesRB;
    private bool trackDownTowerVelocity/*, rotatorReady*/;
    private Quaternion allCubesTracker;


    void Start()
    {
        rotator = GetComponent<Transform>();
        controller = GameController.Instance;
        allCubesRB = controller.allCubes.GetComponent<Rigidbody>();
        lastCubePos = controller.GetLastCubePosition();
        rotatorStartPos = rotator.localPosition;
    }

    void Update()
    {
        bool gameLose = controller.IsGameLost();
        bool towerExplode = controller.allCubes == null;
        if (!gameLose || towerExplode /*|| rotatorReady*/)
        {
            rotator.rotation *= Quaternion.Euler(new Vector3(0, speed * Time.deltaTime, 0));
            
            if (!gameLose && lastCubePos != (curCubePos = controller.GetLastCubePosition()))
            {
                moveRotatorTarget(curCubePos);
                lastCubePos = curCubePos;
            }
            moveRotator();
        }
        else if(!towerExplode && trackDownTowerVelocity)
        {
            RotateToTracker(rotator, allCubesTracker, loseCamRotSpeed);
            moveRotator();
            
            //if (IsRotatorEqual(allCubesTracker)) //continue rotator rotation after rotation in the falling direction is finished
            //    rotatorReady = true;
        }
        else if (!trackDownTowerVelocity)
        {
            moveRotatorTarget(rotatorStartPos);
            TrackVelocity(allCubesRB.velocity, loseCamRotOffset, out allCubesTracker,false);
            trackDownTowerVelocity = true;
            speed *= loseSpeedMul;
        }
    }

    private void moveRotator()
    {
        Vector3 rotPos = rotator.localPosition;
        rotPos = Vector3.MoveTowards(rotPos, rotatorTargetPos, Time.deltaTime * rotatorMoveSpeed);
        rotator.localPosition = rotPos;
    }

    private void moveRotatorTarget(Vector3 target)
    {
        rotatorTargetPos.x = target.x;
        rotatorTargetPos.z = target.z;
    }
    

    private bool IsRotatorEqual(Quaternion Tracker)
    {
        bool IsRotatorEqual;
        int quarter = CheckQuarter(allCubesRB.velocity.x, allCubesRB.velocity.z);
        if(quarter == 1 || quarter == 4)
            IsRotatorEqual = Mathf.Approximately(rotator.rotation.x, Tracker.x) &&
            Mathf.Approximately(rotator.rotation.y, Tracker.y) &&
            Mathf.Approximately(rotator.rotation.z, Tracker.z) &&
            Mathf.Approximately(rotator.rotation.w, Tracker.w);
        else
            IsRotatorEqual = Mathf.Approximately(rotator.rotation.x, -Tracker.x) &&
            Mathf.Approximately(rotator.rotation.y, -Tracker.y) &&
            Mathf.Approximately(rotator.rotation.z, -Tracker.z) &&
            Mathf.Approximately(rotator.rotation.w, -Tracker.w);

        return IsRotatorEqual;
    }

    private void TrackVelocity(Vector3 velocity, float cinematicOffset, out Quaternion tracker, bool debug = true)
    {
        float angelOffset = 0f, cameraRotOffset = -90f, arcTan;
        int quarter = CheckQuarter(velocity.x, velocity.z);

        if (quarter == 1 || quarter == 4)
            angelOffset = 0f;
        else
            angelOffset = 180f;


        arcTan = Mathf.Atan(velocity.z / velocity.x) * Mathf.Rad2Deg;
        tracker = Quaternion.Euler(new Vector3(0, cameraRotOffset - angelOffset - arcTan + cinematicOffset, 0));

        
#if UNITY_EDITOR
        if (debug)
            Debug.Log("velocity - " + velocity.ToString() + " Atan(z/x) = " + arcTan.ToString() + " Quarter - " + quarter); 
#endif
        
    }

    
    private int CheckQuarter(float x, float z)
    {

        if (x >= 0 && z >= 0)
            return 1;

        else if (x < 0 && z >= 0)
            return 2;

        else if (x >= 0 && z < 0)
            return 4;

        else
            return 3;
    }
    

    private void RotateToTracker(Transform rotator, Quaternion tracker, float speed)
    {
        rotator.rotation = Quaternion.Lerp(rotator.rotation, tracker, Time.deltaTime * speed);
    }
}
