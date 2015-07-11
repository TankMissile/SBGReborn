using UnityEngine;
using System.Collections;

public static class GameManager {

	private static Vector3 respawnPoint;
	private static Player player;
	private static int collectiblesAttained;
	private static int totalCollectibles = 5;
	
	public static Player getPlayer(){
		return player;
	}
	public static void setPlayer(Player p){
		player = p;
	}
	
	public static void setSpawnPoint(Vector3 pos){
		respawnPoint = pos;
	}
	public static Vector3 getSpawnPoint(){
		return respawnPoint;
	}
	
	public static void addCollectible(){
		collectiblesAttained++;
	}
	public static int getCollectiblesObtained(){
		return collectiblesAttained;
	}
	
	public static int getTotalCollectibles(){
		return totalCollectibles;
	}
}
