﻿using UnityEngine;
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
	
	public bool canDoubleJump = true;
	bool crouching = false;
	public bool jump = false;
	public bool facingRight = true;
	
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
	
	//Determine how to handle movement
	public enum MoveState{ GROUND, WALLCLIMB, AIRBORNE, LEDGEHANG, AIRDROP, IMMOBILE };
	public MoveState mstate = MoveState.GROUND;

	// Use this for initialization
	new void Start ()
	{
		base.Start ();
		
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
	}
	
	//Do physics here
	void FixedUpdate ()
	{
		//check if touching the ground and/or a wall
		grounded = Physics2D.OverlapArea (groundCheck.position + groundBoxCorners, groundCheck.position - groundBoxCorners, whatIsGround);
		walled = Physics2D.OverlapArea (transform.position + wallBoxCorners, transform.position - wallBoxCorners, whatIsWall);
		
		
		if (grounded) {
			mstate = MoveState.GROUND;
			canDoubleJump = true;
		} else if (walled && !grounded) {
			if(rb.velocity.y > 0 && mstate != MoveState.WALLCLIMB){
				if(rb.velocity.y < jumpSpeed)
					rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
			}
			mstate = MoveState.WALLCLIMB;
			canDoubleJump = false;
		}
		else if(mstate == MoveState.WALLCLIMB && !walled && rb.velocity.y > 0){
			if(facingRight){
				rb.velocity = new Vector3(maxSpeed, 0f);
				mstate = MoveState.AIRBORNE;
			}
			else{
				rb.velocity = new Vector3(-maxSpeed, 0f);
				mstate = MoveState.AIRBORNE;
			}
		}
		else if( !grounded && mstate != MoveState.AIRDROP){
			mstate = MoveState.AIRBORNE;
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
		case MoveState.WALLCLIMB:
			if(jump){
				flip ();
				if(facingRight){
					rb.velocity = new Vector2(maxSpeed, jumpSpeed);
					mstate = MoveState.AIRBORNE;
				}
				else {
					rb.velocity = new Vector3(-maxSpeed, jumpSpeed);
					mstate = MoveState.AIRBORNE;
				}
				jump = false;
			}
			break;
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
	
	void flip(){
		facingRight = !facingRight;
		
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
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
