using UnityEngine;
using UnityEngine.UI;

public class zorro_code : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb2d;
    public Image img;
    public Canvas canvas;

    [Header("Patrullaje")]
    public Transform A;
    public Transform B;
    public float distanciaMinimaPunto = 0.2f;
    private Transform puntoObjetivoActual;
    public Vector2 direccion;

    [Header("Configuracion Cuy")]
    public ControladorCuy player_code;
    public GameObject player;
    public Rigidbody2D player_rb;

    [Header("Deteccion y Velocidad")]
    public float rangoDeteccion = 6f;
    public float speed = 3f;
    public float velocidadCaza = 7f;

    private float velocidadOriginal;
    public bool caza = false;
    
    [Header("Sospecha")]
    public float barra_tot = 100f;
    public float barra_act = 0f;
    public float suma = 1f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        velocidadOriginal = speed;

        if (rb2d != null && rb2d.bodyType == RigidbodyType2D.Static)
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
        
        if (transform.parent != null)
        {
            A = transform.parent.Find("a");
            if (A == null) A = transform.parent.Find("A");

            B = transform.parent.Find("b");
            if (B == null) B = transform.parent.Find("B");
        }

        if (A == null || B == null)
        {
            enabled = false;
            return;
        }

        puntoObjetivoActual = B;
        direccion = (puntoObjetivoActual.position - transform.position).normalized;

        BuscarAlJugador();

        canvas = GetComponentInChildren<Canvas>(true);
    }

    void Update()
    {
        if (player == null)
        {
            BuscarAlJugador();
            if (player == null)
            {
                PatrullarPuntos();
                return;
            }
        }

        if (img != null)
        {
            img.fillAmount = barra_act / barra_tot;
        }

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        float distanciaAlJugador = Vector2.Distance(transform.position, player.transform.position);

        if (caza)
        {
            direccion = (player.transform.position - transform.position).normalized;
            rb2d.linearVelocity = direccion * velocidadCaza;
            if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
        else
        {
            if (distanciaAlJugador <= rangoDeteccion && player_code != null && player_code.entradaMovimiento.magnitude > 0.1f && !player_code.estadoOculto)
            {
                if (barra_act < barra_tot)
                {
                    barra_act += suma;
                    rb2d.linearVelocity = Vector2.zero;
                }
                else
                {
                    caza = true;
                }
            }
            else
            {
                if (barra_act > 0)
                {
                    barra_act -= suma * 0.5f;
                }

                PatrullarPuntos();
            }
        }
    }

    private void BuscarAlJugador()
    {
        player_code = Object.FindAnyObjectByType<ControladorCuy>();
        if (player_code != null)
        {
            player = player_code.gameObject;
            player_rb = player.GetComponent<Rigidbody2D>();
            
            if (velocidadCaza <= player_code.velocidadMaxima)
            {
                velocidadCaza = player_code.velocidadMaxima + 1.2f;
            }
        }
    }

    private void PatrullarPuntos()
    {
        direccion = (puntoObjetivoActual.position - transform.position).normalized;
        rb2d.linearVelocity = direccion * speed;

        if (Vector2.Distance(transform.position, puntoObjetivoActual.position) < distanciaMinimaPunto)
        {
            puntoObjetivoActual = (puntoObjetivoActual == B) ? A : B;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            caza = false;
            barra_act = 0f;
            puntoObjetivoActual = B;
            direccion = (B.position - transform.position).normalized;
            
            if (player_code != null)
            {
                player_code.estadoActual = ControladorCuy.EstadoCuy.Agotado;
            }
            
            Destroy(player);
        }
    }

    private void OnBecameInvisible()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
