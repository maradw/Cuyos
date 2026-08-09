using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AmbienteNocturno : MonoBehaviour
{
    private Light2D luzGlobal;

    void Start()
    {
        luzGlobal = GetComponent<Light2D>();
        
        if (luzGlobal != null)
        {
            luzGlobal.color = new Color(0.2f, 0.2f, 0.5f); 
            luzGlobal.intensity = 0.5f; 
        }
    }
}
