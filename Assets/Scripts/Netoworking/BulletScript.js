#pragma strict
var bulletspeed = 5;

function Update () {
	var direction:Vector3 = Vector3(0, bulletspeed * Time.deltaTime, 0);
	GetComponent(NetworkView).RPC("Move", RPCMode.AllBuffered, direction);
}

@RPC
function Move(dir:Vector3) {
	GetComponent(CharacterController).Move(dir);
}