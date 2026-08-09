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
    
    private bool fotoEntregada = false;

    private void Start()
    {
        if (recetaRequisitos.Count == 0)
        {
            ConfigurarRecetaDefecto();
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
        if (tipoEntregado == TipoInsumo.FragmentoDeFoto)
        {
            fotoEntregada = true;
            Debug.Log("Foto entregada");
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
                    Debug.Log($"Entregado: {tipoEntregado}. Progreso: {requisito.cantidadEntregada}/{requisito.cantidadNecesaria}");
                    
                    ComprobarVictoriaReceta();
                    return true;
                }
            }
        }
        return false;
    }

    private void ComprobarVictoriaReceta()
    {
        foreach (var requisito in recetaRequisitos)
        {
            if (requisito.cantidadEntregada < requisito.cantidadNecesaria)
            {
                return;
            }
        }

        if (requiereFragmentoFoto && !fotoEntregada)
        {
            Debug.Log("Falta foto");
            return;
        }

        Debug.Log("Victoria");
        
        if (!string.IsNullOrEmpty(escenaSiguiente))
        {
            StartCoroutine(RutinaVictoriaFade());
        }
    }

    private IEnumerator RutinaVictoriaFade()
    {
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
