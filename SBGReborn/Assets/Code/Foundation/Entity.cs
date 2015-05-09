using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class Entity : MonoBehaviour {
	public int maxhp=20;
	public int hp;
	public int maxSpeed=10;
	
	public Rigidbody2D rb;
	
	public int[] getHP(){
		return new int[] { hp, maxhp};
	}
	
	protected void Start(){
		rb = GetComponent<Rigidbody2D>();
		hp = maxhp;
	}
	
	protected void OnCollisionEnter2D(Collision2D coll){
	
	}
	protected void OnCollisionExit2D(Collision2D coll){
	
	}
}
