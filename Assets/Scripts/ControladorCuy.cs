using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
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

    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad máxima a la que puede correr el cuy.")]
    public float velocidadMaxima = 6f;

    [Tooltip("Fuerza con la que el cuy acelera para alcanzar la velocidad máxima.")]
    public float fuerzaAceleracion = 50f;

    [Tooltip("Fuerza de frenado cuando se dejan de pulsar las teclas de dirección.")]
    public float fuerzaDesaceleracion = 40f;

    [Header("Rotación y Orientación")]
    [Tooltip("¿El cuy debe girar físicamente hacia la dirección de su movimiento?")]
    public bool rotarHaciaDireccion = true;
    
    [Tooltip("Velocidad con la que el cuy rota hacia su nueva dirección de marcha.")]
    public float velocidadDeGiro = 10f;

    [Header("Estado General")]
    public EstadoCuy estadoActual = EstadoCuy.Quieto;

    [Header("Mochila de Insumos")]
    [Tooltip("Lista de insumos que lleva el cuy actualmente en su espalda.")]
    public List<TipoInsumo> mochilaInsumos = new List<TipoInsumo>();
    
    [Tooltip("Cantidad máxima de insumos que puede cargar al mismo tiempo.")]
    public int capacidadMochila = 3;

    private Rigidbody2D cuerpoFisico;
    private Animator componenteAnimador;
    private SpriteRenderer renderizadorSprite;

    private Vector2 entradaMovimiento;
    private Vector2 velocidadObjetivo;
    private Vector2 velocidadActual;
    private Vector2 ultimaDireccionMirada = Vector2.down;

    private void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        componenteAnimador = GetComponent<Animator>();
        renderizadorSprite = GetComponent<SpriteRenderer>();

        cuerpoFisico.gravityScale = 0f;
        cuerpoFisico.freezeRotation = true;
        cuerpoFisico.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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

        if (entradaMovimiento.magnitude > 0.1f)
        {
            ultimaDireccionMirada = entradaMovimiento;
            estadoActual = (mochilaInsumos.Count > 0) ? EstadoCuy.CargandoComida : EstadoCuy.Caminando;
        }
        else if (estadoActual != EstadoCuy.Oculto)
        {
            estadoActual = (mochilaInsumos.Count > 0) ? EstadoCuy.CargandoComida : EstadoCuy.Quieto;
        }

        GirarHaciaMovimiento();

        ActualizarAnimador();
    }

    private void FixedUpdate()
    {

        velocidadObjetivo = entradaMovimiento * velocidadMaxima;

        float tasaCambioVelocidad = (entradaMovimiento.magnitude > 0.01f) ? fuerzaAceleracion : fuerzaDesaceleracion;

        velocidadActual = Vector2.MoveTowards(cuerpoFisico.linearVelocity, velocidadObjetivo, tasaCambioVelocidad * Time.fixedDeltaTime);
        cuerpoFisico.linearVelocity = velocidadActual;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {

        Insumo insumoDelSuelo = collision.GetComponent<Insumo>();
        if (insumoDelSuelo != null)
        {
            if (mochilaInsumos.Count < capacidadMochila)
            {
                mochilaInsumos.Add(insumoDelSuelo.tipoDeInsumo);
                Debug.Log($"Recogiste: {insumoDelSuelo.nombreInsumo}. Mochila: {mochilaInsumos.Count}/{capacidadMochila}");
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("¡Mochila llena! Debes ir a entregar los insumos al saco.");
            }
            return;
        }

        if (collision.CompareTag("SacoEntrega") && mochilaInsumos.Count > 0)
        {
            GestorReceta gestor = collision.GetComponent<GestorReceta>();
            if (gestor != null)
            {
                Debug.Log("Entregando insumos en el saco...");
                
                for (int i = mochilaInsumos.Count - 1; i >= 0; i--)
                {
                    bool aceptado = gestor.EntregarInsumoEnSaco(mochilaInsumos[i]);
                    if (aceptado)
                    {
                        mochilaInsumos.RemoveAt(i);
                    }
                }
            }
        }
    }

    public void CambiarEstadoOculto(bool estaOculto)
    {
        if (estadoActual == EstadoCuy.Agotado) return;

        estadoActual = estaOculto ? EstadoCuy.Oculto : EstadoCuy.Quieto;

        if (renderizadorSprite != null)
        {
            Color colorSprite = renderizadorSprite.color;
            colorSprite.a = estaOculto ? 0.5f : 1f;
            renderizadorSprite.color = colorSprite;
        }
    }
}
