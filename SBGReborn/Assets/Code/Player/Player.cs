using UnityEngine;
using System.Collections;

public class Player : Entity {

	public float groundAccel = .5f;
	public float groundDecel = .5f;
	public float airAccel = .25f;
	public float airDecel = 0f;

	public float jumpSpeed = 7f;
	public float superJumpSpeed = 11f;
	public float airdropSpeed = 10f;
	
	public bool canJump = true;
	public bool canDoubleJump = true;
	public bool airborne = false;
	public bool crouching = false;
	
	public enum MoveState { GROUND, WALLCLIMB, AIRBORNE, LEDGEHANG, AIRDROP, UNMOVABLE };
	public MoveState mstate = MoveState.GROUND;

	// Use this for initialization
	new void Start () {
		base.Start();
		
		GameManager.setPlayer(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	
		if(Input.GetButtonDown("Crouch") && !crouching){
			setCrouch(true);
		}
		else if(Input.GetButtonUp("Crouch") && crouching){
			setCrouch (false);
		}
	
		switch(mstate){
		case MoveState.GROUND:
			getHorizontalVelocity(groundAccel, groundDecel);
			
			if(canJump && Input.GetButtonDown ("Jump")){
				mstate = MoveState.AIRBORNE;
				if(!crouching){
					rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
				}
				else {
					setCrouch(false);
					rb.velocity = new Vector2(rb.velocity.x, superJumpSpeed);
					canDoubleJump = false;
				}
			}
			break;
		case MoveState.AIRBORNE:
			getHorizontalVelocity(airAccel, airDecel);
		
			if(crouching && Input.GetButtonDown("Jump")){
				mstate = MoveState.AIRDROP;
				rb.velocity = new Vector2(0, -airdropSpeed);
				canDoubleJump = false;
			}
			else if(canDoubleJump && Input.GetButtonDown ("Jump")){
				if(Input.GetAxis("Horizontal") == 0)
					rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
				else
					rb.velocity = new Vector2(Input.GetAxis("Horizontal") * maxSpeed, jumpSpeed);
				canDoubleJump = false;
			}
			break;
		}
	}
	
	void getHorizontalVelocity(float accel, float decel){
		if(Input.GetAxis("Horizontal") < 0 && rb.velocity.x > maxSpeed * Input.GetAxis ("Horizontal")){ //pressing "left" key & not at max speed
			rb.velocity += Vector2.right * -accel;
		}
		else if(Input.GetAxis("Horizontal") > 0 && rb.velocity.x < maxSpeed * Input.GetAxis ("Horizontal")){ //pressing "right" key & not at max speed
			rb.velocity += Vector2.right * accel;
		}
		else if(rb.velocity.x < -groundDecel) {
			rb.velocity += Vector2.right * decel;
		}
		else if(rb.velocity.x > groundDecel) {
			rb.velocity += Vector2.right * -decel;
		}
		else {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
		
		if(!crouching){
			if(rb.velocity.x < -maxSpeed){
				rb.velocity += new Vector2(decel, 0);
				if (rb.velocity.x > -maxSpeed) rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
			}
			else if (rb.velocity.x > maxSpeed){
				rb.velocity += new Vector2(-decel, 0);
				if (rb.velocity.x > maxSpeed) rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
			}
		}
		else {
			if(rb.velocity.x < -maxSpeed/2){
				rb.velocity += new Vector2(decel, 0);
				if (rb.velocity.x > -maxSpeed/2) rb.velocity = new Vector2(-maxSpeed/2, rb.velocity.y);
			}
			else if (rb.velocity.x > maxSpeed/2){
				rb.velocity += new Vector2(-decel, 0);
				if (rb.velocity.x > maxSpeed/2) rb.velocity = new Vector2(maxSpeed/2, rb.velocity.y);
			}
		}
	}
	
	void setCrouch(bool crouch){
		if(crouch){
			crouching = true;
			transform.localScale = new Vector3(transform.localScale.x, .5f * transform.localScale.y, transform.localScale.z);
			transform.position -= transform.up * transform.localScale.y/2;
		}
		else{
			crouching = false;
			transform.position += transform.up * transform.localScale.y/2;
			transform.localScale = new Vector3(transform.localScale.x, 2f * transform.localScale.y, transform.localScale.z);
		}
	}
	
	new protected void OnCollisionEnter2D(Collision2D coll){
		base.OnCollisionEnter2D(coll);
	
		if(mstate != MoveState.GROUND && coll.gameObject.tag.Equals ("Terrain")){
			//Debug.Log ("Hit ground");
			mstate = MoveState.GROUND;
			canDoubleJump = true;
		}
		
	}
	new protected void OnCollisionExit2D(Collision2D coll){
		base.OnCollisionExit2D(coll);
		if(collisionCount == 0){
			mstate = MoveState.AIRBORNE;
			if(crouching)
				rb.velocity = new Vector2(0, -jumpSpeed/2);
		}
	}
	
	protected void OnTriggerEnter2D(Collider2D coll){
		
		if(coll.gameObject.tag.Equals("GrabbableSurface") && mstate != MoveState.GROUND) mstate = MoveState.WALLCLIMB;
	}
}
