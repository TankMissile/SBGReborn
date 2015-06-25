using UnityEngine;
using System.Collections;

public class Player : Entity
{

	public float groundAccel = .5f;
	public float groundDecel = .5f;
	public float airAccel = .25f;
	public float airDecel = 0f;
	
	public float jumpSpeed = 13f;
	public float superJumpSpeed = 11f;
	public float airdropSpeed = 10f;
	public float slideFallSpeed = 10f;
	
	public bool canDoubleJump = true;
	bool crouching = false;
	public bool jump = false;
	
	int knockbackDuration = 10; //decremented in FixedUpdate, so uses frames rather than seconds
	int knockbackTimer = 0;
	MeshRenderer meshrend;
	
	int deathDuration = 20;
	
	public Material[] tex = new Material[2]; //0 is default, 1 is hurt
	
	private bool softAttack = false, hardAttack = false;
	
	//Handle checking for ground collision
	public bool grounded = false;
	public Transform groundCheck;
	Vector3 groundBoxCorners = new Vector3 (.32f, .05f, 0);
	public LayerMask whatIsGround;
	
	//Handle checking for wall collision
	public bool walled = false;
	public Transform wallCheck;
	public Vector3 wallBoxCorners = new Vector3 (.05f, .4f, 0);
	public LayerMask whatIsWall;
	public int lastWall = 0;
	
	//Determine how to handle movement
	public enum MoveState{ GROUND, WALLCLIMB, AIRBORNE, LEDGEHANG, AIRDROP, KNOCKBACK, DEAD, SOFTATTACK, HARDATTACK, WALLJUMP };
	public MoveState mstate = MoveState.GROUND;
	
	//Attacks
	public int chargeDur = 10; //duration of dash attack (in FixedUpdate calls)
	int chargeRem; //remaining time on dash attack
	public int chargeSpeed = 10;

	// Use this for initialization
	new void Start ()
	{
		base.Start ();
		meshrend = GetComponent<MeshRenderer>();
		GameManager.setPlayer (this);
	}
    
	//Do input here
	void Update ()
	{
		if (Input.GetButtonDown ("Crouch") && !crouching) {
			setCrouch (true);
		} else if (Input.GetButtonUp ("Crouch") && crouching) {
			setCrouch (false);
		}
        
		if (Input.GetButtonDown("Jump"))
			jump = true;
			
		if(Input.GetButtonDown("Fire1")){
			softAttack = true;
			chargeRem = chargeDur;
		}
		else if(Input.GetButtonDown("Fire2")){
			hardAttack = true;
		}
	}
	
	//Do physics here
	void FixedUpdate ()
	{
		//check if touching the ground and/or a wall
		grounded = Physics2D.OverlapArea (groundCheck.position + groundBoxCorners, groundCheck.position - groundBoxCorners, whatIsGround);
		walled = Physics2D.OverlapArea (transform.position + wallBoxCorners, transform.position - wallBoxCorners, whatIsWall);
		
		if(softAttack && mstate != MoveState.KNOCKBACK){
			hardAttack = softAttack = false;
			mstate = MoveState.SOFTATTACK;
		}
		else if(hardAttack && mstate != MoveState.KNOCKBACK){
			hardAttack = softAttack = false;
			mstate = MoveState.HARDATTACK;
		}
		else if(mstate != MoveState.KNOCKBACK && mstate != MoveState.WALLJUMP && mstate != MoveState.DEAD && mstate != MoveState.SOFTATTACK){
			if (grounded) { //if on ground, enter grounded state
				mstate = MoveState.GROUND;
				lastWall = 0;
				canDoubleJump = true;
			}
			else if(crouching && !grounded && mstate == MoveState.GROUND){ //if ran off a ledge while crouching, slide down the wall
				flip ();
				rb.velocity = -Vector2.up * maxSpeed/2 + Vector2.right;
				mstate = MoveState.WALLCLIMB;
			}
			else if (walled && !grounded) { //if touching a wall and not on the ground, enter wallslide state
				if(facingRight && lastWall != 1 || !facingRight && lastWall != -1){
					if(rb.velocity.y > 0 && mstate != MoveState.WALLCLIMB){
						if(rb.velocity.y < jumpSpeed)
							rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
					}
					mstate = MoveState.WALLCLIMB;
					canDoubleJump = false;
				}
			}
			else if(mstate == MoveState.WALLCLIMB && !walled && rb.velocity.y > 0){ //If wallsliding and ran off the top of the wall, run on top of it
				float modifier = Mathf.Abs (Input.GetAxis("Horizontal"));
				if(facingRight){
					if(!Input.GetButton("Crouch"))
						rb.velocity = new Vector3(maxSpeed * modifier, jumpSpeed * (1-modifier/2));
					else rb.velocity = new Vector3(maxSpeed * modifier, jumpSpeed * (1-modifier) / 2);
					mstate = MoveState.AIRBORNE;
				}
				else{
					if(!Input.GetButton ("Crouch"))
						rb.velocity = new Vector3(-maxSpeed * modifier, jumpSpeed * (1-modifier/2));
					else rb.velocity = new Vector3(-maxSpeed * modifier, jumpSpeed * (1-modifier) / 2);
					mstate = MoveState.AIRBORNE;
				}
			}
			else if( !grounded && mstate != MoveState.AIRDROP){ //if airborne and not using air drop, enter airborne state
				mstate = MoveState.AIRBORNE;
			}
		}
		else if(mstate == MoveState.WALLJUMP){
			if (grounded) { //if on ground, enter grounded state
				mstate = MoveState.GROUND;
				canDoubleJump = true;
			}
			else if (walled && !grounded) { //if touching a wall and not on the ground, enter wallslide state
				if(rb.velocity.y > 0 && mstate != MoveState.WALLCLIMB){
					if(rb.velocity.y < jumpSpeed)
						rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
				}
				mstate = MoveState.WALLCLIMB;
				canDoubleJump = false;
			}
		}
    
		switch (mstate) {
		case MoveState.GROUND:
			getHorizontalVelocity (groundAccel, groundDecel);
			
			if (jump) {
				mstate = MoveState.AIRBORNE;
				if (!crouching) {
					rb.velocity = new Vector2 (rb.velocity.x, jumpSpeed);
				} else {
					setCrouch (false);
					rb.velocity = new Vector2 (rb.velocity.x, superJumpSpeed);
					canDoubleJump = false;
				}
				jump = false;
			}
			break;
		case MoveState.AIRBORNE:
			getHorizontalVelocity (airAccel, airDecel);
			if (jump) {
				if (crouching) {
					mstate = MoveState.AIRDROP;
					rb.velocity = new Vector2 (0, -airdropSpeed);
					canDoubleJump = false;
				} else if (canDoubleJump) {
					if (Input.GetAxis ("Horizontal") == 0)
						rb.velocity = new Vector2 (rb.velocity.x, jumpSpeed);
					else
						rb.velocity = new Vector2 (Input.GetAxis ("Horizontal") * maxSpeed, jumpSpeed);
					canDoubleJump = false;
				}
				jump = false;
			}
			break;
		case MoveState.WALLJUMP:
			if(Input.GetButton("Horizontal")){
				getHorizontalVelocity (airAccel, airDecel);
				mstate = MoveState.AIRBORNE;
			}
			
			if (jump) {
				if (crouching) {
					mstate = MoveState.AIRDROP;
					rb.velocity = new Vector2 (0, -airdropSpeed);
					canDoubleJump = false;
				} else if (canDoubleJump) {
					if (Input.GetAxis ("Horizontal") == 0)
						rb.velocity = new Vector2 (rb.velocity.x, jumpSpeed);
					else
						rb.velocity = new Vector2 (Input.GetAxis ("Horizontal") * maxSpeed, jumpSpeed);
					canDoubleJump = false;
				}
				jump = false;
			}
			break;
		case MoveState.WALLCLIMB:
			
			if(jump){
				flip ();
				if(facingRight){
					lastWall = -1;
					rb.velocity = new Vector2(maxSpeed, jumpSpeed);
					mstate = MoveState.WALLJUMP;
					walled = false;
				}
				else {
					lastWall = 1;
					rb.velocity = new Vector3(-maxSpeed, jumpSpeed);
					mstate = MoveState.WALLJUMP;
					walled = false;
				}
				jump = false;
			}
			
			if(!crouching && rb.velocity.y < -slideFallSpeed){
				rb.velocity = new Vector2(rb.velocity.x, -slideFallSpeed);
			}
			else if(crouching && rb.velocity.y < -slideFallSpeed/2){
				rb.velocity = new Vector2(rb.velocity.x, -slideFallSpeed/2);
			}
			
			bool wasRight = facingRight;
			getHorizontalVelocity(airAccel, airDecel);
			
			//if no longer facing the wall, count as a wall jump
			if(wasRight != facingRight){
				if(facingRight) lastWall = -1;
				else lastWall = 1;
			}
			
			
			break;
		case MoveState.KNOCKBACK:
			knockbackTimer--;
			if(knockbackTimer <=0){
				if(tex[0] !=null && meshrend != null) meshrend.material = tex[0];
				mstate = MoveState.AIRBORNE;
			}
			break;
		case MoveState.DEAD:
			knockbackTimer--;
			if(knockbackTimer <= -2){
				mstate = MoveState.AIRBORNE;
			}
			else if(knockbackTimer <=0){
				transform.position = GameManager.getSpawnPoint();
				if(tex[0] !=null && meshrend != null) meshrend.material = tex[0];
				hp = maxhp;
			}
			break;
		case MoveState.SOFTATTACK:
			chargeRem--;
			if(chargeRem >0){
				gameObject.tag = "FriendlyAttack";
				if(facingRight){
					rb.velocity = chargeSpeed * Vector2.right;
				}
				else {
					rb.velocity = -chargeSpeed * Vector2.right;
				}
			}
			else{
				gameObject.tag = "Player";
				mstate = MoveState.AIRBORNE;
			}
			break;
		case MoveState.HARDATTACK:
		
			break;
		}
		
	}
	
	void OnTriggerEnter2D (Collider2D coll){
		if(!tag.Equals("FriendlyAttack")) return;
		
		coll.gameObject.SendMessage("checkHitDirection", rb.position);
		coll.gameObject.SendMessage("takeDamage", impactDamage);
	}
	
	void checkHitDirection(Vector3 enemyPos){
		if(enemyPos.x < rb.position.x && facingRight) flip ();
		else if(enemyPos.x > rb.position.x && !facingRight) flip ();
	}
	
	new void takeDamage(int dmg){
		if(mstate == MoveState.DEAD) return;
		base.takeDamage(dmg);
		
		if(facingRight){
			rb.velocity = new Vector2(-maxSpeed/2, jumpSpeed/5);
		}
		else{
			rb.velocity = new Vector2(maxSpeed/2, jumpSpeed/5);
		}
		
		if(tex[1] != null && meshrend != null) {
			meshrend.material = tex[1];
		}
		knockbackTimer = knockbackDuration;
		mstate = MoveState.KNOCKBACK;
		
		if(hp <=0){
			knockbackTimer = deathDuration;
			mstate = MoveState.DEAD;
		}
	}
	
	void getHorizontalVelocity (float accel, float decel)
	{
		float hmove = Input.GetAxis ("Horizontal");
		if (hmove < 0 && rb.velocity.x > maxSpeed * hmove) { //pressing "left" key & not at max speed
			rb.velocity += Vector2.right * -accel;
		} else if (hmove > 0 && rb.velocity.x < maxSpeed * hmove) { //pressing "right" key & not at max speed
			rb.velocity += Vector2.right * accel;
		} else if (rb.velocity.x < -groundDecel) {
			rb.velocity += Vector2.right * decel;
		} else if (rb.velocity.x > groundDecel) {
			rb.velocity += Vector2.right * -decel;
		} else {
			rb.velocity = new Vector2 (0, rb.velocity.y);
		}
		
		if (!crouching) {
			if (rb.velocity.x < -maxSpeed) {
				rb.velocity += new Vector2 (decel, 0);
				if (rb.velocity.x > -maxSpeed)
					rb.velocity = new Vector2 (-maxSpeed, rb.velocity.y);
			} else if (rb.velocity.x > maxSpeed) {
				rb.velocity += new Vector2 (-decel, 0);
				if (rb.velocity.x > maxSpeed)
					rb.velocity = new Vector2 (maxSpeed, rb.velocity.y);
			}	
		} else {
			if (rb.velocity.x < -maxSpeed / 2) {
				rb.velocity += new Vector2 (decel, 0);
				if (rb.velocity.x > -maxSpeed / 2)
					rb.velocity = new Vector2 (-maxSpeed / 2, rb.velocity.y);
			} else if (rb.velocity.x > maxSpeed / 2) {
				rb.velocity += new Vector2 (-decel, 0);
				if (rb.velocity.x > maxSpeed / 2)
					rb.velocity = new Vector2 (maxSpeed / 2, rb.velocity.y);
			}
		}
		
		if(rb.velocity.x > .1f && !facingRight || rb.velocity.x < -.1f && facingRight){
			flip();
		}
	}
	
	void setCrouch (bool crouch)
	{
		if (crouch) {
			crouching = true;
			transform.localScale = new Vector3 (transform.localScale.x, .5f * transform.localScale.y, transform.localScale.z);
			transform.position -= transform.up * transform.localScale.y / 2;
		} else {
			crouching = false;
			transform.position += transform.up * transform.localScale.y / 2;
			transform.localScale = new Vector3 (transform.localScale.x, 2f * transform.localScale.y, transform.localScale.z);
		}
	}
}
