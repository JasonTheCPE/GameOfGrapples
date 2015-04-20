#pragma strict

var isActive = true;

function OnTriggerEnter2D (other : Collider2D)  {
	if(other.tag == "Tiles") {
		Debug.Log("Dead Shuriken");
		isActive = false;
		GetComponent(Rigidbody2D).velocity = Vector3(0,0,0);
	} else if (other.tag == "Player") {
		if (isActive) {
			if (other.gameObject.GetComponent(TeamMember).teamID != GetComponent(TeamMember).teamID) {
				other.gameObject.GetComponent(NetworkView).RPC("Die", RPCMode.All);
				GetComponent(NetworkView).RPC("SelfDestruct", RPCMode.AllBuffered);
				Debug.Log("Shuriken's playerID: " + GetComponent(TeamMember).teamID + " killed " + other.gameObject.GetComponent(TeamMember).teamID);
			}
		} else {
			other.gameObject.GetComponent(NetworkView).RPC("pickupStar", RPCMode.All);
			GetComponent(NetworkView).RPC("SelfDestruct", RPCMode.AllBuffered);
		}
	}
}

@RPC
function SelfDestruct() {
	Destroy(gameObject);
}

private var RemoveUnusedNameSpaceWarningsq:Queue;
private var RemoveUnusedNameSpaceWarningsg:GUI;