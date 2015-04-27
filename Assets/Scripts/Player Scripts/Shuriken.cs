﻿using UnityEngine;
using System.Collections;

public class Shuriken : MonoBehaviour {

	public bool isActive = true;

	// Use this for initialization
	void Start () {
		Debug.Log("Hello");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D other)  {
		if(other.tag == "Tiles") {
			Debug.Log("Dead Shuriken");
			isActive = false;
			GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
		} else if (other.tag == "Player") {
			if (isActive) {
				if (other.gameObject.GetComponent<Team>().teamID != GetComponent<Team>().teamID) {
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

	[RPC]
	void SelfDestruct() {
		Destroy(gameObject);
	}
}