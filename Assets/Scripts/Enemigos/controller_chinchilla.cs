using UnityEngine;

public class controller_chinchilla : MonoBehaviour
{
    public float speed = 5f;
    public float dist_min = 0.5f;
    
    
    public Transform[] points;
    public int rng = 0;

    
    public float frecuenciaZigzag = 4f;
    public float amplitudAnguloZigzag = 20f;

    private SpriteRenderer sr;
    private Rigidbody2D rb2d;
    private Vector2 posicionBaseInterna;
    public GameObject padre;

    private float tiempoEsperaGolpe = 0f;

    void Start()
    {
        padre = transform.parent.gameObject;
        sr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        rng = 0;
        
        posicionBaseInterna = transform.position;
        rotar();
    }

    private void FixedUpdate()
    {
        if (points == null || points.Length == 0) return;

        if (tiempoEsperaGolpe > 0)
        {
            tiempoEsperaGolpe -= Time.fixedDeltaTime;
        }

        Vector2 posObjetivo = points[rng].position;
        Vector2 posActual = rb2d.position;
        Vector2 dirHaciaObjetivo = (posObjetivo - posActual).normalized;

        float anguloBase = Mathf.Atan2(dirHaciaObjetivo.y, dirHaciaObjetivo.x);
        float oscilacion = Mathf.Sin(Time.time * frecuenciaZigzag) * (amplitudAnguloZigzag * Mathf.Deg2Rad);
        float anguloFinal = anguloBase + oscilacion;

        Vector2 dirZigzag = new Vector2(Mathf.Cos(anguloFinal), Mathf.Sin(anguloFinal));

        rb2d.MovePosition(posActual + dirZigzag * speed * Time.fixedDeltaTime);

        if (Vector2.Distance(posActual, posObjetivo) < dist_min)
        {
            rng += 1;
            if (rng >= points.Length)
            {
                rng = 0;
            }
        }
        
        rotar();
    }
    
    public void rotar()
    {
        if (points == null || points.Length <= rng) return;

        Vector2 dir = (points[rng].position - transform.position).normalized;
        if (dir != Vector2.zero)
        {
            float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tiempoEsperaGolpe <= 0f)
        {
            ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
            if (cuy != null)
            {
                cuy.RecibirGolpeChinchilla(transform.position);
                
                rng += 1;
                if (points != null && rng >= points.Length) rng = 0;
                rotar();

                tiempoEsperaGolpe = 1.5f; 
            }
        }
    }
}
