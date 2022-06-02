using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed = 20.0f;
    private Rigidbody2D rb;
    public Sprite destroyed;
    public string target;
    private bool canHit = true;

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
        canHit = false;
        SetSpeed(0);
        GetComponent<SpriteRenderer>().sprite = destroyed;
        if (GetComponent<ParticleSystem>())
        {
            Debug.Log("On");
            GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canHit)
        {
            if (collision.gameObject.CompareTag(target))
            {
                if (collision.gameObject.GetComponent<Health>())
                {
                    collision.gameObject.GetComponent<Health>().LoseHealth();
                }
                else if (collision.gameObject.GetComponent<EnemyHealth>())
                {
                    collision.gameObject.GetComponent<EnemyHealth>().LoseHealth();
                }

                StartCoroutine(DestroyAnim());
            }
            else if (collision.gameObject.GetComponent<Tilemap>())
            {
                StartCoroutine(DestroyAnim());
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
