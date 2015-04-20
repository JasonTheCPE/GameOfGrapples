#pragma strict

var isActive = true;
var playerID = 0;

function OnTriggerEnter2D (other : Collider2D)  {
	if(other.tag == "Tiles") {
		Debug.Log("Dead Shuriken");
		isActive = false;
		GetComponent(Rigidbody2D).velocity = Vector3(0,0,0);
	} else if (other.tag == "Player") {
		if (isActive) {
			if (other.gameObject.GetComponent(PlayerMovement).playerID != playerID) {
				other.gameObject.GetComponent(NetworkView).RPC("Die", RPCMode.All);
				//other.gameObject.GetComponent(PlayerMovement).Die();
				GetComponent(NetworkView).RPC("SelfDestruct", RPCMode.AllBuffered);
			}
		} else {
			other.gameObject.GetComponent(NetworkView).RPC("pickupStar", RPCMode.All);
			//other.gameObject.GetComponent(PlayerMovement).pickupStar();
			GetComponent(NetworkView).RPC("SelfDestruct", RPCMode.AllBuffered);
		}
	}
}

@RPC
function SelfDestruct() {
	Destroy(gameObject);
}
