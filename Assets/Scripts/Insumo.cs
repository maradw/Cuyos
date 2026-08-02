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
    [Header("Configuración del Insumo")]
    [Tooltip("El tipo de ingrediente que representa este objeto.")]
    public TipoInsumo tipoDeInsumo;

    [Tooltip("Nombre legible para mostrar en la interfaz de usuario.")]
    public string nombreInsumo = "Papa Amarilla";

    private void Awake()
    {
        // IDIOTA ASEGURA LOS COLLIDER ANGELO
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
}
