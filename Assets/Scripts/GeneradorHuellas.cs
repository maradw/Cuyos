using UnityEngine;

public class GeneradorHuellas : MonoBehaviour
{
    public float distanciaEntreHuellas = 0.4f;
    private Vector3 ultimaPosicion;
    private Sprite spriteHuella;
    private bool piernaDerecha = true;

    void Start()
    {
        ultimaPosicion = transform.position;
        Texture2D tex = new Texture2D(32, 32);
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dx = x - 16f;
                float dy = y - 16f;
                float dist = (dx * dx) + (dy * dy);
                if (dist < 250f)
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();
        spriteHuella = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, ultimaPosicion) >= distanciaEntreHuellas)
        {
            GenerarHuella();
            ultimaPosicion = transform.position;
        }
    }

    void GenerarHuella()
    {
        GameObject huella = new GameObject("Huella");
        
        Vector3 offsetLateral = Vector3.Cross((transform.position - ultimaPosicion).normalized, Vector3.forward) * (piernaDerecha ? 0.12f : -0.12f);
        huella.transform.position = transform.position - new Vector3(0, 0.35f, 0) + offsetLateral;
        
        Vector3 dir = transform.position - ultimaPosicion;
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        huella.transform.rotation = Quaternion.Euler(0, 0, angulo);
        
        huella.transform.localScale = new Vector3(0.06f, 0.08f, 1f);

        SpriteRenderer sr = huella.AddComponent<SpriteRenderer>();
        sr.sprite = spriteHuella;
        sr.color = new Color(0.1f, 0.25f, 0.15f, 0.35f); 
        sr.sortingOrder = 1;

        huella.AddComponent<DesvanecerHuella>();
        piernaDerecha = !piernaDerecha;
    }
}

public class DesvanecerHuella : MonoBehaviour
{
    private SpriteRenderer sr;
    private float tiempoFade = 3f;
    private float alfaInicial;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) alfaInicial = sr.color.a;
        Destroy(gameObject, tiempoFade);
    }

    void Update()
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a -= (alfaInicial / tiempoFade) * Time.deltaTime;
            sr.color = c;
        }
    }
}
