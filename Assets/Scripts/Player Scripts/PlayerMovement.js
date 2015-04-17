#pragma strict

var speed: float = 20;
var maxSpeed: float = 45.0;
var jump: float = 35;
private var rb: Rigidbody2D;
private var isJumping: short = 0;

var ropePrefab:GameObject;
var starPrefab:GameObject;

function Start() {
	rb = GetComponent(Rigidbody2D);
}

function Update(){
	//if (GetComponent(NetworkView).isMine) {
		var curVel: Vector2 = rb.velocity;
		
		curVel.x = Input.GetAxis("Horizontal") * speed + curVel.x;
		curVel.x = Mathf.Clamp(curVel.x, -maxSpeed, maxSpeed);
		
		if (Input.GetKeyDown("space")) {
			if (isJumping < 2) {
				if (isJumping == 1)
					curVel.y += jump/2;	//ensure shitty double jump
				else
					curVel.y += jump;
				++isJumping;
			}
		}
		rb.velocity = curVel;
		//rb.Move(Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
	/*} else {
		enabled = false;
	}
	
	if(Input.GetMouseButtonDown(0)){
        GetComponent(NetworkView).RPC("shoot", RPCMode.All);
	}*/
}

function OnTriggerEnter2D (other : Collider2D) {
	//Debug.Log("A trigger!");
	if(other.tag == "Tiles" && isJumping > 0) {
		Debug.Log("Hit floor");
		isJumping = 0;
	}
}

@RPC
function throwStar () {
	/*if (GetComponent(NetworkView).isMine) {
		Network.Instantiate(bulletPrefab, transform.position + Vector3(0,1.5,0), Quaternion.identity, 0);
	}*/
}