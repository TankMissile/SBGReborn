using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class Entity : MonoBehaviour {
	public int maxhp=20;
	protected int hp;
	public int maxSpeed=10;
	
	public Rigidbody2D rb;
	
	protected void Start(){
		rb = GetComponent<Rigidbody2D>();
		hp = maxhp;
	}
}
