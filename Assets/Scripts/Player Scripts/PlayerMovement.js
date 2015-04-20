#pragma strict

var speed: float = 20;
var maxSpeed: float = 50.0;
var jump: float = 240;
var maxAmmo: int = 1;
var throwSpeed = 200;

private var rb: Rigidbody2D;
private var isJumping: short = 0;
private var facingRight = true;
private var ammo: int = maxAmmo;
private var col : Vector3;

var ropePrefab:GameObject;
var starPrefab:GameObject;

function Start() {
	rb = GetComponent(Rigidbody2D);
	ammo = maxAmmo;
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		var curVel: Vector2 = rb.velocity;
		var movement = Input.GetAxis("Horizontal") * speed;
		
		if (movement < 0)
			facingRight = false;
		else
			facingRight = true;
		
		curVel.x = Mathf.Clamp(curVel.x + movement, -maxSpeed, maxSpeed);
		
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
		//throwStar(Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0));
        GetComponent(NetworkView).RPC("throwStar", RPCMode.All, Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0));
	}
}

function OnTriggerEnter2D (other : Collider2D) {
	//Debug.Log("A trigger!");
	if((other.tag == "Tiles" || other.tag == "Player") && isJumping > 0) {
		GetComponent(NetworkView).RPC("Land", RPCMode.All);
	}
}

@RPC
function throwStar (dir:Vector3) {
	--ammo;
	//TODO adjust rotation of new stars
	if (GetComponent(NetworkView).isMine) {
		dir.Normalize();
		var newStar = Network.Instantiate(starPrefab, transform.position + dir*7, transform.rotation, 0);
		newStar.GetComponent(Rigidbody2D).velocity = dir*throwSpeed;
		newStar.GetComponent(TeamMember).teamID = GetComponent(TeamMember).teamID;
	}
}

@RPC
function pickupStar () {
	++ammo;
}

@RPC
function Die() {
	ammo = maxAmmo;
	rb.velocity = Vector3(0,0,0);
	rb.position = Vector3(0, 115, 0);
	//Destroy(gameObject);
}

@RPC
function Land() {
	isJumping = 0;
}

@RPC
function setColor(newColor : Vector3) {
	col = newColor;
	GetComponent(Renderer).material.color = Color(newColor.x, newColor.y, newColor.z, 1);
}

private var RemoveUnusedNameSpaceWarningsq:Queue;
private var RemoveUnusedNameSpaceWarningsg:GUI;