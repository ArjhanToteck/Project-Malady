using UnityEngine;

public class Asymptomatic : MonoBehaviour
{
    public bool knownInfected = true;
    public bool lethal = false;
    public GameObject alpha;

    public float infectionTimer = 10f;
    public float timeRemaining;
    public bool timerStopped = false;

    public Disease disease;

    void Start()
    {
        if (!alpha)
        {
            alpha = gameObject;
            FindObjectOfType<GameManager>().CountInfections(alpha, disease);
        }

        timeRemaining = infectionTimer;
    }

    public void InfectionTrigger(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Healthy") && !collision.gameObject.GetComponent<Asymptomatic>())
        {

            // infects target
            collision.gameObject.AddComponent<Asymptomatic>();
            collision.gameObject.GetComponent<Asymptomatic>().alpha = alpha;
            collision.gameObject.GetComponent<Asymptomatic>().lethal = lethal;
            collision.gameObject.GetComponent<Asymptomatic>().disease = disease;
            collision.gameObject.GetComponent<Asymptomatic>().infectionTimer = infectionTimer;
        }
    }

    void Update()
    {
        // lowers timer
        if (timeRemaining > 0f && timerStopped == false)
        {
            timeRemaining -= Time.deltaTime;
        }

        // timer runs out
        if (timeRemaining <= 0f && timerStopped == false)
        {
            // stops timer
            timerStopped = true;

            if (lethal && alpha != gameObject)
            {
                // destroys old AI if applicable
                if (!!GetComponent<HealthyAI>())
                {
                    Destroy(GetComponent<HealthyAI>());
                }

                // player is killed
                GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().deadSprite;
                transform.tag = "Dead";

                // checks if human player
                if (!!GetComponent<PlayerMovement>())
                {
                    // removes player movement
                    Destroy(GetComponent<PlayerMovement>());
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                    // checks if game not over
                    if (FindObjectOfType<GameManager>().CountKills(alpha, disease) == false)
                    {
                        // shows death screen
                        FindObjectOfType<GameManager>().ShowDeathMenu($"Your time ran out and the hidden asymptomatic disease {disease.name} killed you.");
                    }
                }

                // makes corpse stay still
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                // makes sure asymptomatic script is removed
                Destroy(GetComponent<Asymptomatic>());
            } else
            {
                // player is infected
                GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;
                transform.tag = "Infected";

                // activates asymptomatic AI if applicable
                if (!!GetComponent<HealthyAI>())
                {
                    Destroy(GetComponent<HealthyAI>());
                    gameObject.AddComponent<InfectedAI>();
                }

                // checks if game not over
                if (FindObjectOfType<GameManager>().CountInfections(alpha, disease) == false)
                {
                    
                    if(GetComponent<PlayerMovement>()) // this means it's a human player
                    {
                        FindObjectOfType<GameManager>().ShowMessage("You were infected with an asympomatic disease and it is just now appearing.");
                    }
                }                    
            }
        }
    }
}
