using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SeguimientoCamara : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    [Tooltip("El transform del cuy que la cámara debe seguir.")]
    public Transform objetivoASeguir;

    [Header("Ajustes de Suavizado")]
    [Tooltip("Tiempo de respuesta de la cámara para alcanzar al cuy (menor tiempo = cámara más rápida).")]
    [Range(0.01f, 1f)]
    public float tiempoDeSuavizado = 0.2f;

    [Tooltip("Distancia de separación de la cámara respecto al cuy (eje Z debe ser negativo).")]
    public Vector3 desplazamientoCamara = new Vector3(0, 0, -10f);

    [Header("Límites del Escenario")]
    [Tooltip("¿La cámara debe detenerse en los bordes del escenario para no mostrar zonas vacías?")]
    public bool delimitarBordes = false;
    public float limiteIzquierdo, limiteDerecho;
    public float limiteInferior, limiteSuperior;

    private Vector3 velocidadReferenciaInterna = Vector3.zero;

    private void Start()
    {

        if (objetivoASeguir == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                objetivoASeguir = jugador.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (objetivoASeguir == null) return;

        Vector3 posicionDestino = objetivoASeguir.position + desplazamientoCamara;

        if (delimitarBordes)
        {
            float xLimitado = Mathf.Clamp(posicionDestino.x, limiteIzquierdo, limiteDerecho);
            float yLimitado = Mathf.Clamp(posicionDestino.y, limiteInferior, limiteSuperior);
            posicionDestino = new Vector3(xLimitado, yLimitado, posicionDestino.z);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            posicionDestino, 
            ref velocidadReferenciaInterna, 
            tiempoDeSuavizado
        );
    }
}
