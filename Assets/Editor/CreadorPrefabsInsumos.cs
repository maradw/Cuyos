using UnityEngine;
using UnityEditor;
using System.IO;

public class CreadorPrefabsInsumos : Editor
{
    [MenuItem("Tools/Crear Prefabs de Insumos")]
    public static void CrearPrefabs()
    {
        string carpetaPrefabs = "Assets/Prefabs/Insumos";

        if (!Directory.Exists(carpetaPrefabs))
        {
            Directory.CreateDirectory(carpetaPrefabs);
            AssetDatabase.Refresh();
        }

        // Crear insumos con tamaños y colliders más grandes para facilitar la recolección
        CrearInsumoEspecifico("PapaAmarilla_Prefab", TipoInsumo.PapaAmarilla, "Papa Amarilla", Color.yellow);
        CrearInsumoEspecifico("AjiAmarillo_Prefab", TipoInsumo.AjiAmarillo, "Ají Amarillo", new Color(1f, 0.6f, 0f));
        CrearInsumoEspecifico("QuesoFresco_Prefab", TipoInsumo.QuesoFresco, "Queso Fresco", Color.white);
        CrearInsumoEspecifico("GalletaDeSoda_Prefab", TipoInsumo.GalletaDeSoda, "Galleta de Soda", new Color(0.9f, 0.85f, 0.7f));
        CrearInsumoEspecifico("AceitunaBotija_Prefab", TipoInsumo.AceitunaBotija, "Aceituna Botija", new Color(0.2f, 0.1f, 0.3f));
        CrearInsumoEspecifico("FragmentoDeFoto_Prefab", TipoInsumo.FragmentoDeFoto, "Fragmento de Foto", Color.red);

        AssetDatabase.Refresh();
        Debug.Log("¡Todos los prefabs de insumos han sido creados con escala aumentada y colliders optimizados en Assets/Prefabs/Insumos!");
    }

    private static void CrearInsumoEspecifico(string nombrePrefab, TipoInsumo tipo, string nombreLegible, Color colorVisual)
    {
        string rutaCompleta = $"Assets/Prefabs/Insumos/{nombrePrefab}.prefab";

        GameObject go = new GameObject(nombreLegible);

        // Aumentar la escala base del GameObject para hacerlo más grande visualmente
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        sr.color = colorVisual;

        // Aumentar el radio del Collider2D a 0.8f (efectivo con escala 1.5f para una zona de trigger amplia)
        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.8f;

        Insumo scriptInsumo = go.AddComponent<Insumo>();
        scriptInsumo.tipoDeInsumo = tipo;
        scriptInsumo.nombreInsumo = nombreLegible;

        PrefabUtility.SaveAsPrefabAsset(go, rutaCompleta);
        DestroyImmediate(go);
    }
}
