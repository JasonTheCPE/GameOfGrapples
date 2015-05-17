using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public int MaxHealth = 1;
	public int health = 1;

	void Start () {
		health = MaxHealth;
	}

	public int GetHit(int damage) {
		health -= damage;
		if (health <= 0) {
			health = 0;
		}

		return health;
	}

	public int Heal (int restore) {
		health += restore;

		if (health > MaxHealth) {
			health = MaxHealth;
		}

		return health;
	}

	public void InitHealth(int Max) {
		MaxHealth = Max;
		health = MaxHealth;
	}

	[RPC]
	public void GetHurt()
	{
		int health = GetHit(1);
		GetComponent<Throwing>().pickupStar();
		if(GetComponent<NetworkView>().isMine == true) {
			Referee reff = GameObject.Find("Ingame Manager").GetComponent<Referee>();
			if (reff)
			{
				reff.GetComponent<NetworkView>().RPC("KillPlayer", RPCMode.All, Network.player);
			}
			
			if (health == 0) {
				GetComponent<NetworkView>().RPC("Die", RPCMode.All);
			}
		}
	}
}
