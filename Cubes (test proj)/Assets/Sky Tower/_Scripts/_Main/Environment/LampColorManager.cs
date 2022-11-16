using UnityEngine;

public class LampColorManager : Singleton<LampColorManager>
{
    //public static LampManager Instance { get; private set; }

    [SerializeField] private GameObject lamp;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private GameObject cubeSpawner;
    
    
    private Material lampMaterial,cubeMaterial;
    private Light lampLight;
    private float emissionIntensity;

    private void Start()
    {

        cubeMaterial = cubeSpawner.GetComponent<MeshRenderer>().material;
        lampMaterial = lamp.GetComponent<MeshRenderer>().material;
        lampLight = spotLight.GetComponent<Light>();
        
        
        Color emissionColor = cubeMaterial.GetColor("_Emission");
        emissionIntensity = CalculateHDRIntensity(emissionColor);
        
    }

    public void ChangeLampColor(Color color, float intensity, float flickerPerSecond = 0f)
    {
        
        //cubeMaterial.color = color;
        
        cubeMaterial.SetColor("_Emission",color*emissionIntensity);
        if(flickerPerSecond != 0) cubeMaterial.SetFloat("_Flicker",flickerPerSecond);
        
        lampMaterial.color = color;
        
        lampLight.color = color;
        lampLight.intensity = intensity;
    }

    private float CalculateHDRIntensity(Color emissionColor, bool debug = false)
    {
        byte k_MaxByteForOverexposedColor = 191; //internal Unity const

        var maxColorComponent = emissionColor.maxColorComponent;
        var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;

        var hdrIntensity = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);
        var factor = Mathf.Pow(2, hdrIntensity);

        if (debug) {
            print("color r " + emissionColor.r + " r after " + emissionColor.r * emissionIntensity
                  + "color g " + emissionColor.g + " g after " + emissionColor.g * emissionIntensity + "color b "
                  + emissionColor.b + " b after " + emissionColor.b * emissionIntensity);
        }

        return factor;
    }
}
