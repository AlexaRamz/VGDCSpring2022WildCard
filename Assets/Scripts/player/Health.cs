using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private PlayerStateMachine playerState;

    [SerializeField] private HealthUI healthDisplay;

    public float invinciblityPeriod = 0.5f;
    private bool invincible;

    public GameObject spawnPoint;

    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerStateMachine>();
        transform.position = spawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Flicker()
    {
        bool on = true;
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.05f);
            if (on == true)
            {
                on = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                on = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void LoseHealth()
    {
        if (!invincible)
        {
            playerHealth -= 1;
            StartCoroutine(Flicker());

            if (playerHealth <= 0)
            {
                transform.position = spawnPoint.transform.position;
                playerHealth = 4;
                healthDisplay.ResetHearts();
            }

            healthDisplay.LoseAHeart();
            StartCoroutine(HitInvinciblity());
        }
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }

    private IEnumerator HitInvinciblity()
    {
        invincible = true;

        yield return new WaitForSeconds(invinciblityPeriod);

        invincible = false;
    }
}
