using UnityEngine;

[ExecuteInEditMode]
public class FaceRotation : MonoBehaviour
{
    [SerializeField]private Transform objToFace;
    
    Vector3 direction;
    

    private void Update()
    {
        if (objToFace == null) return;
        
        direction = objToFace.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(90, 0, 0);
    }
}
