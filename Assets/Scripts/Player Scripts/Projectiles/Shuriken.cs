using UnityEngine;
using System.Collections;

public class Shuriken : MonoBehaviour
{
	public bool isActive = true;
	public const float spinAmount = 16.0f;

	// Use this for initialization
	void Start ()
	{
		//Debug.Log("Hello");
		GetComponent<ThrowingAudio>().PlayThrowSFX();
	}

	// Update is called once per frame
	void Update ()
	{
		if(!Mathf.Approximately(GetComponent<Rigidbody2D>().velocity.x, 0f) || !Mathf.Approximately(GetComponent<Rigidbody2D>().velocity.y, 0f))
		{
			transform.Rotate(new Vector3(0, 0, spinAmount * -Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x)));
		}
		
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			isActive = false;
			GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
			GetComponent<ThrowingAudio>().PlayHitWallSFX();
		}
		else if (other.tag == "Player")// || other.tag == "OtherDestructible")
		{
			if (isActive)
			{
				int myTeam = GetComponent<ID>().teamID;
				int myNumber = GetComponent<ID>().playerNumber;
				int otherTeam = other.gameObject.GetComponent<ID>().teamID;
				int otherPlayerNumber = other.gameObject.GetComponent<ID>().playerNumber;
				
				if (myNumber != otherPlayerNumber && (myTeam == -1 || myTeam != otherTeam))
				{
					GetComponent<ThrowingAudio>().PlayHitPlayerSFX();
					Debug.Log("Shuriken's playerID: " + myNumber + " killed " + otherPlayerNumber);
					other.gameObject.GetComponent<NetworkView>().RPC("GetHurt", RPCMode.All);
					GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.All);
				}
			}
			else
			{
				GetComponent<ThrowingAudio>().PlayPickupSFX();
				other.gameObject.GetComponent<NetworkView>().RPC("pickupStar", RPCMode.All);
				SelfDestruct();
			}
		}
	}

	[RPC]
	void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
