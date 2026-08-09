using UnityEngine;
using System.Collections;

public class MonoTiti : MonoBehaviour
{
    public float velocidad = 7f;
    public float tiempoEntrePlatanos = 3f;
    public GameObject prefabPlatano;
    
    public Transform A;
    public Transform B;
    
    private Transform puntoObjetivo;
    private float temporizadorPlatano;
    private Rigidbody2D rb2d;
    
    private bool estaBurlon = false;
    private ControladorCuy cuyObjetivo;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;

        cuyObjetivo = FindAnyObjectByType<ControladorCuy>();

        if (A == null) A = transform.Find("RutaMono/A") ?? transform.Find("A");
        if (B == null) B = transform.Find("RutaMono/B") ?? transform.Find("B");

        if (transform.parent != null)
        {
            if (A == null) A = transform.parent.Find("a") ?? transform.parent.Find("A");
            if (B == null) B = transform.parent.Find("b") ?? transform.parent.Find("B");
        }

        if (A != null) A.SetParent(null);
        if (B != null) B.SetParent(null);

        puntoObjetivo = B;
        temporizadorPlatano = tiempoEntrePlatanos;
    }

    void FixedUpdate()
    {
        if (A == null || B == null || estaBurlon) return;

        Vector2 dir = ((Vector2)puntoObjetivo.position - rb2d.position).normalized;
        rb2d.MovePosition(rb2d.position + dir * velocidad * Time.fixedDeltaTime);

        if (Vector2.Distance(rb2d.position, puntoObjetivo.position) < 0.3f)
        {
            puntoObjetivo = (puntoObjetivo == A) ? B : A;
        }

        RotarHacia(dir);
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
        }

        temporizadorPlatano -= Time.fixedDeltaTime;
        if (temporizadorPlatano <= 0f)
        {
            SoltarPlatano();
            temporizadorPlatano = tiempoEntrePlatanos;
        }
    }

    private void RotarHacia(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }

    private void SoltarPlatano()
    {
        if (prefabPlatano != null)
        {
            GameObject platano = Instantiate(prefabPlatano, transform.position, Quaternion.identity);
            
            if (cuyObjetivo != null && Vector2.Distance(transform.position, cuyObjetivo.transform.position) < 5f)
            {
                CascaraPlatano script = platano.GetComponent<CascaraPlatano>();
                if (script != null)
                {
                    script.LanzarHacia(cuyObjetivo.transform.position);
                }
            }
            Destroy(platano, 8f);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null && !estaBurlon)
        {
            cuy.PerderUltimoInsumo();
            
            puntoObjetivo = (puntoObjetivo == A) ? B : A;
        }
    }

    public void IniciarBurla()
    {
        if (!estaBurlon && gameObject.activeInHierarchy)
        {
            StartCoroutine(RutinaBurla());
        }
    }

    private IEnumerator RutinaBurla()
    {
        estaBurlon = true;
        
        if (cuyObjetivo != null)
        {
            Vector2 dir = (cuyObjetivo.transform.position - transform.position).normalized;
            RotarHacia(dir);
        }

        float duracion = 2f;
        float tiempo = 0f;
        Vector3 posOriginal = transform.position;
        
        while (tiempo < duracion)
        {
            float salto = Mathf.Abs(Mathf.Sin(tiempo * 15f)) * 0.3f;
            transform.position = posOriginal + new Vector3(0, salto, 0);
            tiempo += Time.deltaTime;
            yield return null;
        }
        
        transform.position = posOriginal;
        estaBurlon = false;
    }
}
