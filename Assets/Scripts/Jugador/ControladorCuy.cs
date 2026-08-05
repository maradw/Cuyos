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
        Oculto
    }

    [Header("Movimiento")]
    public float velocidadMaxima = 6f;
    public float fuerzaAceleracion = 50f;
    public float fuerzaDesaceleracion = 40f;

    [Header("Rotacion")]
    public bool rotarHaciaDireccion = true;
    public float velocidadDeGiro = 10f;

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
        cuerpoFisico.freezeRotation = true;
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

        bool presionandoShift = false;
        if (Keyboard.current != null)
        {
            presionandoShift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        }

        if (presionandoShift)
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

        ActualizarEfectoVisualSigilo(presionandoShift);
        GirarHaciaMovimiento();
        ActualizarAnimador();
    }

    private void FixedUpdate()
    {
        float velocidadActualMaxima = velocidadMaxima;

        if (estadoActual == EstadoCuy.Oculto)
        {
            velocidadActualMaxima = velocidadMaxima * 0.5f;
        }
        else if (estaRalentizado)
        {
            velocidadActualMaxima = velocidadMaxima * 0.4f;
        }

        velocidadObjetivo = entradaMovimiento * velocidadActualMaxima;
        float tasaCambioVelocidad = (entradaMovimiento.magnitude > 0.01f) ? fuerzaAceleracion : fuerzaDesaceleracion;
        velocidadActual = Vector2.MoveTowards(cuerpoFisico.velocity, velocidadObjetivo, tasaCambioVelocidad * Time.fixedDeltaTime);
        cuerpoFisico.velocity = velocidadActual;
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

    private void GirarHaciaMovimiento()
    {
        if (entradaMovimiento.magnitude > 0.1f)
        {
            if (rotarHaciaDireccion)
            {
                float anguloDestino = Mathf.Atan2(entradaMovimiento.y, entradaMovimiento.x) * Mathf.Rad2Deg - 90f;
                float anguloSuave = Mathf.LerpAngle(transform.eulerAngles.z, anguloDestino, velocidadDeGiro * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, anguloSuave);
            }
            else if (renderizadorSprite != null)
            {
                if (entradaMovimiento.x < -0.1f) renderizadorSprite.flipX = true;
                else if (entradaMovimiento.x > 0.1f) renderizadorSprite.flipX = false;
            }
        }
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

    public void CambiarEstadoOculto(bool estaOculto)
    {
        if (estadoActual == EstadoCuy.Agotado) return;

        estadoActual = estaOculto ? EstadoCuy.Oculto : EstadoCuy.Quieto;
    }
}
