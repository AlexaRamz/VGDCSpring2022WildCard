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

    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoseHealth()
    {
        if (!invincible)
        {
            playerHealth -= 1;
            Debug.Log(playerHealth);

            if (playerHealth <= 0)
            {
                playerState.changeState(PlayerState.Dead);
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
