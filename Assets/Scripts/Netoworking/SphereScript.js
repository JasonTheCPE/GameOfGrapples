#pragma strict

function OnTriggerEnter() {
	Debug.Log("Hello");
	var newCol:Vector3 = Vector3(1,0,0);
	GetComponent(NetworkView).RPC("SetColor", RPCMode.AllBuffered, newCol);
}

function OnTriggerExit() {
	Debug.Log("Goodbye");
	var newCol:Vector3 = Vector3(0,0,1);
	GetComponent(NetworkView).RPC("SetColor", RPCMode.AllBuffered, newCol);
}

@RPC
function SetColor(newColor:Vector3) {
	GetComponent(Renderer).material.color = Color(newColor.x, newColor.y, newColor.z, 1);
}