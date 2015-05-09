using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class HeartImage : MonoBehaviour{
	public Sprite full0, full25, full50, full75, full100;
	public Image img;
	
	public void fill(int full){
		if(full > 0){
			switch(full){
			case 0:
				img.sprite = full100;
				break;
			case 1:
				img.sprite = full25;
				break;
			case 2:
				img.sprite = full50;
				break;
			case 3:
				img.sprite = full75;
				break;
			default:
				img.sprite = full100;
				break;
			}
		}
		else{
			img.sprite = full0;
		}
	}
	
}
