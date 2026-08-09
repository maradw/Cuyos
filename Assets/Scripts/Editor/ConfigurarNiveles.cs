#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class ConfigurarNiveles : MonoBehaviour
{
    [MenuItem("Herramientas/Agregar Todas Las Escenas a Build Settings")]
    public static void AutoAgregarEscenas()
    {
        string[] rutasEscenas = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
        EditorBuildSettingsScene[] escenasBuild = new EditorBuildSettingsScene[rutasEscenas.Length];
        
        for (int i = 0; i < rutasEscenas.Length; i++)
        {
            escenasBuild[i] = new EditorBuildSettingsScene(rutasEscenas[i].Replace("\\", "/"), true);
        }
        
        EditorBuildSettings.scenes = escenasBuild;
        Debug.Log("Todas las escenas fueron agregadas correctamente al Build Settings.");
    }
}
#endif
