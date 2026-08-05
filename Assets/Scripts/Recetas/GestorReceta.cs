using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorReceta : MonoBehaviour
{
    [System.Serializable]
    public struct RequisitoInsumo
    {
        public TipoInsumo tipo;
        public int cantidadNecesaria;
        [HideInInspector] public int cantidadEntregada;
    }

    public string nombrePlato = "Papa a la Huancaína";
    public List<RequisitoInsumo> recetaRequisitos = new List<RequisitoInsumo>();
    public bool requiereFragmentoFoto = true;
    
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
    }
}
