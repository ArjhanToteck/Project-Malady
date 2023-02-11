using UnityEngine;
using Pathfinding;

public class HealthyAI : MonoBehaviour
{
    Transform target;
    float nextWaypointDistance = 0.5f;
    Path path;
    int currentWaypoint = 0;
    bool pathEnded = true;
    Seeker seeker;

    float speed = 350f;
    Rigidbody2D rb;

    bool priorityTarget = false;
    Transform enemy;
    Transform fleeTarget;

    void Start()
    {
        // sets gameobject variables on start
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnVisionStay(Collider2D collision)
    {
        // checks if collision is a player
        if (collision.CompareTag("Infected"))
        {
            // checks if collision is closer than current target
            if (priorityTarget == false || !target || Vector2.Distance(transform.position, collision.transform.position) < Vector2.Distance(transform.position, enemy.position))
            {
                priorityTarget = true;
                enemy = collision.transform;

                // resets fleeTarget
                if (!!fleeTarget)
                {
                    Destroy(fleeTarget.gameObject);
                }

                // goes to place where previous target was last seen
                fleeTarget = new GameObject().transform;
                fleeTarget.SetParent(gameObject.transform);
                fleeTarget.gameObject.name = "FleeTarget";

                // raycast setup
                Vector2 rayDirection;
                RaycastHit2D hit;
                Transform farthestObstacle = transform;

                // loops through left, right, up, and down from negative angle of enemy
                for(int i = 0; i < 8; i++)
                {
                    // raycasts to angle opposite of enemy
                    rayDirection = (transform.position - collision.transform.position).normalized;
                    hit = Physics2D.Raycast(transform.position, rayDirection.Rotate(i * 45), 5, LayerMask.GetMask("Obstacles"));

                    // checks if collided with wall
                    if (!hit.collider)
                    {
                        // sets fleeTarget to end of ray
                        fleeTarget.position = (rayDirection.normalized * 5) + (Vector2)transform.position;

                        target = fleeTarget;
                        seeker.StartPath(transform.position, target.position, OnPathComplete);
                        break;
                    } else
                    {
                        if(Vector2.Distance(transform.position, hit.transform.position) > Vector2.Distance(transform.position, farthestObstacle.position))
                        {
                            farthestObstacle = hit.transform;
                        }
                    }
                }

                if(target != fleeTarget)
                {
                    // sets fleeTarget to end of ray
                    fleeTarget.position = farthestObstacle.position;

                    target = fleeTarget;
                    seeker.StartPath(transform.position, target.position, OnPathComplete);
                }
            }
        }
    }

    public void OnVisionExit(Collider2D collision)
    {
        // checks if collision leaving is enemy
        if (collision.transform == enemy)
        {
            target = null;
            enemy = null;
        }
    }

    void Update()
    {
        // detects if target has been found
        if (target == null)
        {
            // wanders around map
            target = GameObject.Find("Floor").transform.GetChild(Random.Range(0, GameObject.Find("Floor").transform.childCount));
            seeker.StartPath(transform.position, target.position, OnPathComplete);
            priorityTarget = false;
        }
    }

    void FixedUpdate()
    {
        // moves through astar path

        // makes sure path exists
        if (pathEnded || path == null)
        {
            return;
        }
        else
        {
            // resets velocity
            rb.velocity = new Vector2(0, 0);

            // checks if the path ended
            if (currentWaypoint >= path.vectorPath.Count)
            {
                pathEnded = true;
                target = null;
                currentWaypoint = 0;
                return;
            }
            else
            {
                pathEnded = false;

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 velocity = direction * speed * Time.deltaTime;

                rb.velocity = new Vector2(0, 0);
                rb.velocity = velocity;

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error && p != null)
        {
            path = p;
            pathEnded = false;
            currentWaypoint = 0;
        }
        else
        {
            pathEnded = true;
            target = null;
            currentWaypoint = 0;
        }
    }
}
