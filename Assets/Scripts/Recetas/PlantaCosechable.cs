using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlantaCosechable : MonoBehaviour
{
    public GameObject prefabInsumo; 
    public int pulsacionesRequeridas = 5; 

    [Header("Sprites Personalizados (Opcional)")]
    public Sprite spriteBarra; 
    public Sprite spriteParticula; 
    [Tooltip("Sprite personalizado para el botón E de interacción")]
    public Sprite spriteBotonInteractuar; 

    private int pulsacionesActuales = 0;
    private bool jugadorEnRango = false;
    private Vector3 escalaOriginal;

    private GameObject barraProgresoFondo;
    private GameObject barraProgresoRelleno;
    private GameObject indicadorTexto;

    private Sprite spriteBlancoGenerico;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        escalaOriginal = transform.localScale;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        spriteBlancoGenerico = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    void Update()
    {
        if (jugadorEnRango)
        {
            if (indicadorTexto != null)
            {
                float oscilacionE = Mathf.Sin(Time.time * 6f) * 0.06f;
                indicadorTexto.transform.position = transform.position + new Vector3(0f, 1.55f + oscilacionE, 0f);
            }

            bool presionoInteractuar = false;
            if (Keyboard.current != null)
            {
                presionoInteractuar = Keyboard.current.eKey.wasPressedThisFrame;
            }

            if (presionoInteractuar)
            {
                CosecharPaso();
            }
        }
    }

    private void CosecharPaso()
    {
        pulsacionesActuales++;
        
        float factorCrecimiento = 1f + ((float)pulsacionesActuales / pulsacionesRequeridas * 0.25f);
        transform.localScale = new Vector3(
            escalaOriginal.x * factorCrecimiento, 
            escalaOriginal.y * (factorCrecimiento - 0.1f), 
            escalaOriginal.z
        );

        if (barraProgresoRelleno != null)
        {
            float porcentaje = (float)pulsacionesActuales / pulsacionesRequeridas;
            barraProgresoRelleno.transform.localScale = new Vector3(porcentaje * 1.2f, 0.15f, 1f);
            barraProgresoRelleno.transform.position = transform.position + new Vector3(-0.6f + (porcentaje * 0.6f), 1.0f, 0f);
        }

        GenerarParticulasCosecha();

        if (pulsacionesActuales >= pulsacionesRequeridas)
        {
            CompletarCosecha();
        }
    }

    private void CompletarCosecha()
    {
        DestruirIndicadores();

        if (prefabInsumo != null)
        {
            GameObject insumoInstanciado = Instantiate(prefabInsumo, transform.position, Quaternion.identity);
            
            Vector3 escalaOriginalPrefab = insumoInstanciado.transform.localScale;
            insumoInstanciado.transform.localScale = Vector3.zero;
            
            Insumo scriptInsumo = insumoInstanciado.GetComponent<Insumo>();
            if (scriptInsumo != null)
            {
                scriptInsumo.StartCoroutine(EfectoPopInsumo(insumoInstanciado, escalaOriginalPrefab));
            }
            else
            {
                insumoInstanciado.transform.localScale = escalaOriginalPrefab;
            }
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator EfectoPopInsumo(GameObject objeto, Vector3 escalaObjetivo)
    {
        float t = 0f;
        if (escalaObjetivo == Vector3.zero) escalaObjetivo = new Vector3(1.5f, 1.5f, 1f);

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            
            try
            {
                if (objeto == null || objeto.Equals(null))
                {
                    yield break;
                }
                objeto.transform.localScale = Vector3.Lerp(Vector3.zero, escalaObjetivo, t);
            }
            catch (System.Exception)
            {
                yield break;
            }
            
            yield return null;
        }
        
        try
        {
            if (objeto != null && !objeto.Equals(null))
            {
                objeto.transform.localScale = escalaObjetivo;
            }
        }
        catch (System.Exception)
        {
        }
    }

    private void CrearIndicadores()
    {
        if (indicadorTexto != null) return;

        indicadorTexto = new GameObject("IndicadorTexto");
        indicadorTexto.transform.position = transform.position + new Vector3(0f, 1.55f, 0f);

        if (spriteBotonInteractuar != null)
        {
            SpriteRenderer srBoton = indicadorTexto.AddComponent<SpriteRenderer>();
            srBoton.sprite = spriteBotonInteractuar;
            srBoton.sortingOrder = 6;
            indicadorTexto.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            TextMesh textMesh = indicadorTexto.AddComponent<TextMesh>();
            textMesh.text = "[E]";
            textMesh.fontSize = 28;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;
        }

        barraProgresoFondo = new GameObject("BarraFondo");
        barraProgresoFondo.transform.position = transform.position + new Vector3(0f, 1.0f, 0f);
        barraProgresoFondo.transform.localScale = new Vector3(1.2f, 0.15f, 1f);

        SpriteRenderer srFondo = barraProgresoFondo.AddComponent<SpriteRenderer>();
        srFondo.sprite = (spriteBarra != null) ? spriteBarra : spriteBlancoGenerico;
        srFondo.color = new Color(0.15f, 0.15f, 0.15f, 0.85f); 
        srFondo.sortingOrder = 4;

        barraProgresoRelleno = new GameObject("BarraRelleno");
        barraProgresoRelleno.transform.position = transform.position + new Vector3(-0.6f, 1.0f, 0f); 
        barraProgresoRelleno.transform.localScale = new Vector3(0f, 0.15f, 1f); 

        SpriteRenderer srRelleno = barraProgresoRelleno.AddComponent<SpriteRenderer>();
        srRelleno.sprite = (spriteBarra != null) ? spriteBarra : spriteBlancoGenerico;
        srRelleno.color = new Color(0f, 0.8f, 0.2f, 1f); 
        srRelleno.sortingOrder = 5; 
    }

    private void DestruirIndicadores()
    {
        if (indicadorTexto != null) Destroy(indicadorTexto);
        if (barraProgresoFondo != null) Destroy(barraProgresoFondo);
        if (barraProgresoRelleno != null) Destroy(barraProgresoRelleno);

        indicadorTexto = null;
        barraProgresoFondo = null;
        barraProgresoRelleno = null;
    }

    private void GenerarParticulasCosecha()
    {
        int cantidad = Random.Range(3, 6);
        for (int i = 0; i < cantidad; i++)
        {
            GameObject particula = new GameObject("ParticulaCosecha");
            particula.transform.position = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.2f, 0f);
            particula.transform.localScale = new Vector3(0.08f, 0.08f, 1f);

            SpriteRenderer sr = particula.AddComponent<SpriteRenderer>();
            sr.sprite = (spriteParticula != null) ? spriteParticula : spriteBlancoGenerico;
            sr.sortingOrder = 6;

            if (Random.value > 0.5f)
            {
                sr.color = new Color(0.2f, 0.8f, 0.1f, 1f); 
            }
            else
            {
                sr.color = new Color(0.5f, 0.3f, 0.1f, 1f); 
            }

            particula.AddComponent<EfectoParticulaCosecha>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            jugadorEnRango = true;
            CrearIndicadores();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            jugadorEnRango = false;
            pulsacionesActuales = 0;
            transform.localScale = escalaOriginal;
            DestruirIndicadores();
        }
    }

    private void OnDestroy()
    {
        DestruirIndicadores();
    }
}

public class EfectoParticulaCosecha : MonoBehaviour
{
    private Vector2 velocidad;
    private float tiempoVida = 0.5f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        velocidad = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 5f));
    }

    void Update()
    {
        velocidad.y -= 9.8f * Time.deltaTime;
        transform.Translate(velocidad * Time.deltaTime);

        tiempoVida -= Time.deltaTime;
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Clamp01(tiempoVida / 0.5f);
            sr.color = c;
        }

        if (tiempoVida <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
