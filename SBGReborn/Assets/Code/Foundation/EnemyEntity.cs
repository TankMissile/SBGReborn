using UnityEngine;
using System.Collections;

public class EnemyEntity : Entity {

	// Use this for initialization
	new void Start () {
		base.Start();
		
	}
	
	
	
	new void OnCollisionEnter2D(Collision2D coll){
		base.OnCollisionEnter2D(coll);
		if(coll.gameObject.tag.Equals ("Player")){
			coll.gameObject.SendMessage("takeDamage", this.impactDamage);
		}
	}
}
