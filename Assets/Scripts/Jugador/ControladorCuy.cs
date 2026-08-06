using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ControladorCuy : MonoBehaviour
{
    public enum EstadoCuy
    {
        Quieto,
        Caminando,
        CargandoComida,
        Agotado,
        Oculto,
        Deslizando 
    }

    [Header("Movimiento Base")]
    public float velocidadMaxima = 6f;
    public float fuerzaAceleracion = 50f;
    public float fuerzaDesaceleracion = 40f;

    [Header("Peso por Carga")]
    public float penalizacionVelocidadPorInsumo = 0.04f;

    [Header("Impulso de Escape (Dash)")]
    [Tooltip("Permite al cuy dar un breve impulso de velocidad al presionar Espacio")]
    public bool permitirImpulso = true;
    public float multiplicadorImpulso = 1.8f;
    public float duracionImpulso = 0.25f;
    public float cooldownImpulso = 3f;
    [HideInInspector] public bool estaDasheando = false;
    private float temporizadorImpulso = 0f;
    private float temporizadorCooldown = 0f;

    [Header("Mecánica de Charco (Empapado)")]
    [HideInInspector] public bool estaEmpapado = false;
    [HideInInspector] public float temporizadorEmpapado = 0f;
    private float temporizadorGotitas = 0f;
    private Sprite spriteGotaGenerica;
    
    private bool estaSacudiendose = false;
    private float temporizadorSacudida = 0f;
    private float temporizadorBurstGotas = 0f;

    [Header("Rotacion")]
    public bool rotarHaciaDireccion = true;
    public float velocidadDeGiro = 10f;

    [Header("Wobble de Caminata (Visual)")]
    public bool activarBamboleo = true;
    public float velocidadBamboleo = 12f; 
    public float inclinacionBamboleo = 5f; 

    [Header("Estado")]
    public EstadoCuy estadoActual = EstadoCuy.Quieto;
    public Transform puntoDeCarga;
    
    [Header("Distribucion en V")]
    public float espacioHorizontal = 0.3f;
    public float espacioVertical = 0.35f;
    public int capacidadMochila = 5;

    [HideInInspector] public Vector2 entradaMovimiento;
    
    public bool estadoOculto
    {
        get => estadoActual == EstadoCuy.Oculto;
        set => CambiarEstadoOculto(value);
    }

    private List<GameObject> mochilaVisual = new List<GameObject>();
    [HideInInspector] public List<TipoInsumo> mochilaInsumos = new List<TipoInsumo>();
    private Dictionary<GameObject, int> capasOriginalesInsumos = new Dictionary<GameObject, int>();

    private Rigidbody2D cuerpoFisico;
    private Animator componenteAnimador;
    private SpriteRenderer renderizadorSprite;

    private Vector2 velocidadObjetivo;
    private Vector2 velocidadActual;
    private Vector2 ultimaDireccionMirada = Vector2.down;
    
    private bool estaRalentizado = false;
    private float tiempoEsperaRecogida = 0f;

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        componenteAnimador = GetComponent<Animator>();
        renderizadorSprite = GetComponent<SpriteRenderer>();

        cuerpoFisico.bodyType = RigidbodyType2D.Dynamic;
        cuerpoFisico.gravityScale = 0f;
        cuerpoFisico.freezeRotation = false;
        cuerpoFisico.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (capacidadMochila <= 0)
        {
            capacidadMochila = 5;
        }

        if (puntoDeCarga == null)
        {
            GameObject nuevoPunto = new GameObject("PuntoCarga");
            nuevoPunto.transform.SetParent(transform);
            nuevoPunto.transform.localPosition = new Vector3(0, 0.2f, 0);
            puntoDeCarga = nuevoPunto.transform;
        }

        if (renderizadorSprite != null)
        {
            renderizadorSprite.sortingOrder = 5;
        }

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        spriteGotaGenerica = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    private void Update()
    {
        if (estadoActual == EstadoCuy.Agotado)
        {
            entradaMovimiento = Vector2.zero;
            return;
        }

        ObtenerEntradasNuevoSistema();

        if (entradaMovimiento.magnitude > 1f)
        {
            entradaMovimiento.Normalize();
        }

        if (tiempoEsperaRecogida > 0)
        {
            tiempoEsperaRecogida -= Time.deltaTime;
        }

        if (temporizadorCooldown > 0)
        {
            temporizadorCooldown -= Time.deltaTime;
        }

        if (estaDasheando)
        {
            temporizadorImpulso -= Time.deltaTime;
            if (temporizadorImpulso <= 0f)
            {
                estaDasheando = false;
            }
        }

        bool quiereDashear = false;
        if (Keyboard.current != null)
        {
            quiereDashear = Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        if (quiereDashear && permitirImpulso && temporizadorCooldown <= 0f && entradaMovimiento.magnitude > 0.1f)
        {
            estaDasheando = true;
            temporizadorImpulso = duracionImpulso;
            temporizadorCooldown = cooldownImpulso;
            estadoActual = EstadoCuy.Deslizando;
        }

        bool presionandoShift = false;
        if (Keyboard.current != null && !estaDasheando)
        {
            presionandoShift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        }

        if (estaDasheando)
        {
            estadoActual = EstadoCuy.Deslizando;
        }
        else if (presionandoShift)
        {
            estadoActual = EstadoCuy.Oculto;
        }
        else if (entradaMovimiento.magnitude > 0.1f)
        {
            ultimaDireccionMirada = entradaMovimiento;
            estadoActual = (mochilaInsumos.Count > 0) ? EstadoCuy.CargandoComida : EstadoCuy.Caminando;
        }
        else
        {
            estadoActual = (mochilaInsumos.Count > 0) ? EstadoCuy.CargandoComida : EstadoCuy.Quieto;
        }

        if (estaEmpapado)
        {
            if (estaSacudiendose)
            {
                temporizadorSacudida -= Time.deltaTime;

                temporizadorBurstGotas -= Time.deltaTime;
                if (temporizadorBurstGotas <= 0f)
                {
                    temporizadorBurstGotas = 0.06f;
                    EmitirBurstGotas();
                }

                if (temporizadorSacudida <= 0f)
                {
                    estaSacudiendose = false;
                    estaEmpapado = false;
                }
            }
            else
            {
                temporizadorEmpapado -= Time.deltaTime;
                
                if (temporizadorEmpapado <= 0.6f && temporizadorEmpapado > 0f)
                {
                    estaSacudiendose = true;
                    temporizadorSacudida = 0.6f;
                    temporizadorBurstGotas = 0f;
                }

                EmitirGotitaAgua();
            }
        }

        ActualizarEfectoVisualSigilo(presionandoShift);
        ActualizarAnimador();
    }

    private void FixedUpdate()
    {
        float velocidadActualMaxima = velocidadMaxima;

        if (estaDasheando)
        {
            velocidadActualMaxima *= multiplicadorImpulso;
            velocidadObjetivo = entradaMovimiento * velocidadActualMaxima;
            cuerpoFisico.linearVelocity = velocidadObjetivo;
        }
        else
        {
            float multiplicadorDeCarga = 1f - (mochilaInsumos.Count * penalizacionVelocidadPorInsumo);
            velocidadActualMaxima *= Mathf.Clamp(multiplicadorDeCarga, 0.5f, 1f);

            if (estadoActual == EstadoCuy.Oculto)
            {
                velocidadActualMaxima *= 0.5f;
            }
            else if (estaRalentizado)
            {
                velocidadActualMaxima *= 0.4f;
            }

            if (estaEmpapado)
            {
                if (estaSacudiendose)
                {
                    velocidadActualMaxima *= 0.15f; 
                }
                else
                {
                    velocidadActualMaxima *= 0.5f; 
                }
            }

            velocidadObjetivo = entradaMovimiento * velocidadActualMaxima;
            float tasaCambioVelocidad = (entradaMovimiento.magnitude > 0.01f) ? fuerzaAceleracion : fuerzaDesaceleracion;
            velocidadActual = Vector2.MoveTowards(cuerpoFisico.linearVelocity, velocidadObjetivo, tasaCambioVelocidad * Time.fixedDeltaTime);
            cuerpoFisico.linearVelocity = velocidadActual;
        }

        GirarHaciaMovimientoFisico();
    }

    private void ObtenerEntradasNuevoSistema()
    {
        float entradaX = 0f;
        float entradaY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) entradaY = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) entradaY = -1f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) entradaX = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) entradaX = 1f;
        }

        if (Gamepad.current != null)
        {
            Vector2 stickIzquierdo = Gamepad.current.leftStick.ReadValue();
            if (stickIzquierdo.magnitude > 0.1f)
            {
                entradaX = stickIzquierdo.x;
                entradaY = stickIzquierdo.y;
            }
        }

        entradaMovimiento = new Vector2(entradaX, entradaY);
    }

    private void ActualizarEfectoVisualSigilo(bool estaOculto)
    {
        if (renderizadorSprite != null)
        {
            Color colorSprite = renderizadorSprite.color;
            if (estaOculto)
            {
                colorSprite.r = 0.6f;
                colorSprite.g = 0.6f;
                colorSprite.b = 0.6f;
                colorSprite.a = 0.8f;
            }
            else if (estaDasheando)
            {
                colorSprite = Color.white;
            }
            else if (estaEmpapado)
            {
                colorSprite.r = 0.6f;
                colorSprite.g = 0.8f;
                colorSprite.b = 1.0f;
                colorSprite.a = 1.0f;
            }
            else
            {
                colorSprite.r = 1f;
                colorSprite.g = 1f;
                colorSprite.b = 1f;
                colorSprite.a = 1f;
            }
            renderizadorSprite.color = colorSprite;
        }
    }

    private void EmitirGotitaAgua()
    {
        if (cuerpoFisico.linearVelocity.magnitude < 0.2f) return;

        temporizadorGotitas -= Time.deltaTime;
        if (temporizadorGotitas <= 0f)
        {
            temporizadorGotitas = 0.15f; 

            GameObject gota = new GameObject("GotitaAgua");
            gota.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), -0.2f, 0f);
            gota.transform.localScale = new Vector3(0.08f, 0.08f, 1f);

            SpriteRenderer sr = gota.AddComponent<SpriteRenderer>();
            sr.sprite = spriteGotaGenerica;
            sr.color = new Color(0.2f, 0.6f, 1.0f, 0.8f); 
            sr.sortingOrder = sortingOrderPlayer() - 1; 

            gota.AddComponent<EfectoGotitaAgua>();
        }
    }

    private void EmitirBurstGotas()
    {
        int gotas = Random.Range(5, 9);
        for (int i = 0; i < gotas; i++)
        {
            GameObject gota = new GameObject("GotitaSacudida");
            gota.transform.position = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), -0.1f, 0f);
            gota.transform.localScale = new Vector3(0.07f, 0.07f, 1f);

            SpriteRenderer sr = gota.AddComponent<SpriteRenderer>();
            sr.sprite = spriteGotaGenerica;
            sr.color = new Color(0.3f, 0.7f, 1.0f, 0.9f);
            sr.sortingOrder = sortingOrderPlayer() + 1; 

            EfectoGotitaAgua scriptGota = gota.AddComponent<EfectoGotitaAgua>();
            Vector2 dirAleatoria = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.5f)).normalized;
            scriptGota.EstablecerVelocidadBurst(dirAleatoria * Random.Range(2.0f, 3.5f));
        }
    }

    private int sortingOrderPlayer()
    {
        if (renderizadorSprite != null) return renderizadorSprite.sortingOrder;
        return 5;
    }

    private void GirarHaciaMovimientoFisico()
    {
        float anguloDestino = cuerpoFisico.rotation;

        if (entradaMovimiento.magnitude > 0.1f && !estaSacudiendose)
        {
            if (rotarHaciaDireccion)
            {
                anguloDestino = Mathf.Atan2(entradaMovimiento.y, entradaMovimiento.x) * Mathf.Rad2Deg - 90f;
            }
            else if (renderizadorSprite != null)
            {
                if (entradaMovimiento.x < -0.1f) renderizadorSprite.flipX = true;
                else if (entradaMovimiento.x > 0.1f) renderizadorSprite.flipX = false;
            }
        }

        float bamboleo = 0f;
        if (estaSacudiendose)
        {
            bamboleo = Mathf.Sin(Time.time * 20f) * 12f;
        }
        else if (activarBamboleo && cuerpoFisico.linearVelocity.magnitude > 0.2f && !estaDasheando)
        {
            float modVelocidadBamboleo = (estadoActual == EstadoCuy.Oculto) ? 0.6f : 1f;
            bamboleo = Mathf.Sin(Time.time * velocidadBamboleo * modVelocidadBamboleo) * inclinacionBamboleo;
        }

        float anguloSuave = Mathf.LerpAngle(cuerpoFisico.rotation, anguloDestino, velocidadDeGiro * Time.fixedDeltaTime);
        cuerpoFisico.MoveRotation(anguloSuave + bamboleo);
    }

    private void ActualizarAnimador()
    {
        if (componenteAnimador == null) return;

        componenteAnimador.SetFloat("VelocidadActual", velocidadActual.magnitude);
        componenteAnimador.SetFloat("DireccionX", entradaMovimiento.x);
        componenteAnimador.SetFloat("DireccionY", entradaMovimiento.y);
        
        componenteAnimador.SetFloat("UltimaDireccionX", ultimaDireccionMirada.x);
        componenteAnimador.SetFloat("UltimaDireccionY", ultimaDireccionMirada.y);

        componenteAnimador.SetBool("Cargando", mochilaInsumos.Count > 0);
        componenteAnimador.SetBool("Agotado", estadoActual == EstadoCuy.Agotado);
        componenteAnimador.SetBool("Oculto", estadoActual == EstadoCuy.Oculto);
        componenteAnimador.SetBool("Deslizando", estadoActual == EstadoCuy.Deslizando);
    }

    public void SoltarInsumosPorGolpe()
    {
        if (estaRalentizado) return;

        StartCoroutine(RutinaRalentizar());
        
        tiempoEsperaRecogida = 1.5f;

        if (mochilaVisual.Count == 0) return;

        Debug.Log("Colision: Soltando insumos");

        for (int i = mochilaVisual.Count - 1; i >= 0; i--)
        {
            GameObject insumoObj = mochilaVisual[i];
            insumoObj.transform.SetParent(null);
            
            SpriteRenderer insumoSR = insumoObj.GetComponent<SpriteRenderer>();
            if (insumoSR != null && capasOriginalesInsumos.ContainsKey(insumoObj))
            {
                insumoSR.sortingOrder = capasOriginalesInsumos[insumoObj];
                capasOriginalesInsumos.Remove(insumoObj);
            }

            Collider2D col = insumoObj.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
            }
            
            Vector3 dispersion = new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-1.2f, 1.2f), 0);
            insumoObj.transform.position = transform.position + dispersion;
            insumoObj.transform.rotation = Quaternion.identity;
        }

        mochilaInsumos.Clear();
        mochilaVisual.Clear();
    }

    private IEnumerator RutinaRalentizar()
    {
        estaRalentizado = true;
        yield return new WaitForSeconds(2f);
        estaRalentizado = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Colision con: {collision.gameObject.name}");

        Insumo insumoDelSuelo = collision.GetComponent<Insumo>();
        if (insumoDelSuelo != null)
        {
            if (tiempoEsperaRecogida <= 0f && mochilaInsumos.Count < capacidadMochila)
            {
                mochilaInsumos.Add(insumoDelSuelo.tipoDeInsumo);
                collision.enabled = false;
                collision.gameObject.transform.SetParent(puntoDeCarga);
                
                SpriteRenderer insumoSR = collision.GetComponent<SpriteRenderer>();
                if (insumoSR != null && renderizadorSprite != null)
                {
                    if (!capasOriginalesInsumos.ContainsKey(collision.gameObject))
                    {
                        capasOriginalesInsumos.Add(collision.gameObject, insumoSR.sortingOrder);
                    }
                    insumoSR.sortingOrder = renderizadorSprite.sortingOrder + 1;
                }

                Vector3 posicionV = CalcularPosicionV(mochilaVisual.Count);
                collision.gameObject.transform.localPosition = posicionV;
                
                if (rotarHaciaDireccion)
                {
                    collision.gameObject.transform.rotation = Quaternion.identity;
                }
                
                mochilaVisual.Add(collision.gameObject);
            }
            return;
        }

        GestorReceta gestor = collision.GetComponent<GestorReceta>();
        if (gestor != null && mochilaInsumos.Count > 0)
        {
            for (int i = mochilaInsumos.Count - 1; i >= 0; i--)
            {
                bool aceptado = gestor.EntregarInsumoEnSaco(mochilaInsumos[i]);
                if (aceptado)
                {
                    mochilaInsumos.RemoveAt(i);
                    if (capasOriginalesInsumos.ContainsKey(mochilaVisual[i]))
                    {
                        capasOriginalesInsumos.Remove(mochilaVisual[i]);
                    }
                    Destroy(mochilaVisual[i]);
                    mochilaVisual.RemoveAt(i);
                }
            }
            ReordenarMochilaVisual();
        }
    }

    private Vector3 CalcularPosicionV(int indice)
    {
        if (indice == 0) return Vector3.zero;

        float lado = (indice % 2 == 1) ? -1f : 1f;
        int multiplicador = (indice + 1) / 2;
        
        float x = lado * espacioHorizontal * multiplicador;
        float y = espacioVertical * multiplicador;
        
        return new Vector3(x, y, 0);
    }

    private void ReordenarMochilaVisual()
    {
        for (int i = 0; i < mochilaVisual.Count; i++)
        {
            if (mochilaVisual[i] != null)
            {
                mochilaVisual[i].transform.localPosition = CalcularPosicionV(i);
            }
        }
    }

    private void LateUpdate()
    {
        if (rotarHaciaDireccion && mochilaVisual.Count > 0)
        {
            foreach (GameObject insumo in mochilaVisual)
            {
                if (insumo != null)
                {
                    insumo.transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    public void Morir()
    {
        if (estadoActual == EstadoCuy.Agotado) return;

        estadoActual = EstadoCuy.Agotado;
        cuerpoFisico.linearVelocity = Vector2.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcesarMuerteJugador();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarEstadoOculto(bool estaOculto)
    {
        if (estadoActual == EstadoCuy.Agotado) return;

        estadoActual = estaOculto ? EstadoCuy.Oculto : EstadoCuy.Quieto;
    }
}

public class EfectoGotitaAgua : MonoBehaviour
{
    private float tiempoVida = 0.4f;
    private float velocidadCaida = 1.5f;
    private SpriteRenderer sr;
    private Vector2 velocidadBurst;
    private bool esBurst = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void EstablecerVelocidadBurst(Vector2 vel)
    {
        velocidadBurst = vel;
        esBurst = true;
        tiempoVida = 0.35f; 
    }

    void Update()
    {
        if (esBurst)
        {
            velocidadBurst.y -= 9.8f * Time.deltaTime;
            transform.Translate(velocidadBurst * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime);
        }

        tiempoVida -= Time.deltaTime;
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Clamp01(tiempoVida / (esBurst ? 0.35f : 0.4f)) * 0.8f;
            sr.color = c;
        }

        if (tiempoVida <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
