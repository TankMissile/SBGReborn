using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectibleMeter : MonoBehaviour {

	private Text txt;
	
	// Use this for initialization
	void Start () {
		txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		txt.text = GameManager.getCollectiblesObtained() + "/" + GameManager.getTotalCollectibles();
	}
}
