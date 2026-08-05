using UnityEngine;

public class controller_condor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool condor;
    public bool cuy;
    public GameObject player;
    public GameObject ave;
    public bool mov;
    public Rigidbody2D rbcondor;
    public Rigidbody2D rbcuy;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rbcondor = ave.GetComponent<Rigidbody2D>();
        rbcuy = player.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(condor && cuy)
        {
            player.transform.SetParent(ave.transform);
            mov = true;
        }
        if(mov)
        {
            rbcuy.linearVelocity = rbcondor.linearVelocity;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            cuy = true;

        }
        if (collision.transform.CompareTag("Condor"))
        {
            condor = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            cuy = false;

        }
        if (collision.transform.CompareTag("Condor"))
        {
            condor = false;
        }

    }
}
