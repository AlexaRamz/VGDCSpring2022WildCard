using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Dead
}

public struct KeyPressSet
{
    bool left;
    bool right;
    bool shoot;
}

public class PlayerStateMachine: MonoBehaviour
{
    public PlayerState playerState;

    public PlayerStateMachine(PlayerState state)
    {
        playerState = state;
    }

    public void Update()
    {
        KeyPressSet keysPressed = new KeyPressSet();
        // keysPressed.left = <is left being pressed>;
        // keysPressed.right = <is right being pressed>;
        // keysPressed.shoot = <is shoot being pressed>;

        switch (playerState)
        {
            case PlayerState.Normal:
                UpdateNormal(keysPressed);
                break;
            case PlayerState.Dead:
                UpdateDead(keysPressed);
                break;
        }
    }

    public void changeState(PlayerState newState)
    {
        switch (playerState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Dead:
                break;
        }
        switch (newState)
        {
            case PlayerState.Normal:
                break;
            case PlayerState.Dead:
                break;
        }
        playerState = newState;
    }

    private void UpdateNormal(KeyPressSet keysPressed)
    {
        // do things
    }

    // function does nothing
    private void UpdateDead(KeyPressSet keysPressed)
    {
    }
}
