using UnityEngine;
using UnityEngine.Tilemaps;

public class CamaraDinamica : MonoBehaviour
{
    public static CamaraDinamica Instance { get; private set; }

    public Transform objetivo;
    public Tilemap mapa;
    public float suavizado = 5f;
    public float zoomDeseado = 6.5f;

    private Camera cam;
    private Vector2 limiteMin;
    private Vector2 limiteMax;
    
    private float tiempoTemblor = 0f;
    private float intensidadTemblor = 0f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = zoomDeseado;
        }

        if (mapa != null)
        {
            mapa.CompressBounds();
            Bounds limitesMapa = mapa.localBounds;
            
            float altoCamara = cam.orthographicSize;
            float anchoCamara = altoCamara * cam.aspect;
            
            float altoMapa = limitesMapa.max.y - limitesMapa.min.y;
            float anchoMapa = limitesMapa.max.x - limitesMapa.min.x;

            if (anchoMapa < anchoCamara * 2f)
            {
                limiteMin.x = limitesMapa.center.x;
                limiteMax.x = limitesMapa.center.x;
            }
            else
            {
                limiteMin.x = limitesMapa.min.x + anchoCamara;
                limiteMax.x = limitesMapa.max.x - anchoCamara;
            }

            if (altoMapa < altoCamara * 2f)
            {
                limiteMin.y = limitesMapa.center.y;
                limiteMax.y = limitesMapa.center.y;
            }
            else
            {
                limiteMin.y = limitesMapa.min.y + altoCamara;
                limiteMax.y = limitesMapa.max.y - altoCamara;
            }
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;
        
        Vector3 posObjetivo = new Vector3(objetivo.position.x, objetivo.position.y, transform.position.z);
        Vector3 posSuavizada = Vector3.Lerp(transform.position, posObjetivo, suavizado * Time.deltaTime);
        
        if (mapa != null)
        {
            posSuavizada.x = Mathf.Clamp(posSuavizada.x, limiteMin.x, limiteMax.x);
            posSuavizada.y = Mathf.Clamp(posSuavizada.y, limiteMin.y, limiteMax.y);
        }
        
        if (tiempoTemblor > 0)
        {
            posSuavizada += (Vector3)Random.insideUnitCircle * intensidadTemblor;
            tiempoTemblor -= Time.deltaTime;
        }

        transform.position = posSuavizada;
    }

    public void ActivarTemblor(float intensidad, float duracion)
    {
        intensidadTemblor = intensidad;
        tiempoTemblor = duracion;
    }
}
