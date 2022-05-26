using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed = 20.0f;
    private Rigidbody2D rb;
    private Health playerHealth;
    public Sprite destroyed;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.right * speed;
    }

    private IEnumerator DestroyAnim()
    {
        SetSpeed(0);
        GetComponent<SpriteRenderer>().sprite = destroyed;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth = collision.gameObject.GetComponent<Health>();
            playerHealth.LoseHealth();

            if (destroyed != null)
            {
                StartCoroutine(DestroyAnim());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.GetComponent<Tilemap>())
        {
            if (destroyed != null)
            {
                StartCoroutine(DestroyAnim());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
