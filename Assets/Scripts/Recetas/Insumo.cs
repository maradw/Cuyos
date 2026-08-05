using UnityEngine;

public enum TipoInsumo
{
    PapaAmarilla,
    AjiAmarillo,
    QuesoFresco,
    GalletaDeSoda,
    AceitunaBotija,
    FragmentoDeFoto
}

[RequireComponent(typeof(Collider2D))]
public class Insumo : MonoBehaviour
{
    public TipoInsumo tipoDeInsumo;
    public string nombreInsumo = "Papa Amarilla";

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}
