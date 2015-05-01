using UnityEngine;
using System.Collections;

public class KunaiOuterCollide : MonoBehaviour
{
	void OnTriggerEnter2D (Collider2D other)
	{
		if(GetComponentInParent<Kunai>().isStuck == false)
		{
			if(other.tag == "Player")
			{
				int myTeam = GetComponentInParent<Team>().teamID;
				int otherTeam = other.gameObject.GetComponent<Team>().teamID;
				int otherPlayerNumber = other.gameObject.GetComponent<PlayerMovement>().playerNumber;
				int playerNumber = GetComponentInParent<Kunai>().playerNumber;
				
				if (playerNumber != otherPlayerNumber && (myTeam == -1 || myTeam != otherTeam))
				{
					other.gameObject.GetComponent<NetworkView>().RPC("Die", RPCMode.All);
					Debug.Log("Kunai from playerID: " + playerNumber + " killed " + otherPlayerNumber);
				}
			}
			else if(other.tag == "OtherDestructible")
			{
			
			}
			//Debug.Log("Kunai outer collider triggered: " + other.tag);
		}
	}
}
