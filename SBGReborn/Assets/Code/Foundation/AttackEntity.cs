using UnityEngine;
using System.Collections;

public class AttackEntity : Entity {

	void OnCollision2D(Collider2D coll){
		if(coll.tag.Equals("Enemy")){
			coll.SendMessage("takeDamage", impactDamage);
		}
	}
}
