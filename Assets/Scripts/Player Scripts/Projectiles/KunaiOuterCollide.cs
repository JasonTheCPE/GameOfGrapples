using UnityEngine;
using System.Collections;

public class KunaiOuterCollide : MonoBehaviour
{
	Kunai kunai;
	ID id;
	ThrowingAudio audio;
	
	void Start()
	{
		kunai = GetComponentInParent<Kunai>();
		id = GetComponentInParent<ID>();
		audio = GetComponentInParent<ThrowingAudio>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(kunai.isStuck == false && kunai.beingRetracted == false)
		{
			if(other.tag == "Tile")
			{
				GetComponentInParent<Rigidbody2D>().velocity *= 0.5f;
			}
			else if(other.tag == "Player")// || other.tag == "OtherDestructible")
			{
				int myTeam = id.teamID;
				int myNumber = id.playerNumber;
				ID otherID = other.gameObject.GetComponent<ID>();
				int otherTeam = otherID.teamID;
				int otherPlayerNumber = otherID.playerNumber;
					
				if (myNumber != otherPlayerNumber)
				{
					if(myTeam == -1 || myTeam != otherTeam)
					{
						audio.PlayHitPlayerSFX();
						Debug.Log("Kunai's playerID: " + myNumber + " hit " + otherPlayerNumber);
						other.gameObject.GetComponent<NetworkView>().RPC("GetHurt", RPCMode.All);
						GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
					}
				}
				
			}
			//Debug.Log("Kunai outer collider triggered: " + other.tag);
		}
	}
	
	void OnTriggerStay2D (Collider2D other)
	{
		if(other.tag == "Tile")
		{
			GetComponentInParent<Rigidbody2D>().velocity *= 0.5f;
		}
	}
}
