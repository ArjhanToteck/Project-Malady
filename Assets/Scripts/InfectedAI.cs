using UnityEngine;
using Pathfinding;

public class InfectedAI : MonoBehaviour
{
    public Transform target;
    float nextWaypointDistance = 0.5f;
    Path path;
    int currentWaypoint = 0;
    bool pathEnded = true;
    Seeker seeker;

    float speed = 350f;
    Rigidbody2D rb;

    bool priorityTarget = false;
    Transform lastSeen;

    void Start()
    {
        // sets gameobject variables on start
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnVisionStay(Collider2D collision)
    {
        // checks if collision is a healthy 
        if (collision.CompareTag("Healthy"))
        {
            // checks if collision is closer than current target
            if (!target || priorityTarget == false || Vector2.Distance(transform.position, collision.transform.position) < Vector2.Distance(transform.position, target.position))
            {
                priorityTarget = true;
                target = collision.transform;
                seeker.StartPath(transform.position, target.position, OnPathComplete);
            }
        }
    }

    public void OnVisionExit(Collider2D collision)
    {
        // checks if collision leaving is target
        if (!!target && collision.transform == target)
        {
            // resets lastSeen
            if (!!lastSeen)
            {
                Destroy(lastSeen.gameObject);
            }
            

            // goes to place where previous target was last seen
            lastSeen = new GameObject().transform;
            lastSeen.SetParent(gameObject.transform);
            lastSeen.gameObject.name = "LastSeen";

            lastSeen.position = collision.transform.position;

            target = lastSeen;
            seeker.StartPath(transform.position, target.position, OnPathComplete);

            // will abandon the ghost if it finds an actual player
            priorityTarget = false;
        }
    }

    void Update()
    {
        // detects if target has been found
        if (target == null)
        {
            // wanders around map
            target = GameObject.Find("Floor").transform.GetChild(Random.Range(0, GameObject.Find("Floor").transform.childCount));

            seeker.StartPath((Vector2)transform.position, (Vector2)target.position, OnPathComplete);

            // will stop aimlessly wandering if it finds an actual player
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
        } else
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
        } else
        {
            pathEnded = true;
            target = null;
            currentWaypoint = 0;
        }
    }
}
