using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float speed = 5f, trackSpeed = 2f;
    public float cinematicOffset = 140f;
    public GameController controller;


    private Transform rotator;
    private Rigidbody allCubesRB;
    private bool trackDownAllCubes/*, rotatorReady*/;
    private Quaternion allCubesTracker;


    void Start()
    {
        rotator = GetComponent<Transform>();
        allCubesRB = controller.allCubes.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!controller.IsLoose() || controller.allCubes == null /*|| rotatorReady*/)
        {
            rotator.rotation *= Quaternion.Euler(new Vector3(0, speed * Time.deltaTime, 0));
        }
        else if(controller.allCubes != null && trackDownAllCubes)
        {
            RotateToTracker(rotator, allCubesTracker, trackSpeed);
            //if (IsRotatorEqual(allCubesTracker))
            //    rotatorReady = true;
        }
        else if (!trackDownAllCubes)
        {
            trackDownAllCubes = true;
            TrackVelocity(allCubesRB.velocity, cinematicOffset, out allCubesTracker,false);
        }
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
        if (debug)
            Debug.Log("velocity - " + velocity.ToString() + " Atan(z/x) = " + arcTan.ToString() + " Quarter - " + quarter); 
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
