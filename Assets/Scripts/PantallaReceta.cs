using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PantallaReceta : MonoBehaviour
{
    public static PantallaReceta Instance { get; private set; }

    public Sprite spriteRecetario;
    private Texture2D texturaPergamino;
    private Font fuenteDaydream;

    private bool visible = false;
    private GestorReceta gestorActual;
    private ControladorCuy cuyJugador;

    private GUIStyle estiloTitulo;
    private GUIStyle estiloItem;
    private GUIStyle estiloItemOk;
    private GUIStyle estiloSub;
    private GUIStyle estiloHint;
    private Texture2D texVerde;
    private Texture2D texFondo;
    private bool estilosCreados = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        cuyJugador   = FindAnyObjectByType<ControladorCuy>();
        gestorActual = FindAnyObjectByType<GestorReceta>();
        fuenteDaydream = Resources.Load<Font>("Fonts/Daydream DEMO");
        if (spriteRecetario != null)
            texturaPergamino = spriteRecetario.texture;
    }

    private void Update()
    {
        if (gestorActual == null) gestorActual = FindAnyObjectByType<GestorReceta>();
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            visible = !visible;
    }

    private Texture2D MkTex(Color c) { var t = new Texture2D(1,1); t.SetPixel(0,0,c); t.Apply(); return t; }

    private void InicializarEstilos()
    {
        texVerde = MkTex(new Color(0.1f, 0.45f, 0.1f, 0.5f));
        texFondo = MkTex(new Color(0.08f, 0.18f, 0.05f, 0.97f));

        int fs  = Mathf.Max(14, Mathf.RoundToInt(Screen.height * 0.022f));
        int fsT = Mathf.Max(18, Mathf.RoundToInt(Screen.height * 0.028f));

        estiloTitulo = new GUIStyle(GUI.skin.label)
            { font = fuenteDaydream, fontSize = fsT, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        estiloTitulo.normal.textColor = new Color(0.22f, 0.10f, 0.01f);

        estiloItem = new GUIStyle(GUI.skin.label)
            { font = fuenteDaydream, fontSize = fs, wordWrap = false };
        estiloItem.normal.textColor = new Color(0.18f, 0.08f, 0.01f);

        estiloItemOk = new GUIStyle(estiloItem);
        estiloItemOk.normal.textColor = new Color(0.05f, 0.32f, 0.05f);
        estiloItemOk.normal.background = texVerde;

        estiloSub = new GUIStyle(estiloTitulo) { fontSize = fs };
        estiloSub.normal.textColor = new Color(0.28f, 0.13f, 0.01f);

        estiloHint = new GUIStyle(estiloTitulo) { fontSize = Mathf.Max(11, fs - 3) };
        estiloHint.normal.textColor = new Color(0.30f, 0.20f, 0.08f, 0.7f);

        estilosCreados = true;
    }

    private void OnGUI()
    {
        if (!visible) return;
        if (!estilosCreados) InicializarEstilos();

        List<GestorReceta.RequisitoInsumo> reqs =
            gestorActual != null ? gestorActual.recetaRequisitos
                                 : new List<GestorReceta.RequisitoInsumo>();

        Dictionary<TipoInsumo, int> mochila = new Dictionary<TipoInsumo, int>();
        if (cuyJugador != null)
            foreach (var ins in cuyJugador.mochilaInsumos)
            { if (mochila.ContainsKey(ins)) mochila[ins]++; else mochila[ins] = 1; }

        bool tieneFoto    = gestorActual != null && gestorActual.requiereFragmentoFoto;
        bool tieneMochila = mochila.Count > 0;
        int  filas        = reqs.Count + (tieneFoto?1:0) + (tieneMochila ? mochila.Count+1 : 0);

        float lh   = Screen.height * 0.056f;
        float panW = Screen.width  * 0.88f;
        float panH = lh * 1.8f + filas * (lh + 6f) + lh * 1.6f + (tieneMochila ? lh * 1.4f : 0f);
        panH = Mathf.Min(panH, Screen.height * 0.88f);

        float px = (Screen.width  - panW) * 0.5f;
        float py = (Screen.height - panH) * 0.5f;

        if (texturaPergamino != null)
            GUI.DrawTexture(new Rect(px, py, panW, panH), texturaPergamino, ScaleMode.StretchToFill);
        else
            GUI.DrawTexture(new Rect(px, py, panW, panH), texFondo);

        float tx    = Screen.width * 0.30f;
        float tw    = Screen.width * 0.40f;
        float cy    = py + lh * 0.5f;

        GUI.Label(new Rect(tx, cy, tw, lh * 1.5f), "== RECETA ==", estiloTitulo);
        cy += lh * 1.6f;

        foreach (var req in reqs)
        {
            bool ok = req.cantidadEntregada >= req.cantidadNecesaria;
            string txt = (ok ? "v " : "o ") + req.tipo.ToString() + "  " + req.cantidadEntregada + "/" + req.cantidadNecesaria;
            GUI.Label(new Rect(tx, cy, tw, lh), txt, ok ? estiloItemOk : estiloItem);
            cy += lh + 6f;
        }

        if (tieneFoto)
        {
            bool ok = false;
            if (cuyJugador != null)
                foreach (var ins in cuyJugador.mochilaInsumos)
                    if (ins == TipoInsumo.FragmentoDeFoto) { ok = true; break; }
            string txt = (ok ? "v " : "o ") + "Foto  " + (ok ? "1" : "0") + "/1";
            GUI.Label(new Rect(tx, cy, tw, lh), txt, ok ? estiloItemOk : estiloItem);
            cy += lh + 6f;
        }

        if (tieneMochila)
        {
            cy += lh * 0.4f;
            GUI.Label(new Rect(tx, cy, tw, lh), "-- MOCHILA --", estiloSub);
            cy += lh + 2f;
            foreach (var kvp in mochila)
            {
                GUI.Label(new Rect(tx, cy, tw, lh), "* " + kvp.Key.ToString() + " x" + kvp.Value, estiloItem);
                cy += lh + 4f;
            }
        }

        GUI.Label(new Rect(tx, py + panH - lh * 1.2f, tw, lh), "[ TAB ] cerrar", estiloHint);
    }
}
