using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SistemaEstressCuy : MonoBehaviour
{
    public static SistemaEstressCuy Instance { get; private set; }

    private ControladorCuy cuyJugador;
    private Image imagenCorazonHUD;
    private Image imagenVignetaEstress;
    private float nivelEstress = 0f;
    private float distanciaDeteccion = 6f;
    private float velocidadLatido = 1f;
    private float escalaBase = 1f;
    private bool inicializado = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        cuyJugador = FindAnyObjectByType<ControladorCuy>();
        StartCoroutine(InicializarConRetraso());
    }

    private IEnumerator InicializarConRetraso()
    {
        yield return new WaitForSeconds(0.5f);
        BuscarCorazonEnHUD();
        CrearVignetaEstress();
        inicializado = true;
    }

    private void BuscarCorazonEnHUD()
    {
        Image[] todasLasImagenes = FindObjectsByType<Image>(FindObjectsSortMode.None);
        foreach (Image img in todasLasImagenes)
        {
            if (img.gameObject.name.ToLower().Contains("corazon") || img.gameObject.name.ToLower().Contains("heart") || img.gameObject.name.ToLower().Contains("vida"))
            {
                imagenCorazonHUD = img;
                escalaBase = img.transform.localScale.x;
                break;
            }
        }
    }

    private void CrearVignetaEstress()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject goVigneta = new GameObject("VignetaEstress");
        goVigneta.transform.SetParent(canvas.transform, false);
        goVigneta.transform.SetAsFirstSibling();

        imagenVignetaEstress = goVigneta.AddComponent<Image>();
        imagenVignetaEstress.color = new Color(0.6f, 0f, 0f, 0f);

        Texture2D texVigneta = new Texture2D(64, 64);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = (x - 32f) / 32f;
                float dy = (y - 32f) / 32f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(dist - 0.3f);
                texVigneta.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texVigneta.Apply();
        imagenVignetaEstress.sprite = Sprite.Create(texVigneta, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        imagenVignetaEstress.type = Image.Type.Simple;
        imagenVignetaEstress.raycastTarget = false;

        RectTransform rt = imagenVignetaEstress.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (!inicializado || cuyJugador == null) return;

        float distanciaMinima = ObtenerDistanciaEnemigoCercano();
        float nivelObjetivo = 0f;

        if (distanciaMinima < distanciaDeteccion)
        {
            nivelObjetivo = 1f - (distanciaMinima / distanciaDeteccion);
            nivelObjetivo = Mathf.Clamp01(nivelObjetivo);
        }

        nivelEstress = Mathf.Lerp(nivelEstress, nivelObjetivo, Time.deltaTime * 2f);

        velocidadLatido = Mathf.Lerp(1f, 4.5f, nivelEstress);
        float latido = 1f + Mathf.Abs(Mathf.Sin(Time.time * velocidadLatido * Mathf.PI)) * 0.35f * nivelEstress;

        if (imagenCorazonHUD != null)
        {
            imagenCorazonHUD.transform.localScale = Vector3.one * escalaBase * latido;
            Color colorCorazon = Color.Lerp(Color.white, new Color(1f, 0.2f, 0.2f, 1f), nivelEstress);
            imagenCorazonHUD.color = colorCorazon;
        }

        if (imagenVignetaEstress != null)
        {
            float alphaVigneta = nivelEstress * 0.55f * Mathf.Abs(Mathf.Sin(Time.time * velocidadLatido * Mathf.PI * 0.5f));
            imagenVignetaEstress.color = new Color(0.7f, 0f, 0f, alphaVigneta);
        }
    }

    private float ObtenerDistanciaEnemigoCercano()
    {
        float distanciaMenor = float.MaxValue;

        Collider2D[] cercanos = Physics2D.OverlapCircleAll(cuyJugador.transform.position, distanciaDeteccion);
        foreach (Collider2D col in cercanos)
        {
            if (col.GetComponent<Condor>() != null ||
                col.GetComponent<zorro_code>() != null ||
                col.GetComponent<MonoTiti>() != null ||
                col.GetComponent<controller_chinchilla>() != null)
            {
                float d = Vector2.Distance(cuyJugador.transform.position, col.transform.position);
                if (d < distanciaMenor) distanciaMenor = d;
            }
        }

        return distanciaMenor;
    }
}
