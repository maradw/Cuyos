using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AuraCuy : MonoBehaviour
{
    void Start()
    {
        Light2D luz = gameObject.AddComponent<Light2D>();
        
        luz.lightType = Light2D.LightType.Point;
        luz.color = new Color(1f, 0.85f, 0.5f);
        luz.intensity = 1.2f;
        
        luz.pointLightOuterRadius = 4.5f;
        luz.pointLightInnerRadius = 1f;
        
        luz.falloffIntensity = 0.6f;
    }
}
