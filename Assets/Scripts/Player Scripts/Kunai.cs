using UnityEngine;
using System.Collections;

public class Kunai : MonoBehaviour
{
	
	public bool hasStuck = false;
	
	// Use this for initialization
	void Start ()
	{
		//Debug.Log("Kunai Born!");
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 dir = GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			//Debug.Log("Dead Kunai");
			hasStuck = true;
			GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
		}
		else if (other.tag == "Player")
		{
			if (!hasStuck)
			{
				if (other.gameObject.GetComponent<Team>().teamID != GetComponent<Team>().teamID)
				{
					other.gameObject.GetComponent<NetworkView>().RPC("Die", RPCMode.All);
					GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
					Debug.Log("Kunai's playerID: " + GetComponent<Team>().teamID + " killed " + other.gameObject.GetComponent<Team>().teamID);
				}
			}
			else
			{
				//other.gameObject.GetComponent<NetworkView>().RPC("pickupStar", RPCMode.All);
				//GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
			}
		}
	}
	
	//TODO this may not need to be destroyed as RPC
	[RPC]
	void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
