using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class Entity : MonoBehaviour {
	public int maxhp=20;
	protected int hp;
	public int maxSpeed=10;
	
	public int collisionCount = 0;
	
	public Rigidbody2D rb;
	
	protected void Start(){
		rb = GetComponent<Rigidbody2D>();
		hp = maxhp;
	}
	
	protected void OnCollisionEnter2D(Collision2D coll){
		if(coll.gameObject.tag.Equals ("Terrain")){
			collisionCount++;
		}
	}
	protected void OnCollisionExit2D(Collision2D coll){
		//Debug.Log ("Collision Exited");
		if(coll.gameObject.tag.Equals ("Terrain")){
			collisionCount--;
		}
	}
}
