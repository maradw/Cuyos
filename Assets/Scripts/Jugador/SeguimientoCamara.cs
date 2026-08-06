using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    public enum MomentoActualizacion
    {
        Update,
        FixedUpdate,
        LateUpdate
    }

    public Transform objetivoASeguir;
    
    [Tooltip("Elige cuándo debe moverse la cámara. Si el cuy tiembla, prueba cambiar esto en el Inspector en tiempo real.")]
    public MomentoActualizacion momentoDeSeguimiento = MomentoActualizacion.LateUpdate;

    [Range(0f, 1f)] public float tiempoDeSuavizado = 0.1f;
    public Vector3 desplazamientoCamara = new Vector3(0, 0, -10f);

    public bool delimitarBordes = false;
    public float limiteIzquierdo, limiteDerecho;
    public float limiteInferior, limiteSuperior;

    private Vector3 velocidadReferenciaInterna = Vector3.zero;
    
    private float duracionSacudida = 0f;
    private float magnitudSacudida = 0f;
    private Vector3 offsetSacudida = Vector3.zero;

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

    private void Update()
    {
        if (duracionSacudida > 0f)
        {
            offsetSacudida = Random.insideUnitSphere * magnitudSacudida;
            offsetSacudida.z = 0f; 
            duracionSacudida -= Time.deltaTime;
        }
        else
        {
            offsetSacudida = Vector3.zero;
        }

        if (momentoDeSeguimiento == MomentoActualizacion.Update)
        {
            EjecutarSeguimiento();
        }
    }

    private void FixedUpdate()
    {
        if (momentoDeSeguimiento == MomentoActualizacion.FixedUpdate)
        {
            EjecutarSeguimiento();
        }
    }

    private void LateUpdate()
    {
        if (momentoDeSeguimiento == MomentoActualizacion.LateUpdate)
        {
            EjecutarSeguimiento();
        }
    }

    private void EjecutarSeguimiento()
    {
        if (objetivoASeguir == null) return;

        try
        {
            Vector3 posicionDestino = objetivoASeguir.position + desplazamientoCamara;

            if (delimitarBordes)
            {
                float xLimitado = Mathf.Clamp(posicionDestino.x, limiteIzquierdo, limiteDerecho);
                float yLimitado = Mathf.Clamp(posicionDestino.y, limiteInferior, limiteSuperior);
                posicionDestino = new Vector3(xLimitado, yLimitado, posicionDestino.z);
            }

            Vector3 posicionSuave = Vector3.SmoothDamp(
                transform.position, 
                posicionDestino, 
                ref velocidadReferenciaInterna, 
                tiempoDeSuavizado
            );

            transform.position = posicionSuave + offsetSacudida;
        }
        catch (System.Exception)
        {
        }
    }

    public void SacudirCamara(float duracion, float magnitud)
    {
        duracionSacudida = duracion;
        magnitudSacudida = magnitud;
    }
}
