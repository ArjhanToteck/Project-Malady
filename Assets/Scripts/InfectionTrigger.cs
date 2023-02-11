using UnityEngine;

public class InfectionTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!!transform.parent.GetComponent<ClassicDisease>())
        {
            transform.parent.GetComponent<ClassicDisease>().InfectionTrigger(collision);
        }
        else if (!!transform.parent.GetComponent<Hivemind>())
        {
           transform.parent.GetComponent<Hivemind>().InfectionTrigger(collision);
        }
        else if (!!transform.parent.GetComponent<Parasite>())
        {
            transform.parent.GetComponent<Parasite>().InfectionTrigger(collision);
        }
        else if (!!transform.parent.GetComponent<Asymptomatic>())
        {
            transform.parent.GetComponent<Asymptomatic>().InfectionTrigger(collision);
        }
    }
}
