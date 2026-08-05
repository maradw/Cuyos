using UnityEngine;

public class Condor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 3f;
    public Vector2 direccion;
    public GameObject sombra_guia;
    public Transform sombra;
    public Rigidbody2D rb2d;
    public Vector3 new_escala;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sombra_guia = transform.parent.gameObject;
        sombra = transform.parent.Find("sombra_condor");
        direccion = (sombra_guia.transform.position-transform.position).normalized;
        new_escala = sombra.localScale;

        
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.linearVelocity = direccion * speed;
        float dist = transform.position.y - sombra.transform.position.y;
        float escala = 1 / (dist + 1);
        new_escala.x = escala;
        sombra.localScale = new_escala;
        sombra.transform.position = new Vector3(transform.position.x, sombra.transform.position.y, sombra.transform.position.z);
        

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Sombra"))
        {
            change_direction();
        }
    }
    public void change_direction()
    {
        Vector2 new_direccion = new Vector2((direccion.x), (direccion.y * -1));
        direccion = new_direccion;
    }
    private void OnBecameInvisible()
    {
        Destroy(sombra_guia);
    }
}
