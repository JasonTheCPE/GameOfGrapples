using UnityEngine;
using System.Collections;

public class Throwing : MonoBehaviour {
	public const int maxAmmo = 4;
	public int throwSpeed = 200;
	public int ammo;
	public GameObject myKunai;

	public GrappleManager grappleManager;
	public GameObject kunaiPrefab;
	public GameObject starPrefab;

	// Use this for initialization
	void Start () {
		grappleManager = GetComponent<GrappleManager>();
		ammo = maxAmmo;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && ammo > 0)
		{
			Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			throwVector = throwVector - transform.position;

			throwStar(throwVector);
		}
		
		if(Input.GetButtonDown("Fire2"))
		{
			if(!grappleManager.grappleIsOut)
			{
				Vector3 throwVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				throwVector = throwVector - transform.position;
				throwVector.Normalize();
				throwGrapple(throwVector.x, throwVector.y);
				grappleManager.InitializeGrapple(myKunai.GetComponent<Kunai>());
			}
			else
			{
				grappleManager.RetractGrapple();
			}
		}
	}

	void throwStar (Vector3 dir) {//TODO - optimize: Vector3 (12 bytes) -> float, float (8 bytes)
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
	void pickupStar ()
	{
		++ammo;
	}

	public void refill() {
		ammo = maxAmmo;
	}
}
