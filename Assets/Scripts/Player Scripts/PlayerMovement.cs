using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public const int maxAmmo = 4;
	
	public float speed = 20;
	public float maxSpeed = 50.0f;
	public float jump = 240;
	public int throwSpeed = 200;
	private int ammo;
	
	public int playerNumber;
	public int health = 1;
	public GameObject myKunai;
	
	private Rigidbody2D rb;
	private short isJumping = 0;
	private bool facingRight = true;
	
	public GrappleManager grappleManager;
	public GameObject kunaiPrefab;
	public GameObject starPrefab;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		grappleManager = GetComponent<GrappleManager>();
		ammo = maxAmmo;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<NetworkView>().isMine)
		{
			Vector2 curVel = rb.velocity;
			var movement = Input.GetAxis("Horizontal") * speed;
			
			if (movement < 0)
				facingRight = false;
			else
				facingRight = true;
			
			curVel.x = Mathf.Clamp(curVel.x + movement, -maxSpeed, maxSpeed);
			
			if (Input.GetKeyDown("space"))
			{
				if (isJumping < 2)
				{
					if (isJumping == 1)
						curVel.y += jump/2;	//ensure shitty double jump
					else
						curVel.y += jump;
					++isJumping;
				}
			}
			rb.velocity = curVel;
		}
		else
		{
			enabled = false;
		}
		
		if(Input.GetButtonDown("Fire1") && ammo > 0)
		{
			Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			throwVector = throwVector - transform.position;
			
			GetComponent<NetworkView>().RPC("throwStar", RPCMode.All, throwVector);
		}
		
		if(Input.GetButtonDown("Fire2"))
		{
			if(!grappleManager.grappleIsOut)
			{
				Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				throwVector = throwVector - transform.position;
				throwVector.Normalize();
				GetComponent<NetworkView>().RPC("throwGrapple", RPCMode.All, throwVector.x, throwVector.y);
				grappleManager.InitializeGrapple(myKunai.GetComponent<Kunai>());
			}
			else
			{
				grappleManager.RetractGrapple();
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		//Debug.Log("A trigger!");
		if((other.tag == "Tiles" || other.tag == "Player") && isJumping > 0)
		{
			GetComponent<NetworkView>().RPC("Land", RPCMode.All);
		}
	}
	
	[RPC]
	void throwStar (Vector3 dir) {//TODO - optimize: Vector3 (12 bytes) -> float, float (8 bytes)
		--ammo;
		if (GetComponent<NetworkView>().isMine)
		{
			dir.Normalize();
			GameObject newStar = (GameObject)Network.Instantiate(starPrefab, transform.position + dir*7, Quaternion.identity, 0);
			newStar.GetComponent<Rigidbody2D>().velocity = dir*throwSpeed;
			newStar.GetComponent<Team>().teamID = GetComponent<Team>().teamID;
			newStar.GetComponent<Shuriken>().playerNumber = playerNumber;
		}
	}
	
	[RPC]
	void throwGrapple (float xDir, float yDir)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			Vector3 dir = new Vector3(xDir, yDir, 0.1f);
			myKunai = (GameObject)Network.Instantiate(kunaiPrefab, transform.position + dir * 7, Quaternion.identity, 0);
			myKunai.GetComponent<Rigidbody2D>().velocity = dir * throwSpeed * 2;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
			myKunai.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			myKunai.GetComponent<Team>().teamID = GetComponent<Team>().teamID;
			myKunai.GetComponent<Kunai>().playerNumber = playerNumber;
		}
	}
	
	[RPC]
	void pickupStar ()
	{
		++ammo;
	}

	[RPC]
	void SelfDestruct() {
		Die();
	}
	
	[RPC]
	void Die()
	{
		ammo = maxAmmo;
		rb.velocity = new Vector3(0,0,0);
		rb.position = new Vector3(0, 0, 0);
		if(GetComponent<NetworkView>().isMine == true) {
			Referee reff = GameObject.Find("Ingame Manager").GetComponent<Referee>();
			if (reff)
			{
				reff.GetComponent<NetworkView>().RPC("KillPlayer", RPCMode.All, Network.player);
			}
		}
		//Destroy(gameObject);
	}
	
	[RPC]
	void Land()
	{
		isJumping = 0;
	}
	
}