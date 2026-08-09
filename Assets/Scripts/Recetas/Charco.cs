using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Charco : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            cuy.estaEmpapado = true;
            cuy.temporizadorEmpapado = 2.0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            cuy.estaEmpapado = true;
            cuy.temporizadorEmpapado = 3.0f; 
        }
    }
}
