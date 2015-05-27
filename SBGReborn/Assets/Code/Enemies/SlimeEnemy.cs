using UnityEngine;
using System.Collections;

public class SlimeEnemy : EnemyEntity {

	public float range = 5f; //how far it can travel from starting position
	float startpos;
	float targetpos;
	float idlecooldown = 1f;
	public float minIdleTime = 1f;
	
	int moveDirection = 0;
	
	public enum MoveState{ IDLE, MOVING };
	public MoveState mstate = MoveState.IDLE;
	
	// Use this for initialization
	new void Start () {
		base.Start();
		startpos = rb.position.x;
		targetpos = startpos;
	}
	
	// Update is called once per frame
	void Update () {
		if(rb.velocity.x > 0 && !facingRight){
			flip();
		}
		else if(rb.velocity.x < 0 && facingRight){
			flip ();
		}
	
		switch(mstate){
		case MoveState.IDLE:
			rb.velocity = Vector2.zero;
			idlecooldown -= Time.deltaTime;
			if(idlecooldown <=0){
				if(Random.Range(0f,1f) > .5f){
					startMoving();
				}
				else {
					stopMoving();
				}
			}
			break;
		case MoveState.MOVING:
			//check if arriving at destination and stop
			float newpos = rb.velocity.x * Time.deltaTime + rb.position.x;
			if(moveDirection > 0 && newpos >= targetpos || moveDirection < 0 && newpos <= targetpos){
				stopMoving ();
			}
		
			//move toward destination
			if(rb.position.x > targetpos){
				rb.velocity = transform.right * -maxSpeed;
				if(facingRight) flip ();
			}
			else if(rb.position.x < targetpos){
				rb.velocity = transform.right * maxSpeed;
				if(!facingRight) flip ();
			}
			break;
		}
	}
	
	private void startMoving(){
		targetpos = Random.Range(startpos - this.range, startpos + this.range);
		mstate = MoveState.MOVING;
		if(targetpos > rb.position.x) moveDirection = 1;
		else if(targetpos < rb.position.x) moveDirection = -1;
	}
	
	private void stopMoving(){
		rb.position = new Vector2(targetpos, rb.position.y);
		rb.velocity = new Vector2(0, rb.velocity.y);
		idlecooldown = minIdleTime;
		mstate = MoveState.IDLE;
		moveDirection = 0;
	}
}
