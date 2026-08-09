using UnityEngine;

public class Condor : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 direccion;
    public GameObject sombra_guia;
    public Transform sombra;
    public Rigidbody2D rb2d;
    private Animator animador;

    
    public float alturaBase = 5f;
    public float frecuenciaVuelo = 2f;
    public float amplitudVuelo = 2.5f;

    
    public float velocidadDescenso = 10f;
    public float velocidadAscenso = 12f; 

    private Vector3 escalaBaseSombra;
    private Vector3 posicionInicialAve;
    private SpriteRenderer sombraSR;
    
    private bool enPicada = false;
    private bool subiendoConCuy = false;
    private GameObject cuyObjetivo;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>(); 
        sombra_guia = transform.parent.gameObject;
        
        if (sombra == null && transform.parent != null)
        {
            foreach (Transform hijo in transform.parent)
            {
                if (hijo != transform && (hijo.name.ToLower().Contains("sombra") || hijo.name.ToLower().Contains("shadow") || hijo.GetComponent<controller_condor>() != null))
                {
                    sombra = hijo;
                    break;
                }
            }
        }

        direccion = (sombra_guia.transform.position - transform.position).normalized;
        
        if (direccion == Vector2.zero)
        {
            direccion = Vector2.right;
        }

        if (sombra != null)
        {
            escalaBaseSombra = sombra.localScale;
            
            sombraSR = sombra.GetComponent<SpriteRenderer>();
            if (sombraSR != null)
            {
                sombraSR.color = new Color(0f, 0f, 0f, 0.4f); 
                sombraSR.sortingOrder = 1; 
            }
        }
        else
        {
            escalaBaseSombra = new Vector3(1.5f, 1.5f, 1f);
        }

        posicionInicialAve = transform.localPosition;
    }

    void Update()
    {
        if (subiendoConCuy)
        {
            rb2d.linearVelocity = Vector2.zero; 
            transform.position += Vector3.up * velocidadAscenso * Time.deltaTime;
        }
        else
        {
            rb2d.linearVelocity = direccion * speed;
        }

        if (enPicada)
        {
            Vector3 destinoPicada = new Vector3(transform.position.x, sombra.position.y + 0.3f, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, destinoPicada, velocidadDescenso * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - (sombra.position.y + 0.3f)) < 0.2f)
            {
                enPicada = false;
                subiendoConCuy = true;
                
                AudioClip clipPajaro = Resources.Load<AudioClip>("pajaro");
                if (clipPajaro != null)
                {
                    GameObject objSonido = new GameObject("SonidoPajaro");
                    AudioSource src = objSonido.AddComponent<AudioSource>();
                    src.clip = clipPajaro;
                    src.volume = 0.8f;
                    src.Play();
                    Destroy(objSonido, clipPajaro.length);
                }
                
                if (cuyObjetivo != null)
                {
                    ControladorCuy scriptCuy = cuyObjetivo.GetComponent<ControladorCuy>();
                    if (scriptCuy != null)
                    {
                        scriptCuy.SoltarInsumosPorGolpe();
                    }

                    cuyObjetivo.transform.SetParent(transform);
                    cuyObjetivo.transform.localPosition = new Vector3(0f, -0.4f, 0f);
                    cuyObjetivo.transform.localRotation = Quaternion.identity;

                    StartCoroutine(RutinaMatarCuy(cuyObjetivo, 1.5f));

                    SeguimientoCamara camara = Object.FindAnyObjectByType<SeguimientoCamara>();
                    if (camara != null)
                    {
                        camara.SacudirCamara(0.35f, 0.25f);
                    }
                }
            }
        }
        else if (!subiendoConCuy)
        {
            float oscilacion = Mathf.Sin(Time.time * frecuenciaVuelo) * amplitudVuelo;
            transform.localPosition = new Vector3(posicionInicialAve.x, alturaBase + oscilacion, posicionInicialAve.z);
        }

        if (!subiendoConCuy)
        {
            float anguloVuelo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, anguloVuelo);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        if (sombra != null)
        {
            sombra.transform.position = new Vector3(transform.position.x, sombra.transform.position.y, sombra.transform.position.z);
            
            float dist = transform.position.y - sombra.transform.position.y;
            float factorEscala = 1.5f / (dist * 0.15f + 0.4f);
            sombra.localScale = escalaBaseSombra * Mathf.Clamp(factorEscala, 0.4f, 2.5f);

            if (sombraSR != null)
            {
                sombraSR.color = new Color(0f, 0f, 0f, Mathf.Clamp(factorEscala * 0.2f, 0f, 0.4f));
            }
        }

        ActualizarAnimaciones();
    }

    private System.Collections.IEnumerator RutinaMatarCuy(GameObject cuy, float retraso)
    {
        yield return new WaitForSeconds(retraso);
        if (cuy != null)
        {
            ControladorCuy scriptCuy = cuy.GetComponent<ControladorCuy>();
            if (scriptCuy != null)
            {
                scriptCuy.Morir();
            }
        }
        subiendoConCuy = false;
    }

    private void ActualizarAnimaciones()
    {
        if (animador != null)
        {
            animador.SetBool("enpicada", enPicada);
            animador.SetBool("subiendo", subiendoConCuy);
        }
    }

    public void IniciarPicada(GameObject cuy)
    {
        enPicada = true;
        cuyObjetivo = cuy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Sombra"))
        {
            change_direction();
        }
    }

    public void change_direction()
    {
        direccion = new Vector2(direccion.x * -1f, direccion.y);
    }

    private void OnBecameInvisible()
    {
    }
}
