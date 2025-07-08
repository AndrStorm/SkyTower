using System.Collections;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    private int FramesPerSec;
        private float frequency = 1.0f;
        private string fps;
     
        
        void Start()
        {
#if UNITY_EDITOR
            StartCoroutine(FPS());
#endif
        }
     
        private IEnumerator FPS() {
            for(;;){
                // Capture frame-per-second
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(frequency);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;
               
                // Display it
     
                fps = string.Format("<b>FPS: {0}</b>" , Mathf.RoundToInt(frameCount / timeSpan));
            }
        }
     
     
        /*void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 21;
            style.richText = true;
            GUI.Label(new Rect(Screen.width - 120,2,150,20), fps, style);
            
        }*/
}

