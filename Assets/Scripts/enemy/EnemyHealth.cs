using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Color()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 86, 86, 255);
        yield return new WaitForSeconds(0.12f);
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
    void Die()
    {
        Destroy(gameObject);
    }
    public void LoseHealth()
    {
        health -= 1;
        StartCoroutine(Color());
        if (health <= 0)
        {
            Die();
        }
    }
}
