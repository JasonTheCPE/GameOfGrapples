using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed = 20;
	public float maxSpeed = 50.0f;
	public float jump = 240;
	public int maxAmmo = 4;
	public int throwSpeed = 200;
	
	private Rigidbody2D rb;
	private short isJumping = 0;
	private bool facingRight = true;
	private int ammo;
	private Vector3 col;
	
	public GameObject ropePrefab;
	public GameObject starPrefab;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		ammo = maxAmmo;
	}

	// Update is called once per frame
	void Update () {
		if (GetComponent<NetworkView>().isMine) {
			Vector2 curVel = rb.velocity;
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
			GetComponent<NetworkView>().RPC("throwStar", RPCMode.All, new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0));
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		//Debug.Log("A trigger!");
		if((other.tag == "Tiles" || other.tag == "Player") && isJumping > 0) {
			GetComponent<NetworkView>().RPC("Land", RPCMode.All);
		}
	}
	
	[RPC]
	void throwStar (Vector3 dir) {
		--ammo;
		//TODO adjust rotation of new stars
		if (GetComponent<NetworkView>().isMine) {
			dir.Normalize();
			GameObject newStar = (GameObject)Network.Instantiate(starPrefab, transform.position + dir*7, transform.rotation, 0);
			newStar.GetComponent<Rigidbody2D>().velocity = dir*throwSpeed;
			newStar.GetComponent<Team>().teamID = GetComponent<Team>().teamID;
		}
	}
	
	[RPC]
	void pickupStar () {
		++ammo;
	}
	
	[RPC]
	void Die() {
		ammo = maxAmmo;
		rb.velocity = new Vector3(0,0,0);
		rb.position = new Vector3(0, 0, 0);
		//Destroy(gameObject);
	}
	
	[RPC]
	void Land() {
		isJumping = 0;
	}
	
	[RPC]
	void setColor(Vector3 newColor) {
		col = newColor;
		GetComponent<Renderer>().material.color = new Color(newColor.x, newColor.y, newColor.z, 1);
	}
}
