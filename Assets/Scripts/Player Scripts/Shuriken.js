#pragma strict

var isActive = true;
var playerID;

function OnTriggerEnter2D (other : Collider2D)  {
	if(other.tag == "Tiles") {
		Debug.Log("Dead Shuriken");
		isActive = false;
		GetComponent(Rigidbody2D).velocity = Vector3(0,0,0);
	} else if (other.tag == "Player") {
		if (isActive && other.gameObject.GetComponent(PlayerMovement).playerID != playerID) {
			other.gameObject.GetComponent(NetworkView).RPC("die", RPCMode.All);
			//other.gameObject.GetComponent(PlayerMovement).Die();
			Destroy(gameObject);
		} else {
			other.gameObject.GetComponent(NetworkView).RPC("pickupStar", RPCMode.All);
			//other.gameObject.GetComponent(PlayerMovement).pickupStar();
			Destroy(gameObject);
		}
	}
}
