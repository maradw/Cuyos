using UnityEngine;

public class ChingueController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform[] walkPoints;
    [SerializeField] float speed = 3f;
    [SerializeField] float minDistance = 0.1f;
    bool isSleeping;
    int currentPoint;
    [SerializeField] ParticleSystem effect;
    void Start()
    {
        effect.Stop();
        isSleeping = false;
        currentPoint = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentPoint + 1 >= walkPoints.Length)
        {
           // isSleeping = true;
        }
        if(isSleeping== true)
        {

        }
    }
    private void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPos = walkPoints[currentPoint].position;
        Vector2 newPos = Vector2.MoveTowards(currentPosition, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        /*if (targetPos.x > currentPosition.x)
            sr.flipX = true;
        else if (targetPos.x < currentPosition.x)
            sr.flipX = false;
    */
        if (Vector2.Distance(currentPosition, targetPos) <= minDistance && isSleeping == false)
        {
            currentPoint++;

            if (currentPoint+1 >= walkPoints.Length )
            {
                isSleeping = true;
                print("sleep");
                Debug.Log("waza");
                //currentPoint = 0;
            }
                
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isSleeping== true && collision.gameObject.tag== "Player")
        {
            //effect.duration == 5f;
            var main = effect.main;
            //main.duration = 5f;

            //effect.time = 10f;

           // effect.d
           // effect.time = 10f;
            effect.Play();
        }
    }
}