var speed: int = 5;
var gravity = 5;
private var cc: CharacterController;

function Start() {
	cc = GetComponent(CharacterController);
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		cc.Move(Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
	} else {
		enabled = false;
	}
}