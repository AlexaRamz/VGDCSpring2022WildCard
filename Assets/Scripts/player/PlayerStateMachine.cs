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

	//inputs
	KeyPressSet keysPressed;
	Vector2 mouseDir;

	//for coding and playtesting while im in lecture and cant use my mouse lmao, delete later - calli
	public bool mouselessAim;

	//component references (non-state-specific)
	Rigidbody2D body;
	new Camera camera;
	SpriteRenderer spriteRenderer;
	public Transform gun;

	public void Awake() {
		changeState(PlayerState.Normal);

		body = GetComponent<Rigidbody2D>();
		camera = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer>();

		keysPressed = new KeyPressSet();
		mouseDir = Vector2.zero;
	}

	public void LateUpdate() {
		lastVelocity = velocity;
	}

	#region **Collision Checks** (get onGround and friction)
	private bool onGround;
		bool last_onGround;
		bool onWall;
		bool last_onWall;
		private float friction;

		private void OnCollisionExit2D(Collision2D collision) {
			onGround = false;
			onWall = false;
			friction = airFriction;
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			last_onGround = onGround;
			last_onWall = onWall;
			EvaluateCollision(collision);
			friction = groundFriction;
			//RetrieveFriction(collision);
		}

		private void OnCollisionStay2D(Collision2D collision) {
			last_onGround = onGround;
			last_onWall = onWall;
			EvaluateCollision(collision);
			friction = groundFriction;
			//RetrieveFriction(collision);
		}

		private void EvaluateCollision(Collision2D collision) {
			for (int i = 0; i < collision.contactCount; i++) {
				Vector2 normal = collision.GetContact(i).normal;
				onGround |= (normal.y >= 0.9f);
				onWall |= (Mathf.Abs(normal.x) >= 0.9f);
			}
		}

		/*private void RetrieveFriction(Collision2D collision) {
			PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

			friction = 0;

			if (material != null) {
				friction = material.friction;
			}
		}*/
	#endregion


	public void Update() {
		//input assignment to key (and mouse button) presses, but these keybinds are just randomly chosen by me (calli) and we can change the controls here later
		keysPressed.left = Input.GetKey(KeyCode.A);
		keysPressed.right = Input.GetKey(KeyCode.D);

		if (!mouselessAim) {
			//mouse aim
			mouseDir = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			mouseDir.Normalize();

			//unless we add something with continuous firing we only want the shoot input on an initial press
			keysPressed.shootPressed = Input.GetMouseButtonDown(0);
		} else {
			//***for coding and playtesting while im in lecture and cant use my mouse lmao, delete later - calli
			mouseDir.x = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
			mouseDir.y = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);

			keysPressed.shootPressed = Input.GetKeyDown(KeyCode.C);
		}

		switch (playerState) {
			case PlayerState.Normal:
				UpdateNormal(keysPressed, mouseDir);
				break;
			case PlayerState.Dead:
				UpdateDead(keysPressed);
				break;
		}
	}
	public void FixedUpdate() {
		

		switch (playerState) {
			case PlayerState.Normal:
				FixedUpdateNormal(keysPressed, mouseDir);
				break;
			case PlayerState.Dead:
				FixedUpdateDead(keysPressed);
				break;
		}
	}

	public void changeState(PlayerState newState) {
		switch (playerState) {
			//on state exit
			case PlayerState.Normal:
				hasShot = false;
				break;
			case PlayerState.Dead:
				break;
			default:
				break;
		}
		switch (newState) {
			//on state enter
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
	Vector2 lastVelocity;
	float maxSpeedChange;
	float acceleration;
	[SerializeField] float groundFriction;
	[SerializeField] float airFriction;

	[SerializeField] float shootTestVelocity;
	bool hasShot;

	[SerializeField] float slideRatio;
	
	private void UpdateNormal(KeyPressSet _keysPressed, Vector2 aimDir)
	{
		//movement
		direction.x = (_keysPressed.right ? 1 : 0) - (_keysPressed.left ? 1 : 0);
		desiredVelocity = new Vector2(direction.x, 0f) * moveSpeed;

		if (direction.x != 0 && Mathf.Sign(direction.x) == Mathf.Sign(velocity.x)) {
			spriteRenderer.flipX = (Mathf.Sign(direction.x) == 1) ? false : true;
		}

		//gun rotation
		if (aimDir.x > 0) {
			gun.GetComponentInParent<SpriteRenderer>().flipX = false;
			gun.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aimDir.y, aimDir.x));
		} else {
			gun.GetComponentInParent<SpriteRenderer>().flipX = true;
			gun.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(-aimDir.y, -aimDir.x));
		}

		//test force (to make sure gun recoil will work properly with platformer controller movement)
		if (_keysPressed.shootPressed) {
			//Debug.Log("*vine boom sound effect*");
			hasShot = true;
		}
	}
	private void FixedUpdateNormal(KeyPressSet _keysPressed, Vector2 aimDir) {
		velocity = body.velocity;

		//movement
		acceleration = onGround ? maxAcceleration : maxAirAcceleration;
		maxSpeedChange = acceleration * Time.deltaTime;
		//if (onGround || direction.x == -Mathf.Sign(velocity.x) || (Mathf.Abs(velocity.x) < moveSpeed && direction.x != 0)) { // <- removes sliding
		if ((onGround && direction.x != Mathf.Sign(velocity.x)) || direction.x == -Mathf.Sign(velocity.x) || (Mathf.Abs(velocity.x) < moveSpeed && direction.x != 0)) {
			velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		} else {
			Debug.DrawRay(transform.position,Vector2.up,Color.blue,1f);
		}

		//on land (for now just slide but we may want camera shake here later maybe?)
		if (onGround && !last_onGround) {
			//Debug.Log("landed");
			//slide mechanic test
			if (lastVelocity.magnitude > moveSpeed && direction.x != 0) {
				velocity.x += direction.x * Mathf.Abs(lastVelocity.y) * slideRatio;
			}
		}

		velocity.x -= friction * Mathf.Sign(velocity.x) * Time.deltaTime;

		//aerial stuff
		if (body.velocity.y < 0) {
			body.gravityScale = fallingGravityScale;
		} else {
			body.gravityScale = defaultGravityScale;
		}

		//test force
		if (hasShot) {
			hasShot = false;
			
			velocity = -aimDir * shootTestVelocity;
		}

		body.velocity = velocity;
	}


	// function does nothing
	private void UpdateDead(KeyPressSet keysPressed)
	{
	}
	//function also does nothing but this time its on ~physics updates~ instead of frames
	private void FixedUpdateDead(KeyPressSet keysPressed) {
	}
}
