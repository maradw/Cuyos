using UnityEngine;
using UnityEngine.UI;

public class zorro_code : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb2d;
    public Image img;
    public Canvas canvas;
    private Animator animador;

    [Header("Patrullaje")]
    public Transform A;
    public Transform B;
    public float distanciaMinimaPunto = 0.2f;
    public float tiempoDeEsperaEnPunto = 1.5f;
    private Transform puntoObjetivoActual;
    public Vector2 direccion;
    private float temporizadorEspera = 0f;
    private bool estaEsperando = false;

    [Header("Configuracion Cuy")]
    public ControladorCuy player_code;
    public GameObject player;
    public Rigidbody2D player_rb;

    [Header("Deteccion y Velocidad")]
    public float rangoDeteccion = 12.0f; 
    public float rangoDeteccionProximidad = 6.0f; 
    public float speed = 3f;
    public float velocidadCaza = 7f;
    public float velocidadGiroZorro = 8f;

    private float velocidadOriginal;
    public bool caza = false;
    
    [Header("Sospecha")]
    public float barra_tot = 100f;
    public float barra_act = 0f;
    public float velocidadLlenadoBarra = 15f; 

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
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
        if (player == null || player_code == null)
        {
            player = null;
            player_code = null;
            player_rb = null;

            BuscarAlJugador();
            if (player == null)
            {
                PatrullarPuntos();
                ActualizarAnimaciones();
                return;
            }
        }

        ActualizarBarraDeSospecha();

        float anguloObjetivo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg + 90f;
        float anguloSuave = Mathf.LerpAngle(transform.eulerAngles.z, anguloObjetivo, velocidadGiroZorro * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, anguloSuave);

        if (canvas != null)
        {
            canvas.transform.position = transform.position + new Vector3(0f, 1.3f, 0f);
            canvas.transform.rotation = Quaternion.identity;
        }

        float distanciaAlJugador = 999f;
        try
        {
            if (player != null)
            {
                distanciaAlJugador = Vector2.Distance(transform.position, player.transform.position);
            }
        }
        catch (System.Exception)
        {
            player = null;
            player_code = null;
            player_rb = null;
            PatrullarPuntos();
            ActualizarAnimaciones();
            return;
        }

        if (caza)
        {
            estaEsperando = false;
            try
            {
                direccion = (player.transform.position - transform.position).normalized;
                rb2d.linearVelocity = direccion * velocidadCaza;
            }
            catch (System.Exception)
            {
                caza = false;
                barra_act = 0f;
                rb2d.linearVelocity = Vector2.zero;
            }
            
            if (canvas != null)
            {
                canvas.enabled = true;
            }
        }
        else
        {
            bool detectadoNormal = (distanciaAlJugador <= rangoDeteccion && player_code != null && player_code.entradaMovimiento.magnitude > 0.1f && !player_code.estadoOculto);
            bool detectadoPorProximidad = (distanciaAlJugador <= rangoDeteccionProximidad);

            if (detectadoNormal || detectadoPorProximidad)
            {
                estaEsperando = false;
                
                if (barra_act < barra_tot)
                {
                    float factorCercania = 1f - Mathf.Clamp01(distanciaAlJugador / rangoDeteccion);
                    float multiplicadorProximidad = Mathf.Lerp(1f, 4f, factorCercania);
                    
                    float modificadorSigilo = 2.2f;
                    if (player_code != null && player_code.estadoOculto)
                    {
                        modificadorSigilo = 0.4f;
                    }
                    
                    barra_act += velocidadLlenadoBarra * multiplicadorProximidad * modificadorSigilo * Time.deltaTime;
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
                    barra_act -= velocidadLlenadoBarra * 0.7f * Time.deltaTime;
                }
                else
                {
                    barra_act = 0f;
                }

                PatrullarPuntos();
            }
        }

        ActualizarAnimaciones();
    }

    private void ActualizarBarraDeSospecha()
    {
        if (img == null) return;

        img.fillAmount = barra_act / barra_tot;

        if (caza)
        {
            img.color = Mathf.PingPong(Time.time * 8f, 1f) > 0.5f ? Color.red : new Color(0.3f, 0f, 0f);
        }
        else if (barra_act >= barra_tot * 0.70f)
        {
            img.color = Color.red; 
        }
        else if (barra_act >= barra_tot * 0.35f)
        {
            img.color = new Color(1f, 0.5f, 0f); 
        }
        else
        {
            img.color = Color.green; 
        }
    }

    private void ActualizarAnimaciones()
    {
        if (animador != null)
        {
            animador.SetFloat("velocidad", rb2d.linearVelocity.magnitude);
            animador.SetBool("caza", caza);
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
        if (estaEsperando)
        {
            rb2d.linearVelocity = Vector2.zero;
            temporizadorEspera -= Time.deltaTime;
            if (temporizadorEspera <= 0f)
            {
                estaEsperando = false;
                puntoObjetivoActual = (puntoObjetivoActual == B) ? A : B;
            }
            return;
        }

        direccion = (puntoObjetivoActual.position - transform.position).normalized;
        rb2d.linearVelocity = direccion * speed;

        if (Vector2.Distance(transform.position, puntoObjetivoActual.position) < distanciaMinimaPunto)
        {
            estaEsperando = true;
            temporizadorEspera = tiempoDeEsperaEnPunto;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            float distanciaReal = Vector2.Distance(transform.position, cuy.transform.position);
            
            if (distanciaReal <= 1.3f)
            {
                if (caza || cuy.estadoOculto)
                {
                    caza = false;
                    barra_act = 0f;
                    puntoObjetivoActual = B;
                    direccion = (B.position - transform.position).normalized;
                    cuy.estadoActual = ControladorCuy.EstadoCuy.Agotado;
                    
                    Debug.Log("¡Atrapado! El zorro te comió.");
                    cuy.Morir();
                }
                else
                {
                    barra_act = barra_tot;
                    caza = true;
                }
            }
        }
    }
}
