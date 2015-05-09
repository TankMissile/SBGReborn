using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	int hp, maxHp;
	Player player;
	public GameObject heartObject;
	ArrayList hearts = new ArrayList();
	
	// Update is called once per frame
	void Update () {
		if(GameManager.getPlayer() == null) return;
		player = GameManager.getPlayer ();
		
		
		int[] hvals = player.getHP();
		if(hvals[1] != maxHp){
			maxHp = hvals[1];
			foreach(GameObject heart in hearts){
				Destroy (heart);
			}
			hearts.Clear();
			for(int i = 0; i < hvals[1]; i+=4){
				GameObject newHeart  = (GameObject)GameObject.Instantiate(heartObject, transform.position + 7 * i * Vector3.right, Quaternion.identity);
				newHeart.transform.SetParent(transform);
				hearts.Add(newHeart);
			}
		}
		
		if(hvals[0] != hp){
			hp = hvals[0];
			foreach(GameObject heart in hearts){
				heart.SendMessage("fill", hvals[0]);
				hvals[0] -= 4;
			}
		}
	}
}
