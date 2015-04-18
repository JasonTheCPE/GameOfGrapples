#pragma strict

var speed: float = 20;
var maxSpeed: float = 50.0;
var jump: float = 240;
var ammo: int = 1;
var throwSpeed = 200;
var playerID = 0;

private var rb: Rigidbody2D;
private var isJumping: short = 0;
private var facingRight = true;

var ropePrefab:GameObject;
var starPrefab:GameObject;

function Start() {
	rb = GetComponent(Rigidbody2D);
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		var curVel: Vector2 = rb.velocity;
		var movement = Input.GetAxis("Horizontal") * speed;
		
		if (movement < 0)
			facingRight = false;
		else
			facingRight = true;
		
		curVel.x = movement + curVel.x;
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
	} else {
		enabled = false;
	}
	
	if(Input.GetButtonDown("Fire1") && ammo > 0){
		--ammo;
		//throwStar(Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0));
        GetComponent(NetworkView).RPC("throwStar", RPCMode.All, Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0));
	}
}

function OnTriggerEnter2D (other : Collider2D) {
	//Debug.Log("A trigger!");
	if(other.tag == "Tiles" && isJumping > 0) {
		Debug.Log("Hit floor");
		isJumping = 0;
	}
}

@RPC
function throwStar (dir:Vector3) {
	Debug.Log("Mousex: " + dir.x + " Mousey: " + dir.y);
	//TODO adjust rotation of new stars
	if (GetComponent(NetworkView).isMine) {
		dir.Normalize();
		var newStar = Network.Instantiate(starPrefab, transform.position + dir*7, transform.rotation, 0);
		newStar.GetComponent(Rigidbody2D).velocity = dir*throwSpeed;
		newStar.GetComponent(Shuriken).playerID = playerID;
	}
}

@RPC
function pickupStar () {
	++ammo;
}

@RPC
function Die() {
	Destroy(gameObject);
}