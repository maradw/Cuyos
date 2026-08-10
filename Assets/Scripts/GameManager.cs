using Game.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    
    public int vidasMaximas = 5;
    public int vidasActuales = 5;

    
    
    public Transform puntoRespawn;

    
    
    public Sprite spriteHudTablita;

    private ControladorCuy cuyJugador;
    private Vector3 posicionInicialCuy;

    private Canvas canvasUI;
    private Image imagenNegraFade;
    private Text textoVidasFade;
    private Text textoHUDVidas;

    [SerializeField] MusicData BgMusic;

    private bool procesandoMuerte = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Font fuenteDaydream = Resources.Load<Font>("Fonts/Daydream DEMO");
        if (fuenteDaydream != null)
        {
            Text[] todosLosTextos = Resources.FindObjectsOfTypeAll<Text>();
            foreach (Text t in todosLosTextos)
            {
                t.font = fuenteDaydream;
            }
        }

        vidasActuales = vidasMaximas;
        
        if (spriteHudTablita == null)
        {
            spriteHudTablita = Resources.Load<Sprite>("UI_TablitaVidas");
        }

        /* AudioSource musicaFondo = gameObject.AddComponent<AudioSource>();
         musicaFondo.clip = Resources.Load<AudioClip>("musicanivel");
         if (musicaFondo.clip != null)
         {
             musicaFondo.loop = true;
             musicaFondo.volume = 0.35f;
             musicaFondo.Play();
         }*/
    }

    private void Start()
    {
        if (MusicManager.Instance != null && BgMusic != null)
            MusicManager.Instance.PlayBG(BgMusic);
        BuscarJugadorYGuardarPosicion();
        CrearElementosUIDinamicos();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuscarJugadorYGuardarPosicion();
        CrearElementosUIDinamicos();
        ActualizarHUDVidas();
    }

    private void BuscarJugadorYGuardarPosicion()
    {
        cuyJugador = Object.FindAnyObjectByType<ControladorCuy>();
        if (cuyJugador != null)
        {
            posicionInicialCuy = cuyJugador.transform.position;
        }
        procesandoMuerte = false;
    }

    private void CrearElementosUIDinamicos()
    {
        if (canvasUI != null)
        {
            Destroy(canvasUI.gameObject);
        }

        GameObject goCanvas = new GameObject("GameManager_CanvasUI");
        DontDestroyOnLoad(goCanvas);

        canvasUI = goCanvas.AddComponent<Canvas>();
        canvasUI.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasUI.sortingOrder = 999;

        CanvasScaler scaler = goCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        goCanvas.AddComponent<GraphicRaycaster>();

        GameObject goFade = new GameObject("ImageFade");
        goFade.transform.SetParent(canvasUI.transform, false);

        imagenNegraFade = goFade.AddComponent<Image>();
        imagenNegraFade.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rtFade = imagenNegraFade.rectTransform;
        rtFade.anchorMin = Vector2.zero;
        rtFade.anchorMax = Vector2.one;
        rtFade.sizeDelta = Vector2.zero;

        Font fuenteDefault = Resources.Load<Font>("Fonts/Daydream DEMO");
        if (fuenteDefault == null) fuenteDefault = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (fuenteDefault == null) fuenteDefault = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (fuenteDefault == null) fuenteDefault = Font.CreateDynamicFontFromOSFont("Arial", 14);

        GameObject goTextFade = new GameObject("TextFadeVidas");
        goTextFade.transform.SetParent(goFade.transform, false);

        textoVidasFade = goTextFade.AddComponent<Text>();
        textoVidasFade.font = fuenteDefault;
        textoVidasFade.fontSize = 65;
        textoVidasFade.alignment = TextAnchor.MiddleCenter;
        textoVidasFade.color = new Color(1f, 1f, 1f, 0f);
        textoVidasFade.text = "Vidas Restantes: 5";

        RectTransform rtTextFade = textoVidasFade.rectTransform;
        rtTextFade.anchorMin = Vector2.zero;
        rtTextFade.anchorMax = Vector2.one;
        rtTextFade.sizeDelta = Vector2.zero;

        GameObject goHUD = new GameObject("HUD_Vidas");
        goHUD.transform.SetParent(canvasUI.transform, false);

        RectTransform rtHUD = goHUD.AddComponent<RectTransform>();
        rtHUD.anchorMin = new Vector2(0f, 1f);
        rtHUD.anchorMax = new Vector2(0f, 1f);
        rtHUD.pivot = new Vector2(0f, 1f);
        rtHUD.anchoredPosition = new Vector3(30f, -30f, 0f);

        if (spriteHudTablita != null)
        {
            Image imgTablita = goHUD.AddComponent<Image>();
            imgTablita.sprite = spriteHudTablita;
            imgTablita.color = Color.white;
            imgTablita.preserveAspect = false;

            rtHUD.sizeDelta = new Vector2(280f, 140f);

            GameObject goHUDText = new GameObject("HUD_Text");
            goHUDText.transform.SetParent(goHUD.transform, false);

            textoHUDVidas = goHUDText.AddComponent<Text>();
            textoHUDVidas.font = fuenteDefault;
            textoHUDVidas.fontSize = 42;
            textoHUDVidas.fontStyle = FontStyle.Bold;
            textoHUDVidas.alignment = TextAnchor.MiddleCenter;
            textoHUDVidas.color = Color.white;

            Shadow shadow = goHUDText.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
            shadow.effectDistance = new Vector2(3f, -3f);

            RectTransform rtHUDText = textoHUDVidas.rectTransform;
            rtHUDText.anchorMin = new Vector2(0.52f, 0f);
            rtHUDText.anchorMax = new Vector2(1f, 1f);
            rtHUDText.pivot = new Vector2(0.5f, 0.5f);
            rtHUDText.anchoredPosition = Vector2.zero;
            rtHUDText.sizeDelta = Vector2.zero;
        }
        else
        {
            textoHUDVidas = goHUD.AddComponent<Text>();
            textoHUDVidas.font = fuenteDefault;
            textoHUDVidas.fontSize = 36;
            textoHUDVidas.alignment = TextAnchor.UpperLeft;
            textoHUDVidas.color = Color.white;

            Shadow shadow = goHUD.AddComponent<Shadow>();
            shadow.effectColor = Color.black;
            shadow.effectDistance = new Vector2(2f, -2f);

            rtHUD.sizeDelta = new Vector2(400f, 100f);
        }

        ActualizarHUDVidas();
    }

    public void ActualizarHUDVidas()
    {
        if (textoHUDVidas != null)
        {
            if (spriteHudTablita != null)
            {
                textoHUDVidas.text = $"x{vidasActuales}";
            }
            else
            {
                textoHUDVidas.text = $"Vidas: {vidasActuales} / {vidasMaximas}";
            }
        }
    }

    public void ProcesarMuerteJugador()
    {
        if (procesandoMuerte) return;
        procesandoMuerte = true;

        if (CamaraDinamica.Instance != null)
        {
            CamaraDinamica.Instance.ActivarTemblor(0.8f, 0.4f);
        }

        vidasActuales--;
        ActualizarHUDVidas();

        if (vidasActuales > 0)
        {
            StartCoroutine(RutinaRespawn());
        }
        else
        {
            StartCoroutine(RutinaGameOver());
        }
    }

    private IEnumerator RutinaRespawn()
    {
        if (cuyJugador != null)
        {
            cuyJugador.estadoActual = ControladorCuy.EstadoCuy.Agotado;
            cuyJugador.entradaMovimiento = Vector2.zero;
            Rigidbody2D rb = cuyJugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        float t = 0f;
        if (textoVidasFade != null)
        {
            textoVidasFade.text = "¡Cui, cuiii... cuidado!";
        }

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (imagenNegraFade != null) imagenNegraFade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (textoVidasFade != null) textoVidasFade.color = new Color(1f, 1f, 1f, Mathf.Clamp01(t));
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            if (textoVidasFade != null) textoVidasFade.color = new Color(1f, 1f, 1f, 1f - Mathf.Clamp01(t));
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return null; 

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (imagenNegraFade != null) imagenNegraFade.color = new Color(0f, 0f, 0f, 1f - Mathf.Clamp01(t));
            yield return null;
        }

        procesandoMuerte = false;
    }

    private IEnumerator RutinaGameOver()
    {
        if (cuyJugador != null)
        {
            cuyJugador.estadoActual = ControladorCuy.EstadoCuy.Agotado;
            cuyJugador.entradaMovimiento = Vector2.zero;
            Rigidbody2D rb = cuyJugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            if (imagenNegraFade != null) imagenNegraFade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t));
            yield return null;
        }

        vidasActuales = vidasMaximas;

        if (PantallaEventos.Instance != null)
        {
            if (imagenNegraFade != null) imagenNegraFade.color = Color.clear;
            yield return StartCoroutine(PantallaEventos.Instance.MostrarGameOver());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
