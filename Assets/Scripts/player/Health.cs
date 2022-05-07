using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private PlayerStateMachine playerState;

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
        playerHealth -= 1;
        Debug.Log(playerHealth);

        if (playerHealth <= 0)
        {
            playerState.changeState(PlayerState.Dead);
        }

    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }
}
