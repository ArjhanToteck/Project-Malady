using System.Collections.Generic;
using UnityEngine;

public class Hivemind : MonoBehaviour
{
    public bool knownInfected = true;
    public GameObject alpha;
    public List<GameObject> children = new List<GameObject>();

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
                FindObjectOfType<GameManager>().ShowMessage("You are the host of a hivemind. Absorb as many people as you can.");
            }
        }

        GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;
    }

    public void InfectionTrigger(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Healthy") && !collision.gameObject.GetComponent<Asymptomatic>())
        {
            collision.gameObject.AddComponent<Hivemind>();
            collision.gameObject.GetComponent<Hivemind>().alpha = alpha;
            collision.gameObject.GetComponent<Hivemind>().disease = disease;
            collision.gameObject.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().infectedSprite;
            collision.gameObject.tag = "Infected";

            // checks if game not over
            if (FindObjectOfType<GameManager>().CountInfections(alpha, disease) == false)
            {
                // removes old AI if applicable
                if (!!collision.gameObject.GetComponent<HealthyAI>())
                {
                    Destroy(collision.gameObject.GetComponent<HealthyAI>());
                }

                // removes player movement and enables spectator mode if applicable
                if (!!collision.gameObject.GetComponent<PlayerMovement>())
                {
                    // removes player movement
                    Destroy(collision.gameObject.GetComponent<PlayerMovement>());
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                    // shows death screen
                    FindObjectOfType<GameManager>().ShowDeathMenu($"{alpha.GetComponent<PlayerName>().playerName} absorbed your body into their hivemind.");
                }

                // adds to children
                alpha.GetComponent<Hivemind>().children.Add(collision.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        // makes children follow alpha if alpha
        if(alpha == gameObject)
        {
            foreach (GameObject child in alpha.GetComponent<Hivemind>().children)
            {
                child.GetComponent<Rigidbody2D>().velocity = alpha.GetComponent<Rigidbody2D>().velocity;
            }
        }
    }
}
