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
    public bool left;
    public bool right;
    public bool shootPressed;
}

public class PlayerStateMachine : MonoBehaviour {
    public PlayerState playerState;

    public PlayerStateMachine(PlayerState state) {
        playerState = state;
    }

    //component references (non-state-specific)
    Rigidbody2D body;
    new Camera camera;

    public void Awake() {
        changeState(PlayerState.Normal);

        body = GetComponent<Rigidbody2D>();
        camera = Camera.main;
    }

    #region **Collision Checks** (get onGround and friction)
        private bool onGround;
        private float friction;

        private void OnCollisionExit2D(Collision2D collision) {
            onGround = false;
            friction = 0;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            EvaluateCollision(collision);
            RetrieveFriction(collision);
        }

        private void OnCollisionStay2D(Collision2D collision) {
            EvaluateCollision(collision);
            RetrieveFriction(collision);
        }

        private void EvaluateCollision(Collision2D collision) {
            for (int i = 0; i < collision.contactCount; i++) {
                Vector2 normal = collision.GetContact(i).normal;
                onGround |= normal.y >= 0.9f;
            }
        }

        private void RetrieveFriction(Collision2D collision) {
            PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

            friction = 0;

            if (material != null) {
                friction = material.friction;
            }
        }
    #endregion


    public void Update() {
        KeyPressSet keysPressed = new KeyPressSet();

        //input assignment to key (and mouse button) presses, but these keybinds are just randomly chosen by me (calli) and we can change the controls here later
        keysPressed.left = Input.GetKey(KeyCode.A);
        keysPressed.right = Input.GetKey(KeyCode.D);
        //unless we add something with continuous firing we only want the shoot input on an initial press
        keysPressed.shootPressed = Input.GetMouseButtonDown(0);

        //mouse aim
        Vector2 mousedir = (transform.position - camera.ScreenToWorldPoint(Input.mousePosition));
        mousedir.Normalize();

        switch (playerState) {
            case PlayerState.Normal:
                UpdateNormal(keysPressed, mousedir);
                break;
            case PlayerState.Dead:
                UpdateDead(keysPressed);
                break;
        }
    }
    public void FixedUpdate() {
        

        switch (playerState) {
            case PlayerState.Normal:
                FixedUpdateNormal();
                break;
            case PlayerState.Dead:
                FixedUpdateDead();
                break;
        }
    }

    public void changeState(PlayerState newState) {
        switch (playerState) {
            case PlayerState.Normal:
                hasShot = false;
                break;
            case PlayerState.Dead:
                break;
            default:
                break;
        }
        switch (newState) {
            case PlayerState.Normal:
                break;
            case PlayerState.Dead:
                break;
            default:
                break;
        }
        playerState = newState;
    }


    //normal state variables
    [SerializeField, Range(0f, 100f)] float moveSpeed = 4f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] float maxAirAcceleration = 20f;
    [SerializeField, Range(0f, 5f)] float fallingGravityScale = 5f;
    [SerializeField, Range(0f, 10f)] float defaultGravityScale = 3f;

    Vector2 direction;
    Vector2 desiredVelocity;
    Vector2 velocity;
    float maxSpeedChange;
    float acceleration;

    public float shootTestVelocity;
    Vector2 shootTestDir; //this is essentially making the mouse aim a higher scope variable for no good reason but
    //                              later when we implement this fully we'll have other things to base the recoil on so for now i think it's fine?
    bool hasShot;
    
    private void UpdateNormal(KeyPressSet keysPressed, Vector2 mousedir)
    {
        //**friction isnt a simulation of friction it just slows the player if theyre on a surface with a higher friction value,
        //**please remind me to remove it later if we dont end up having any "sticky" surfaces in the game that would actually need the feature

        //movement
        direction.x = (keysPressed.right ? 1 : 0) - (keysPressed.left ? 1 : 0);
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(moveSpeed - friction, 0f);

        //test force (to make sure gun recoil will work properly with platformer controller movement)
        if (keysPressed.shootPressed) {
            Debug.Log("*vine boom sound effect*");
            hasShot = true;
            shootTestDir = mousedir;
		}
    }
    private void FixedUpdateNormal() {
        velocity = body.velocity;

        //movement
        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        //aerial stuff
        if (body.velocity.y < 0) {
            body.gravityScale = fallingGravityScale;
		} else {
            body.gravityScale = defaultGravityScale;
		}

        //test force
        if (hasShot) {
            hasShot = false;

            velocity = shootTestDir * shootTestVelocity;
		}

        body.velocity = velocity;
    }


    // function does nothing
    private void UpdateDead(KeyPressSet keysPressed)
    {
    }
    //function also does nothing but this time its on ~physics updates~ instead of frames
    private void FixedUpdateDead() {
    }
}
