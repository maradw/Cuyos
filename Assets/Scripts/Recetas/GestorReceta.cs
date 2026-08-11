using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestorReceta : MonoBehaviour
{
    [System.Serializable]
    public struct RequisitoInsumo
    {
        public TipoInsumo tipo;
        public int cantidadNecesaria;
        public int cantidadEntregada;
    }

    public string nombrePlato = "Papa a la Huancaína";
    public List<RequisitoInsumo> recetaRequisitos = new List<RequisitoInsumo>();
    public bool requiereFragmentoFoto = true;
    public string escenaSiguiente = "";
    public bool fotoEntregada = false;

    private List<GameObject> itemsEnSaco = new List<GameObject>();

    private void Start()
    {
        if (recetaRequisitos.Count == 0)
            ConfigurarRecetaDefecto();

        if (string.IsNullOrEmpty(escenaSiguiente))
        {
            string escenaActual = SceneManager.GetActiveScene().name;
            if (escenaActual == "escena1_tiles")
            {
                escenaSiguiente = "escena2_official";
            }
            else if (escenaActual == "escena2_official")
            {
                escenaSiguiente = "final";
            }
            else
            {
                int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    string path = SceneUtility.GetScenePathByBuildIndex(nextIndex);
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                    escenaSiguiente = sceneName;
                }
            }
        }
    }

    private void ConfigurarRecetaDefecto()
    {
        recetaRequisitos.Add(new RequisitoInsumo { tipo = TipoInsumo.PapaAmarilla, cantidadNecesaria = 3 });
        recetaRequisitos.Add(new RequisitoInsumo { tipo = TipoInsumo.AjiAmarillo, cantidadNecesaria = 2 });
        recetaRequisitos.Add(new RequisitoInsumo { tipo = TipoInsumo.QuesoFresco, cantidadNecesaria = 1 });
        recetaRequisitos.Add(new RequisitoInsumo { tipo = TipoInsumo.GalletaDeSoda, cantidadNecesaria = 1 });
        recetaRequisitos.Add(new RequisitoInsumo { tipo = TipoInsumo.AceitunaBotija, cantidadNecesaria = 1 });
    }

    public bool EntregarInsumoEnSaco(TipoInsumo tipoEntregado)
    {
        if (tipoEntregado == TipoInsumo.ItemTrampa)
        {
            for (int i = 0; i < recetaRequisitos.Count; i++)
            {
                RequisitoInsumo r = recetaRequisitos[i];
                r.cantidadEntregada = r.cantidadNecesaria;
                recetaRequisitos[i] = r;
            }
            fotoEntregada = true;
            ComprobarVictoriaReceta();
            return true;
        }

        if (tipoEntregado == TipoInsumo.FragmentoDeFoto)
        {
            fotoEntregada = true;
            ComprobarVictoriaReceta();
            return true;
        }

        for (int i = 0; i < recetaRequisitos.Count; i++)
        {
            RequisitoInsumo requisito = recetaRequisitos[i];
            if (requisito.tipo == tipoEntregado)
            {
                if (requisito.cantidadEntregada < requisito.cantidadNecesaria)
                {
                    requisito.cantidadEntregada++;
                    recetaRequisitos[i] = requisito;
                    ComprobarVictoriaReceta();
                    return true;
                }
            }
        }
        return false;
    }

    public void AgregarItemVisual(GameObject item)
    {
        if (item == null) return;

        Collider2D col = item.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float offsetRandomX = Random.Range(-0.1f, 0.1f);
        float offsetRandomY = Random.Range(-0.05f, 0.05f);
        float alturaDePila = itemsEnSaco.Count * 0.12f;

        item.transform.position = transform.position + new Vector3(offsetRandomX, 0.1f + alturaDePila + offsetRandomY, 0f);
        item.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-15f, 15f));
        item.transform.localScale = new Vector3(0.12f, 0.12f, 1f);

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 5 + itemsEnSaco.Count;

        itemsEnSaco.Add(item);
    }

    private void ComprobarVictoriaReceta()
    {
        foreach (var requisito in recetaRequisitos)
        {
            if (requisito.cantidadEntregada < requisito.cantidadNecesaria)
                return;
        }

        if (requiereFragmentoFoto && !fotoEntregada)
            return;

        if (!string.IsNullOrEmpty(escenaSiguiente))
            StartCoroutine(RutinaVictoriaFade());
    }

    private IEnumerator RutinaVictoriaFade()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.LoadScene(escenaSiguiente);
            yield break;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            GameObject fadeObj = new GameObject("FadeVictoria");
            fadeObj.transform.SetParent(canvas.transform, false);

            Image imgFade = fadeObj.AddComponent<Image>();
            imgFade.color = new Color(0f, 0f, 0f, 0f);

            RectTransform rt = imgFade.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 1.5f;
                imgFade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t));
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene(escenaSiguiente);
    }
}
