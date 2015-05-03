using UnityEngine;
using System.Collections;

public static class GameManager {

	private static Player player;
	
	public static Player getPlayer(){
		return player;
	}
	public static void setPlayer(Player p){
		player = p;
	}
}
