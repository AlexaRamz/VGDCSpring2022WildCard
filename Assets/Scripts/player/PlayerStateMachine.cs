using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
	Normal,
	Slide,
	Dead
}

public struct KeyPressSet
{
	public bool left;
	public bool right;
	public bool downPressed;
	public bool down;
	public bool shootPressed;
}

public class PlayerStateMachine : MonoBehaviour {
	public PlayerState playerState;

	public PlayerStateMachine(PlayerState state) {
		playerState = state;
	}

	//inputs
	KeyPressSet keysPressed;
	Vector2 aimDir;

	//for coding and playtesting while im in lecture and cant use my mouse lmao, delete later - calli
	public bool mouselessAim;

	//component references (non-state-specific)
	Rigidbody2D body;
	new Camera camera;
	SpriteRenderer spriteRenderer;
	public Transform gun;
	Transform launchPoint;

	public void Awake() {
		changeState(PlayerState.Normal);

		body = GetComponent<Rigidbody2D>();
		camera = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer>();

		keysPressed = new KeyPressSet();
		aimDir = Vector2.zero;

		launchPoint = gun.Find("LaunchPoint");
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
		keysPressed.downPressed = Input.GetKeyDown(KeyCode.S);
		keysPressed.down = Input.GetKey(KeyCode.S);

		if (!mouselessAim) {
			//mouse aim
			aimDir = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			aimDir.Normalize();

			//unless we add something with continuous firing we only want the shoot input on an initial press
			keysPressed.shootPressed = Input.GetMouseButtonDown(0);
		} else {
			//***for coding and playtesting while im in lecture and cant use my mouse lmao, delete later - calli
			aimDir.x = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
			aimDir.y = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);

			keysPressed.shootPressed = Input.GetKeyDown(KeyCode.C);
		}

		switch (playerState) {
			case PlayerState.Normal:
				UpdateNormal();
				break;
			case PlayerState.Slide:
				UpdateSlide();
				break;
			case PlayerState.Dead:
				UpdateDead();
				break;
		}
	}
	public void FixedUpdate() {
		switch (playerState) {
			case PlayerState.Normal:
				FixedUpdateNormal();
				break;
			case PlayerState.Slide:
				FixedUpdateSlide();
				break;
			case PlayerState.Dead:
				FixedUpdateDead();
				break;
		}
	}

	public void changeState(PlayerState newState) {
		switch (playerState) {
			//on state exit
			case PlayerState.Normal:
				startSlide = false;
				hasShot = false;
				slideReverseTime = -1;
				break;
			case PlayerState.Slide:
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
			case PlayerState.Slide:
				//add slideSpd
				int slideDir;
				if (Mathf.Abs(velocity.x) < slide_stopSpd) {
					if (direction.x != 0) {
						slideDir = (int)direction.x;
					} else {
						slideDir = spriteRenderer.flipX ? -1 : 1;
						slideReverseTime = Time.time;
					}
				} else {
					slideDir = (int)Mathf.Sign(velocity.x);
				}
				if (onGround && !last_onGround) {
					velocity.x += slideSpd * slideDir;
				} else {
					velocity.x = slideSpd * slideDir;
				}
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
	bool startSlide;
	bool hasShot;
	private void UpdateNormal()
	{
		//movement
		direction.x = (keysPressed.right ? 1 : 0) - (keysPressed.left ? 1 : 0);
		desiredVelocity = new Vector2(direction.x, 0f) * moveSpeed;

		if (direction.x != 0 && Mathf.Sign(direction.x) == Mathf.Sign(velocity.x)) {
			spriteRenderer.flipX = (Mathf.Sign(direction.x) == 1) ? false : true;
		}

		//gun rotation
		if (aimDir.x > 0) {
			gun.GetComponentInParent<SpriteRenderer>().flipY = false;
		} 
		else
		{
			gun.GetComponentInParent<SpriteRenderer>().flipY = true;
		}
		gun.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aimDir.y, aimDir.x));

		//shoot input
		if (keysPressed.shootPressed) {
			hasShot = true;
		}

		//enter slide
		if (keysPressed.downPressed) {
			startSlide = true;
		}
		if (!keysPressed.down) {
			startSlide = false;
		}
	}
	private void FixedUpdateNormal() {
		velocity = body.velocity;

		//movement
		acceleration = onGround ? maxAcceleration : maxAirAcceleration;
		maxSpeedChange = acceleration * Time.deltaTime;
		//if ((onGround && direction.x != Mathf.Sign(velocity.x)) || direction.x == -Mathf.Sign(velocity.x) || (Mathf.Abs(velocity.x) < moveSpeed && direction.x != 0)) {
		if (onGround || direction.x == -Mathf.Sign(velocity.x) || (Mathf.Abs(velocity.x) < moveSpeed && direction.x != 0)) {
			velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		}

		//on land (we may want camera shake here later maybe?)
		/*if (onGround && !last_onGround) {
			//Debug.Log("landed");
			if (lastVelocity.magnitude > moveSpeed && direction.x != 0) {
				velocity.x += direction.x * Mathf.Abs(lastVelocity.y) * slideRatio;
			}
		}*/

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

			GetComponent<ProjectileManager>().Shoot();
		}

		if (startSlide && onGround) {
			changeState(PlayerState.Slide);
		}

		//apply final velocity to rigidbody
		body.velocity = velocity;
	}

	//slide state variables
	[SerializeField] float slideSpd;
	[SerializeField] float slide_stopSpd;

	[SerializeField] float slideDirectionBuffer;
	float slideReverseTime;
	private void UpdateSlide() {
		direction.x = (keysPressed.right ? 1 : 0) - (keysPressed.left ? 1 : 0);
	}
	private void FixedUpdateSlide() {
		velocity = body.velocity;

		//apply friction
		velocity.x -= friction * Mathf.Sign(velocity.x) * Time.deltaTime;

		//reverse slide direction on buffered input
		if (Time.time-slideReverseTime < slideDirectionBuffer && direction.x == -Mathf.Sign(velocity.x)) {
			velocity.x *= -1;
			slideReverseTime = -1;
		}

		//exit slide
		if (!keysPressed.down || !onGround || Mathf.Abs(velocity.x) < slide_stopSpd) {
			changeState(PlayerState.Normal);
		}

		//apply final velocity to rigidbody
		body.velocity = velocity;
	}

	// function does nothing
	private void UpdateDead()
	{
	}
	//function also does nothing but this time its on ~physics updates~ instead of frames
	private void FixedUpdateDead() {
	}
}
