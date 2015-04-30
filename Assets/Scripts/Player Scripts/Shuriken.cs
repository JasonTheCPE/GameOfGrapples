using UnityEngine;
using System.Collections;

public class Shuriken : MonoBehaviour {

	public bool isActive = true;
	public static float spinAmount = 16.0f;

	// Use this for initialization
	void Start () {
		//Debug.Log("Hello");
	}
	
	// Update is called once per frame
	void Update () {
		if(!Mathf.Approximately(GetComponent<Rigidbody2D>().velocity.x, 0f) || !Mathf.Approximately(GetComponent<Rigidbody2D>().velocity.y, 0f))
		{
			transform.Rotate(new Vector3(0, 0, spinAmount * -Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x)));
		}
		
	}

	void OnTriggerEnter2D (Collider2D other)  {
		if(other.tag == "Tiles") {
			isActive = false;
			GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
		} else if (other.tag == "Player") {
			if (isActive) {
				int otherID = GetComponent<Team>().teamID;
				if (otherID == -1 || other.gameObject.GetComponent<Team>().teamID != otherID) {
					other.gameObject.GetComponent<NetworkView>().RPC("Die", RPCMode.All);
					GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
					Debug.Log("Shuriken's playerID: " + GetComponent<Team>().teamID + " killed " + other.gameObject.GetComponent<Team>().teamID);
				}
			} else {
				other.gameObject.GetComponent<NetworkView>().RPC("pickupStar", RPCMode.All);
				GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
			}
		}
	}

	//This needs to be RPC
	[RPC]
	void SelfDestruct() {
		Destroy(gameObject);
	}
}
