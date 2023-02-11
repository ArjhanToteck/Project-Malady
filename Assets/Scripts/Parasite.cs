using UnityEngine;

public class Parasite : MonoBehaviour
{
    public bool knownInfected = true;
    public GameObject alpha;

    public float deathTimerMax = 25f;
    float deathTimer;

    public Disease disease;

    void Start()
    {
        transform.tag = "Infected";
        if (!alpha)
        {
            alpha = gameObject;
            FindObjectOfType<GameManager>().CountInfections(alpha, disease);
        }

        GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;

        deathTimer = deathTimerMax;

        if(alpha == gameObject)
        {
            if (!!GetComponent<PlayerMovement>())
            {
                FindObjectOfType<GameManager>().ShowMessage($"You are the host of a parasite. Pass it to someone else within {deathTimerMax} seconds or it will kill you.");
            }
        }
    }

    public void InfectionTrigger(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Healthy") && !collision.gameObject.GetComponent<Asymptomatic>() && deathTimer > 0)
        {
            // removes old target AI if applicable
            if (!!collision.gameObject.GetComponent<HealthyAI>())
            {
                Destroy(collision.gameObject.GetComponent<HealthyAI>());
            }

            // infects target
            collision.gameObject.AddComponent<Parasite>();
            collision.gameObject.GetComponent<Parasite>().alpha = alpha;
            collision.gameObject.GetComponent<Parasite>().disease = disease;
            collision.gameObject.GetComponent<Parasite>().deathTimerMax = deathTimerMax;
            collision.gameObject.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;

            // activates target AI if applicable
            if (!!collision.gameObject.GetComponent<HealthyAI>())
            {
                collision.gameObject.AddComponent<InfectedAI>();
            }
            else // this means it's a human player
            {
                FindObjectOfType<GameManager>().ShowMessage($"A parasite was transferred to you. Pass it to someone else within {deathTimerMax} seconds or it will kill you.");
            }

            // removes parasite AI from player if applicable
            if (!!GetComponent<InfectedAI>())
            {
                Destroy(GetComponent<InfectedAI>());
                gameObject.AddComponent<HealthyAI>();
            }

            // heals player
            gameObject.tag = "Healthy";
            GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().healthySprite;
            Destroy(GetComponent<Parasite>());
        }
    }

    void Update()
    {
        // lowers timer
        deathTimer -= Time.deltaTime;

        // dies
        if(deathTimer <= 0f && !GetComponent<HealthyAI>())
        {
            GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().deadSprite;
            gameObject.tag = "Dead";
            FindObjectOfType<GameManager>().CountInfections(alpha, disease);

            // checks if human player
            if (!!GetComponent<PlayerMovement>())
            {
                // removes player movement
                Destroy(GetComponent<PlayerMovement>());
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                if(FindObjectOfType<GameManager>().CountKills(alpha, disease) == false)
                {
                    // shows death screen
                    FindObjectOfType<GameManager>().ShowDeathMenu($"Your time ran out and the parasite {disease.name} killed you.");
                }
            }

            // destroys parasite AI if applicable
            if(!!GetComponent<InfectedAI>()) Destroy(GetComponent<InfectedAI>());

            // makes corpse stay still
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // destroys this script
            Destroy(GetComponent<Parasite>());
        }
    }
}