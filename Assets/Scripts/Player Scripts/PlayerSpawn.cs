using UnityEngine;
using System.Collections;

public class PlayerSpawn
{
	private const float respawnInvincibilityTime = 1.5f;
	
	public static void SpawnPlayer(GameObject player, Vector3 location)
	{
		Throwing throwing = player.GetComponent<Throwing>();
		Health health = player.GetComponent<Health>();
		
		throwing.ammo = throwing.maxAmmo;
		health.BeInvincible(respawnInvincibilityTime);
		player.transform.position = location;
	}
}
