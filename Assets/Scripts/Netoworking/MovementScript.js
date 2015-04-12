var speed: int = 5;
var gravity = 5;
private var cc: CharacterController;

var bulletPrefab:GameObject;

function Start() {
	cc = GetComponent(CharacterController);
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		cc.Move(Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
		if(Input.GetMouseButtonDown(0)){
            GetComponent(NetworkView).RPC("shoot", RPCMode.All);
    	}
	} else {
		enabled = false;
	}
}

@RPC
function shoot () {
	if (GetComponent(NetworkView).isMine) {
		Network.Instantiate(bulletPrefab, transform.position + Vector3(0,1.5,0), Quaternion.identity, 0);
	}
}