using UnityEngine;

public class ClassicDisease : MonoBehaviour
{
    public bool knownInfected = true;
    public bool lethal = false;
    public GameObject alpha;

    public Disease disease;

    void Start()
    {
        transform.tag = "Infected";
        if (!alpha)
        {
            alpha = gameObject;
            FindObjectOfType<GameManager>().CountInfections(alpha, disease);

            if (!!GetComponent<PlayerMovement>())
            {
                if(lethal == false)
                {
                    FindObjectOfType<GameManager>().ShowMessage("You are the alpha of a classic disease. Infect as many others as you can.");
                }
                else
                {
                    FindObjectOfType<GameManager>().ShowMessage("You are the alpha of a lethal disease. Kill as many others as you can.");
                }
            }
        }

        GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;
    }

    public void InfectionTrigger(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Healthy") && !collision.gameObject.GetComponent<Asymptomatic>())
        {
            // removes old target AI if applicable
            if (!!collision.gameObject.GetComponent<HealthyAI>())
            {
                Destroy(collision.gameObject.GetComponent<HealthyAI>());
            }

            // checks if lethal
            if (lethal)
            {
                // kills player
                collision.gameObject.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().deadSprite;
                collision.gameObject.tag = "Dead";
                FindObjectOfType<GameManager>().CountInfections(alpha, disease);

                // makes player's corpse stay still
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                // removes player movement and enables spectator mode if applicable
                if (!!collision.gameObject.GetComponent<PlayerMovement>())
                {
                    // removes player movement
                    Destroy(collision.gameObject.GetComponent<PlayerMovement>());
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                    if(FindObjectOfType<GameManager>().CountKills(alpha, disease) == false){

                        // shows death screen
                        FindObjectOfType<GameManager>().ShowDeathMenu($"{alpha.GetComponent<PlayerName>().playerName} spread {disease.name} to you and killed you.");
                    }
                }
            }
            else
            {
                // infects target
                collision.gameObject.AddComponent<ClassicDisease>();
                collision.gameObject.GetComponent<ClassicDisease>().alpha = alpha;
                collision.gameObject.GetComponent<ClassicDisease>().disease = disease;
                collision.gameObject.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;
                collision.gameObject.tag = "Infected";
                FindObjectOfType<GameManager>().CountInfections(alpha, disease);

                // activates target AI if applicable
                if (!!collision.gameObject.GetComponent<HealthyAI>())
                {
                    collision.gameObject.AddComponent<InfectedAI>();
                }
                else // this means it's a human player
                {
                    if(lethal == false)
                    {

                        FindObjectOfType<GameManager>().ShowMessage("A classic disease was transmitted to you. Infect as many others as you can.");
                    }
                }
            }
        }
    }
}
