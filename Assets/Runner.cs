using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Runner : MonoBehaviour
{
    public float minRandom = -1f;
    public float maxRandom = 1f;
    public float directionChangeInterval = 2f;
    public float moveSpeed = 2f;

    private Vector3 currentDirection;
    private float directionTimer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        PickNewDirection();
    }

    void FixedUpdate()
    {
        Vector3 move = currentDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0)
        {
            PickNewDirection();
        }
    }

    void PickNewDirection()
    {
        float randomX = Random.Range(minRandom, maxRandom);
        float randomZ = Random.Range(minRandom, maxRandom);
        currentDirection = new Vector3(randomX, 0, randomZ).normalized;
        directionTimer = directionChangeInterval;
    }
}
