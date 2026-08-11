using UnityEngine;

public enum TipoInsumo
{
    PapaAmarilla,
    AjiAmarillo,
    QuesoFresco,
    GalletaDeSoda,
    AceitunaBotija,
    Leche,
    Huacatay,
    Ajo,
    Choclo,
    Azucar,
    Pasas,
    Anis,
    Rocoto,
    Mani,
    CarneMolida,
    Huevo,
    Camote,
    Habas,
    Trigo,
    Zapallo,
    FragmentoDeFoto,
    ItemTrampa
}

[RequireComponent(typeof(Collider2D))]
public class Insumo : MonoBehaviour
{
    public TipoInsumo tipoDeInsumo;
    public string nombreInsumo = "";

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}
