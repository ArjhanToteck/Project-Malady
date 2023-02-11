using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 350f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(0, 0);

        // controls

        // up

        // if string
        if (GameSettings.Controls.up.GetType() == typeof(string))
        {
            if (Input.GetKey((string)GameSettings.Controls.up))
            {
                rb.velocity += new Vector2(0, speed * Time.deltaTime);
            }
        }
        else // if keycode
        {
            if (Input.GetKey((KeyCode)GameSettings.Controls.up))
            {
                rb.velocity += new Vector2(0, speed * Time.deltaTime);
            }
        }

        // left

        // if string
        if (GameSettings.Controls.left.GetType() == typeof(string))
        {
            if (Input.GetKey((string)GameSettings.Controls.left))
            {
                rb.velocity += new Vector2(-speed * Time.deltaTime, 0);
            }
        }
        else // if keycode
        {
            if (Input.GetKey((KeyCode)GameSettings.Controls.left))
            {
                rb.velocity += new Vector2(-speed * Time.deltaTime, 0);
            }
        }

        // down

        // if string
        if (GameSettings.Controls.down.GetType() == typeof(string))
        {
            if (Input.GetKey((string)GameSettings.Controls.down))
            {
                rb.velocity += new Vector2(0, -speed * Time.deltaTime);
            }
        }
        else // if keycode
        {
            if (Input.GetKey((KeyCode)GameSettings.Controls.down))
            {
                rb.velocity += new Vector2(0, -speed * Time.deltaTime);
            }
        }

        // right

        // if string
        if (GameSettings.Controls.right.GetType() == typeof(string))
        {
            if (Input.GetKey((string)GameSettings.Controls.right))
            {
                rb.velocity += new Vector2(speed * Time.deltaTime, 0);
            }
        }
        else // if keycode
        {
            if (Input.GetKey((KeyCode)GameSettings.Controls.right))
            {
                rb.velocity += new Vector2(speed * Time.deltaTime, 0);
            }
        }

        transform.rotation = new Quaternion(0, 0, 0, 1);
    }
}
