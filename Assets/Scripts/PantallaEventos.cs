using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PantallaEventos : MonoBehaviour
{
    public static PantallaEventos Instance { get; private set; }

    public Sprite spriteCuyTriste;
    public int numeroNoche = 1;

    private Canvas canvas;
    private Font fuenteDaydream;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        fuenteDaydream = Resources.Load<Font>("Fonts/Daydream DEMO");
        if (fuenteDaydream == null)
            fuenteDaydream = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CrearCanvas();
        StartCoroutine(MostrarIndicadorNoche());
    }

    private void CrearCanvas()
    {
        GameObject goCanvas = new GameObject("CanvasEventos");
        canvas = goCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = goCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        goCanvas.AddComponent<GraphicRaycaster>();
    }

    public IEnumerator MostrarIndicadorNoche()
    {
        yield return new WaitForSeconds(0.3f);

        GameObject goFondo = new GameObject("FondoNoche");
        goFondo.transform.SetParent(canvas.transform, false);
        Image imgFondo = goFondo.AddComponent<Image>();
        imgFondo.color = new Color(0f, 0f, 0f, 0f);
        RectTransform rtFondo = imgFondo.rectTransform;
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.sizeDelta = Vector2.zero;

        GameObject goBarra = new GameObject("BarraHorizontal");
        goBarra.transform.SetParent(canvas.transform, false);
        Image imgBarra = goBarra.AddComponent<Image>();
        imgBarra.color = new Color(0f, 0f, 0f, 0f);
        RectTransform rtBarra = imgBarra.rectTransform;
        rtBarra.anchorMin = new Vector2(0f, 0.35f);
        rtBarra.anchorMax = new Vector2(1f, 0.65f);
        rtBarra.sizeDelta = Vector2.zero;

        GameObject goTextoNoche = new GameObject("TextoNoche");
        goTextoNoche.transform.SetParent(canvas.transform, false);
        Text textoNoche = goTextoNoche.AddComponent<Text>();
        textoNoche.font = fuenteDaydream;
        textoNoche.fontSize = 90;
        textoNoche.fontStyle = FontStyle.Bold;
        textoNoche.alignment = TextAnchor.MiddleCenter;
        textoNoche.color = new Color(0.7f, 1f, 0.4f, 0f);
        textoNoche.text = "NOCHE  " + numeroNoche;

        Shadow sombra = goTextoNoche.AddComponent<Shadow>();
        sombra.effectColor = new Color(0.1f, 0.4f, 0.1f, 1f);
        sombra.effectDistance = new Vector2(4f, -4f);

        RectTransform rtTexto = textoNoche.rectTransform;
        rtTexto.anchorMin = Vector2.zero;
        rtTexto.anchorMax = Vector2.one;
        rtTexto.sizeDelta = Vector2.zero;
        rtTexto.anchoredPosition = Vector2.zero;

        GameObject goSubtexto = new GameObject("SubtextoNoche");
        goSubtexto.transform.SetParent(canvas.transform, false);
        Text subtexto = goSubtexto.AddComponent<Text>();
        subtexto.font = fuenteDaydream;
        subtexto.fontSize = 28;
        subtexto.alignment = TextAnchor.MiddleCenter;
        subtexto.color = new Color(0.9f, 0.7f, 0.3f, 0f);
        subtexto.text = "- sobrevive y completa la receta -";

        RectTransform rtSub = subtexto.rectTransform;
        rtSub.anchorMin = new Vector2(0f, 0.35f);
        rtSub.anchorMax = new Vector2(1f, 0.55f);
        rtSub.sizeDelta = Vector2.zero;
        rtSub.anchoredPosition = new Vector2(0f, -70f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            float a = Mathf.Clamp01(t);
            imgFondo.color = new Color(0f, 0f, 0f, a * 0.88f);
            imgBarra.color = new Color(0.05f, 0.15f, 0.03f, a * 0.9f);
            textoNoche.color = new Color(0.7f, 1f, 0.4f, a);
            subtexto.color = new Color(0.9f, 0.7f, 0.3f, a * 0.85f);
            yield return null;
        }

        yield return new WaitForSeconds(1.8f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            float a = 1f - Mathf.Clamp01(t);
            imgFondo.color = new Color(0f, 0f, 0f, a * 0.88f);
            imgBarra.color = new Color(0.05f, 0.15f, 0.03f, a * 0.9f);
            textoNoche.color = new Color(0.7f, 1f, 0.4f, a);
            subtexto.color = new Color(0.9f, 0.7f, 0.3f, a * 0.85f);
            yield return null;
        }

        Destroy(goFondo);
        Destroy(goBarra);
        Destroy(goTextoNoche);
        Destroy(goSubtexto);
    }

    public IEnumerator MostrarGameOver()
    {
        GameObject goFondo = new GameObject("FondoGameOver");
        goFondo.transform.SetParent(canvas.transform, false);
        Image imgFondo = goFondo.AddComponent<Image>();
        imgFondo.color = new Color(0f, 0f, 0f, 0f);
        RectTransform rtFondo = imgFondo.rectTransform;
        rtFondo.anchorMin = Vector2.zero;
        rtFondo.anchorMax = Vector2.one;
        rtFondo.sizeDelta = Vector2.zero;

        GameObject goTitulo = new GameObject("TituloGameOver");
        goTitulo.transform.SetParent(canvas.transform, false);
        Text textoTitulo = goTitulo.AddComponent<Text>();
        textoTitulo.font = fuenteDaydream;
        textoTitulo.fontSize = 100;
        textoTitulo.fontStyle = FontStyle.Bold;
        textoTitulo.alignment = TextAnchor.MiddleCenter;
        textoTitulo.color = new Color(1f, 0.1f, 0.1f, 0f);
        textoTitulo.text = "GAME  OVER";

        Shadow sombraTitulo = goTitulo.AddComponent<Shadow>();
        sombraTitulo.effectColor = new Color(0.5f, 0f, 0f, 1f);
        sombraTitulo.effectDistance = new Vector2(5f, -5f);

        RectTransform rtTitulo = textoTitulo.rectTransform;
        rtTitulo.anchorMin = Vector2.zero;
        rtTitulo.anchorMax = Vector2.one;
        rtTitulo.sizeDelta = Vector2.zero;
        rtTitulo.anchoredPosition = new Vector2(0f, 80f);

        GameObject goSubtitulo = new GameObject("SubtituloGameOver");
        goSubtitulo.transform.SetParent(canvas.transform, false);
        Text textoSub = goSubtitulo.AddComponent<Text>();
        textoSub.font = fuenteDaydream;
        textoSub.fontSize = 26;
        textoSub.alignment = TextAnchor.MiddleCenter;
        textoSub.color = new Color(0.9f, 0.9f, 0.9f, 0f);
        textoSub.text = "el condor se llevo al cuy...";

        RectTransform rtSub = textoSub.rectTransform;
        rtSub.anchorMin = Vector2.zero;
        rtSub.anchorMax = Vector2.one;
        rtSub.sizeDelta = Vector2.zero;
        rtSub.anchoredPosition = new Vector2(0f, -30f);

        GameObject goHint = new GameObject("HintGameOver");
        goHint.transform.SetParent(canvas.transform, false);
        Text textoHint = goHint.AddComponent<Text>();
        textoHint.font = fuenteDaydream;
        textoHint.fontSize = 20;
        textoHint.alignment = TextAnchor.MiddleCenter;
        textoHint.color = new Color(0.6f, 0.6f, 0.6f, 0f);
        textoHint.text = "[ presiona cualquier tecla ]";

        RectTransform rtHint = textoHint.rectTransform;
        rtHint.anchorMin = Vector2.zero;
        rtHint.anchorMax = Vector2.one;
        rtHint.sizeDelta = Vector2.zero;
        rtHint.anchoredPosition = new Vector2(0f, -130f);

        if (spriteCuyTriste != null)
        {
            GameObject goSprite = new GameObject("CuyTriste");
            goSprite.transform.SetParent(canvas.transform, false);
            Image imgCuy = goSprite.AddComponent<Image>();
            imgCuy.sprite = spriteCuyTriste;
            imgCuy.color = new Color(1f, 1f, 1f, 0f);
            imgCuy.preserveAspect = true;
            RectTransform rtCuy = imgCuy.rectTransform;
            rtCuy.anchorMin = new Vector2(0.5f, 0.5f);
            rtCuy.anchorMax = new Vector2(0.5f, 0.5f);
            rtCuy.pivot = new Vector2(0.5f, 0.5f);
            rtCuy.sizeDelta = new Vector2(180f, 180f);
            rtCuy.anchoredPosition = new Vector2(0f, -220f);

            float tSprite = 0f;
            while (tSprite < 1f)
            {
                tSprite += Time.deltaTime * 1.2f;
                imgCuy.color = new Color(1f, 1f, 1f, Mathf.Clamp01(tSprite));
                yield return null;
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.2f;
            float a = Mathf.Clamp01(t);
            imgFondo.color = new Color(0.05f, 0f, 0f, a * 0.95f);
            textoTitulo.color = new Color(1f, 0.1f, 0.1f, a);
            textoSub.color = new Color(0.9f, 0.9f, 0.9f, a * 0.8f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            textoHint.color = new Color(0.6f, 0.6f, 0.6f, Mathf.Abs(Mathf.Sin(Time.time * 2f)));
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                break;
            yield return null;
        }

        while (true)
        {
            textoHint.color = new Color(0.6f, 0.6f, 0.6f, Mathf.Abs(Mathf.Sin(Time.time * 2f)));
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                break;
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            float a = 1f - Mathf.Clamp01(t);
            imgFondo.color = new Color(0.05f, 0f, 0f, Mathf.Lerp(0.95f, 1f, Mathf.Clamp01(t)));
            textoTitulo.color = new Color(1f, 0.1f, 0.1f, a);
            textoSub.color = new Color(0.9f, 0.9f, 0.9f, a);
            textoHint.color = new Color(0.6f, 0.6f, 0.6f, a);
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
