using UnityEngine;

public class controller_condor : MonoBehaviour
{
    public GameObject ave;
    private bool capturado = false;
    private SpriteRenderer[] renderizadoresAve;

    private void Start()
    {
        if (ave == null && transform.parent != null)
        {
            foreach (Transform hermano in transform.parent)
            {
                if (hermano.GetComponent<Condor>() != null || hermano.name.ToLower().Contains("ave") || hermano.name.ToLower().Contains("condor"))
                {
                    ave = hermano.gameObject;
                    break;
                }
            }
        }

        if (ave != null)
        {
            try
            {
                renderizadoresAve = ave.GetComponentsInChildren<SpriteRenderer>();
                SetRenderersActive(false);
            }
            catch (System.Exception) { }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (capturado) return;

        ControladorCuy cuy = collision.GetComponent<ControladorCuy>();
        if (cuy != null)
        {
            capturado = true;

            cuy.estadoActual = ControladorCuy.EstadoCuy.Agotado;

            Rigidbody2D cuyRb = cuy.GetComponent<Rigidbody2D>();
            if (cuyRb != null)
            {
                cuyRb.bodyType = RigidbodyType2D.Kinematic;
                cuyRb.linearVelocity = Vector2.zero;
            }

            Collider2D cuyCol = cuy.GetComponent<Collider2D>();
            if (cuyCol != null)
            {
                cuyCol.enabled = false;
            }

            if (renderizadoresAve == null && ave != null)
            {
                try { renderizadoresAve = ave.GetComponentsInChildren<SpriteRenderer>(); } catch { }
            }

            SetRenderersActive(true);

            if (ave != null)
            {
                try
                {
                    Condor scriptCondor = ave.GetComponent<Condor>();
                    if (scriptCondor != null)
                    {
                        scriptCondor.IniciarPicada(cuy.gameObject);
                    }
                }
                catch (System.Exception) { }
            }
        }
    }

    private void SetRenderersActive(bool active)
    {
        if (renderizadoresAve == null) return;

        foreach (SpriteRenderer sr in renderizadoresAve)
        {
            if (sr != null)
            {
                sr.enabled = active;
            }
        }
    }

    public void RestablecerCaptura()
    {
        capturado = false;
        SetRenderersActive(false);
    }
}
