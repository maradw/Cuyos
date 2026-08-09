using UnityEngine;
using System.Collections;

public class CascaraPlatano : MonoBehaviour
{
    private bool activo = true;

    public void LanzarHacia(Vector2 destino)
    {
        activo = false; 
        StartCoroutine(RutinaLanzamiento(destino));
    }

    private void Update()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
        }
    }

    private IEnumerator RutinaLanzamiento(Vector2 destino)
    {
        Vector2 origen = transform.position;
        float duracion = 0.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;
            
            Vector2 posLineal = Vector2.Lerp(origen, destino, t);
            float altura = Mathf.Sin(t * Mathf.PI) * 1.5f; 
            
            transform.position = posLineal + new Vector2(0, altura);
            transform.Rotate(0, 0, 720f * Time.deltaTime); 
            
            yield return null;
        }

        transform.position = destino;
        activo = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activo) return; 

        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            cuy.ResbalarConPlatano();
            
            MonoTiti[] monos = FindObjectsByType<MonoTiti>(FindObjectsSortMode.None);
            foreach (MonoTiti mono in monos)
            {
                mono.IniciarBurla();
            }
            
            Destroy(gameObject);
        }
    }
}
