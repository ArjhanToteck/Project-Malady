using UnityEngine;

public class AIVision : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        // only passes collision forward if it is a CPU
        if ((collision.CompareTag("Healthy") || collision.CompareTag("Infected")) && collision.gameObject.GetComponent<Renderer>().isVisible)
        {
            if (!!transform.parent.GetComponent<InfectedAI>())
            {
                transform.parent.GetComponent<InfectedAI>().OnVisionStay(collision);
            }
            else if (!!transform.parent.GetComponent<HealthyAI>())
            {
                transform.parent.GetComponent<HealthyAI>().OnVisionStay(collision);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // only passes collision forward if it is a CPU
        if(collision.CompareTag("Healthy") || collision.CompareTag("Infected"))
        {
            if (!!transform.parent.GetComponent<InfectedAI>())
            {
                transform.parent.GetComponent<InfectedAI>().OnVisionExit(collision);
            }
            else if (!!transform.parent.GetComponent<HealthyAI>())
            {
                transform.parent.GetComponent<HealthyAI>().OnVisionExit(collision);
            }
        }
    }
}
