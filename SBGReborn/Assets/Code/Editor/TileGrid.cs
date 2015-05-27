using UnityEngine;
using System.Collections;

public class TileGrid : MonoBehaviour {

	float width = 1f, height = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos (){
		Vector3 pos = Camera.current.transform.position;
		
		for(float y = pos.y - 800f; y < pos.y + 800f; y += height){
			Gizmos.DrawLine(new Vector3(-10000000f, Mathf.Floor(y/height) * height, 0), new Vector3(1000000f, Mathf.Floor(y/height) * height, 0));
		}
		
		for (float x = pos.x - 1200f; x < pos.x + 1200f; x += width){
			Gizmos.DrawLine(new Vector3(Mathf.Floor(x/width) * width, -1000000f, 0), new Vector3(Mathf.Floor(x/width) * width, 1000000f, 0));
		}
	}
}
