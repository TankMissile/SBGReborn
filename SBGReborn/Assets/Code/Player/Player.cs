using UnityEngine;
using System.Collections;

public class Player : Entity {

	public float groundAccel = .5f;
	public float groundDecel = .5f;
	public float airAccel = .1f;

	public float jumpSpeed = 5f;
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
			//rb.velocity = new Vector2(Input.GetAxis("Horizontal") * maxSpeed, rb.velocity.y, 0);
			if(Input.GetAxis("Horizontal") < 0 && rb.velocity.x > maxSpeed * Input.GetAxis ("Horizontal")){ //pressing "left" key & not at max speed
				rb.velocity += Vector2.right * -groundAccel;
			}
			else if(Input.GetAxis("Horizontal") > 0 && rb.velocity.x < maxSpeed * Input.GetAxis ("Horizontal")){ //pressing "right" key & not at max speed
				rb.velocity += Vector2.right * groundAccel;
			}
			else if(rb.velocity.x < -groundDecel) {
				rb.velocity += Vector2.right * groundDecel;
			}
			else if(rb.velocity.x > groundDecel) {
				rb.velocity += Vector2.right * -groundDecel;
			}
			else {
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
			
			if(rb.velocity.x < -maxSpeed){
				rb.velocity += new Vector2(groundDecel, 0);
				if (rb.velocity.x > -maxSpeed) rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
			}
			else if (rb.velocity.x > maxSpeed){
				rb.velocity += new Vector2(-groundDecel, 0);
				if (rb.velocity.x > maxSpeed) rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
			}
			
			if(canJump && Input.GetButtonDown ("Jump")){
				airborne = true;
				rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			}
			break;
		case MoveState.AIRBORNE:
			if(crouching && Input.GetButtonDown("Jump")){
				mstate = MoveState.AIRDROP;
				
			}
			else if(canDoubleJump && Input.GetButtonDown ("Jump")){
				rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
				canDoubleJump = false;
			}
			break;
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
	
	void OnCollisionEnter2D(Collision2D coll){
		if(mstate != MoveState.GROUND ){//&& coll.contacts[0].point.y >= this.transform.position.y + this.transform.localScale.y/2 - .1){
			Debug.Log ("Hit ground");
			mstate = MoveState.GROUND;
		}
		else if(mstate == MoveState.AIRBORNE //freefalling
				&& coll.contacts[0].point.x >= this.transform.position.x + this.transform.localScale.x/2 - .05 //point is on the rightmost edge of SBG
				|| coll.contacts[0].point.x <= this.transform.position.x - this.transform.localScale.x/2 + .05){ //point is on the leftmost edge of SBG
			Debug.Log ("Climbing Wall");
			mstate = MoveState.WALLCLIMB;
		}
		canDoubleJump = true;
	}
	
	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag.Equals("GrabbableSurface") && mstate != MoveState.GROUND) mstate = MoveState.WALLCLIMB;
	}
}
