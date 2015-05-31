using UnityEngine;
using System.Collections;

public class Throwing : MonoBehaviour {
	public int maxAmmo = 4;
	public int throwSpeed = 200;
	public int ammo;
	private float lastThrowTime = 0f;
	public float minThrowDelay = 0.5f;
	
	public GameObject myKunai;
	public GrappleManager grappleManager;
	public PhysicsPlayerMovement playerMovement;
	
	public GameObject kunaiPrefab;
	public GameObject starPrefab;

	// Use this for initialization
	void Start () {
		grappleManager = GetComponent<GrappleManager>();
		playerMovement = GetComponent<PhysicsPlayerMovement>();
		ammo = maxAmmo;
	}

	public void InitShurikens(int Max) {
		maxAmmo = Max;
		ammo = maxAmmo;
	}
	
	// Update is called once per frame
	void Update ()
	{
		bool canThrow = Time.time > lastThrowTime + minThrowDelay;
	
		if(Input.GetButtonDown("Fire1") && ammo > 0 && canThrow)
		{
			Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			throwVector = throwVector - transform.position;
			playerMovement.AnimateThrow(throwVector);
			throwStar(throwVector);
		}
		
		if(Input.GetButtonDown("Fire2") && canThrow)
		{
			if(!grappleManager.grappleIsOut && grappleManager.heldRopeSegments != 0 && canThrow)
			{
				Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				throwVector = throwVector - transform.position;
				throwVector.Normalize();
				throwGrapple(throwVector.x, throwVector.y);
				playerMovement.AnimateThrow(throwVector);
				grappleManager.InitializeGrapple(myKunai.GetComponent<Kunai>());
			}
			else if(grappleManager.grappleIsOut)
			{
				grappleManager.RetractGrapple();
			}
		}
		
//		if(Input.GetButtonUp("Fire2"))
//		{
//			grappleManager.RetractGrapple();
//		}
	}

	void throwStar (Vector3 dir) {
		--ammo;
		if (GetComponent<NetworkView>().isMine)
		{
			dir.Normalize();
			GameObject newStar = (GameObject)Network.Instantiate(starPrefab, transform.position + dir*7, Quaternion.identity, 0);
			newStar.GetComponent<Rigidbody2D>().velocity = dir*throwSpeed;
			newStar.GetComponent<ID>().teamID = GetComponent<ID>().teamID;
			newStar.GetComponent<ID>().playerNumber = GetComponent<ID>().playerNumber;
		}
	}

	void throwGrapple (float xDir, float yDir)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			Vector3 dir = new Vector3(xDir, yDir, 0.1f);
			myKunai = (GameObject)Network.Instantiate(kunaiPrefab, transform.position + dir * 7, Quaternion.identity, 0);
			myKunai.GetComponent<Rigidbody2D>().velocity = dir * throwSpeed * 2;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
			myKunai.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			myKunai.GetComponent<ID>().teamID = GetComponent<ID>().teamID;
			myKunai.GetComponent<Kunai>().playerNumber = GetComponent<ID>().playerNumber;
		}
	}

	[RPC]
	public void pickupStar ()
	{
		++ammo;
	}

	public void refill() {
		ammo = maxAmmo;
	}
}
