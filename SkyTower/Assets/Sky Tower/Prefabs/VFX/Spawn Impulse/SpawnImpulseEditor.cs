#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(SpawnImpulse))]
public class SpawnImpulseEditor : Editor
{
    private SpawnImpulse impulse;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //impulse.StartVFX();

        /*using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (check.changed)
            {
                impulse.StartVFX();
            }
        }*/
        
    }

    private void OnEnable()
    {
        impulse = (SpawnImpulse)target;
    }
}

#endif