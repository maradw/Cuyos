using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcesamientoNoche : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (camData != null)
            {
                camData.renderPostProcessing = true;
            }
        }

        Volume vol = gameObject.AddComponent<Volume>();
        vol.isGlobal = true;
        
        VolumeProfile perfil = ScriptableObject.CreateInstance<VolumeProfile>();
        vol.profile = perfil;

        Bloom bloom = perfil.Add<Bloom>();
        bloom.active = true;
        bloom.intensity.Override(2.0f);
        bloom.threshold.Override(0.85f);
        bloom.scatter.Override(0.7f);

        Vignette vignette = perfil.Add<Vignette>();
        vignette.active = true;
        vignette.intensity.Override(0.45f);
        vignette.smoothness.Override(0.8f);
        vignette.color.Override(Color.black);
    }
}
