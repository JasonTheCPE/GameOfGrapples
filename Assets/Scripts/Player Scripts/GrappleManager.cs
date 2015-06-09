using UnityEngine;
using System.Collections;

public class GrappleManager : MonoBehaviour
{
	private const float pullInDistanceMultiplier = 12;
	private const float outOfRopeKunaiDrag = 0.1f;
	private const float outOfRopeKunaiMass = 8f;
	private const float pullInDivisorBase = 4f;
	private const float pullInDivisorStep = 0.1f;
	private const float pullInKunaiDrag = 8f;
	private const float pullInKunaiMass = 0.1f;
	private const float pullInRopeDrag = 0.8f;
	private const float ropeSpawnDistanceRatio = 6f;
	private const float playerFrequencyOnRetract = 16;

	public int maxRopeSegments = 50;
	//public int ropeSegmentsInRope;
	public int heldRopeSegments;
	public bool grappleIsOut = false;
	public bool beingRetracted = false;
	public bool grappleMassReduced;
	public bool ropeConnectedToPlayer;
	
	public float ropeSegSize;
	
	public Kunai kunai;
	public GameObject lastRopeSegment;
	
	public GameObject ropePrefab;
	
	void Start()
	{
		heldRopeSegments = maxRopeSegments;
	}
	
	public void InitializeGrapple(Kunai newKunai)
	{
		if(heldRopeSegments == 0)
		{
			kunai.GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
			return;
		}
	
		kunai = newKunai;
		grappleIsOut = true;
		grappleMassReduced = false;
		ropeConnectedToPlayer = false;
		lastRopeSegment = (GameObject)Network.Instantiate(ropePrefab, transform.position, Quaternion.identity, 0);
		lastRopeSegment.GetComponent<RopeDestruction>().kunai = kunai;
		SpringJoint2D jointSpr = lastRopeSegment.GetComponent<SpringJoint2D>();
		DistanceJoint2D jointDis = lastRopeSegment.GetComponent<DistanceJoint2D>();
		jointSpr.connectedBody = kunai.GetComponent<Rigidbody2D>();
		jointDis.connectedBody = jointSpr.connectedBody;
		Transform ropeBindPoint = kunai.transform.FindChild("RopeBindPoint");
		jointSpr.connectedAnchor = new Vector2(ropeBindPoint.localPosition.x, ropeBindPoint.localPosition.y);
		jointDis.connectedAnchor = jointSpr.connectedAnchor;
		ropeSegSize = lastRopeSegment.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		jointSpr.distance = ropeSegSize;
		jointDis.distance = ropeSegSize;
		
	}
	
	public void RetractGrapple()
	{
		if(kunai != null && kunai.ropeIntact)
		{
			kunai.isStuck = false;
			kunai.beingRetracted = true;
			kunai.turnTowardsVelocity = false;
			kunai.GetComponent<Rigidbody2D>().isKinematic = false;
			//kunai.GetComponentInChildren<KunaiInnerCollide>().gameObject.SetActive(false);
			//kunai.GetComponentInChildren<KunaiOuterCollide>().gameObject.SetActive(false);
		}
		
		beingRetracted = true;
		if(!ropeConnectedToPlayer)
		{
			ConnectRopeToPlayer();
		}
		if(!grappleMassReduced)
		{
			ReduceGrappleMass();
		}
	}
	
	private void ReduceGrappleMass()
	{
		//Debug.Log("ReduceGrappleMass");
		SpringJoint2D joint = GetComponent<SpringJoint2D>().connectedBody.GetComponent<SpringJoint2D>();
		Rigidbody2D next;
		
		float divisor = pullInDivisorBase;
		
		while(joint != null && joint.connectedBody != null)
		{
			next = joint.connectedBody;
			
			if(next.gameObject == kunai.gameObject)
			{
				next.mass = pullInKunaiMass;
				next.drag = pullInKunaiDrag;
				next.velocity = new Vector2(0, 0);
				grappleMassReduced = true;
				return;
			}
			next.mass /= divisor;
			next.drag = pullInRopeDrag;
			next.velocity = new Vector2(0, 0);
			joint.frequency /= divisor;
			divisor += pullInDivisorStep;
			
			joint = joint.connectedBody.GetComponent<SpringJoint2D>();
		}
		grappleMassReduced = true;
	}
	
	private void AddToRope()
	{
		var dir = lastRopeSegment.transform.position - gameObject.transform.position;
		float distanceLeft = dir.magnitude;
		
		dir = dir.normalized;
		float distanceBetweenSpawnedSegs = ropeSegSize * ropeSpawnDistanceRatio;
		Rigidbody2D lastRopeSegRb;
		while(heldRopeSegments > 0 && distanceLeft > distanceBetweenSpawnedSegs)
		{
			GameObject newRope = (GameObject)Network.Instantiate(ropePrefab, lastRopeSegment.transform.position - dir * ropeSegSize, Quaternion.identity, 0);
			newRope.GetComponent<RopeDestruction>().kunai = kunai;
			lastRopeSegRb = lastRopeSegment.GetComponent<Rigidbody2D>();
			SpringJoint2D jointSpr = newRope.GetComponent<SpringJoint2D>();
			jointSpr.connectedBody = lastRopeSegRb;
			jointSpr.distance = ropeSegSize;
			DistanceJoint2D jointDis = newRope.GetComponent<DistanceJoint2D>();
			jointDis.connectedBody = lastRopeSegRb;
			jointDis.distance = ropeSegSize;
			lastRopeSegment = newRope;
			
			distanceLeft -= distanceBetweenSpawnedSegs;
			--heldRopeSegments;
		}
	}
	
	public void LetOutRope()
	{
		Debug.Log("LOR?");
		if(heldRopeSegments > 0) //&& lastRopeSegment != null)
		{
			Debug.Log("Letting out rope");
			Rigidbody2D lastRopeSegRb = lastRopeSegment.GetComponent<Rigidbody2D>();;
	
			GameObject newRope = (GameObject)Network.Instantiate(ropePrefab, gameObject.transform.position, Quaternion.identity, 0);
			newRope.GetComponent<RopeDestruction>().kunai = kunai;
			SpringJoint2D jointSpr = newRope.GetComponent<SpringJoint2D>();
			jointSpr.connectedBody = lastRopeSegRb;
			jointSpr.distance = ropeSegSize;
			DistanceJoint2D jointDis = newRope.GetComponent<DistanceJoint2D>();
			jointDis.connectedBody = lastRopeSegRb;
			jointDis.distance = ropeSegSize;
			lastRopeSegment = newRope;
			
			SpringJoint2D playerSprJoint = GetComponent<SpringJoint2D>();
			playerSprJoint.enabled = true;
			playerSprJoint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
			playerSprJoint.distance = 0;
			
			DistanceJoint2D playerDisJoint = GetComponent<DistanceJoint2D>();
			playerDisJoint.enabled = true;
			playerDisJoint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
			playerDisJoint.distance = 0;
			
			--heldRopeSegments;
		}
	}
	
	private void ConnectRopeToPlayer()
	{
		//Debug.Log("ConnectRopeToPlayer");
		SpringJoint2D playerSprJoint = GetComponent<SpringJoint2D>();
		playerSprJoint.enabled = true;
		playerSprJoint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
		
		SpringJoint2D lastInRope = lastRopeSegment.GetComponent<SpringJoint2D>();
		playerSprJoint.distance = 0;
		playerSprJoint.dampingRatio = lastInRope.dampingRatio;
		playerSprJoint.frequency = lastInRope.frequency;
		
		DistanceJoint2D playerDisJoint = GetComponent<DistanceJoint2D>();
		playerDisJoint.enabled = true;
		playerDisJoint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
		playerDisJoint.distance = 0;
		
		kunai.GetComponent<Rigidbody2D>().mass = outOfRopeKunaiMass;
		kunai.turnTowardsVelocity = false;
		kunai.GetComponent<Rigidbody2D>().drag = outOfRopeKunaiDrag;
		
		if(kunai.isStuck)
		{
			GetComponent<PhysicsPlayerMovement>().jumpsUsed = 0;
		}
		ropeConnectedToPlayer = true;
	}
	
	public void PullInRope()
	{	
		if(!ropeConnectedToPlayer && kunai.isStuck && kunai.ropeIntact)
		{
			ConnectRopeToPlayer();
		}
		
		SpringJoint2D playersJoint = GetComponent<SpringJoint2D>();
		Rigidbody2D grapplePiece = playersJoint.connectedBody;
		Rigidbody2D toDestroy;
		int pulledIn = 1;
		
		if(grapplePiece == null)
		{
			grappleIsOut = false;
			beingRetracted = false;
			return;
		}
		
		if((grapplePiece.position - GetComponent<Rigidbody2D>().position).magnitude > ropeSegSize * pullInDistanceMultiplier)
		{
			return;
		}
		
		toDestroy = grapplePiece;
		if(kunai != null && grapplePiece == kunai.GetComponent<Rigidbody2D>()) //pulling in the kunai not a rope segment
		{
			pulledIn -= 2;
		}
		else
		{
			grapplePiece = grapplePiece.GetComponent<SpringJoint2D>().connectedBody;
		}
		toDestroy.GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
		//Destroy(toDestroy.gameObject);
		
		if(grapplePiece != null)
		{
			playersJoint.connectedBody = grapplePiece;
			lastRopeSegment = grapplePiece.gameObject;
		}
		
		heldRopeSegments += pulledIn;
	}
		
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(grappleIsOut)
		{
			if(beingRetracted)
			{
				PullInRope();
			}
			else
			{
				if(kunai.ropeIntact)
				{
					if(!kunai.isStuck)
					{
						if(heldRopeSegments > 0)
						{
							AddToRope();
						}
						else
						{
							RetractGrapple();
						}
					}
					
					if(kunai.isStuck && !ropeConnectedToPlayer)
					{
						if(!ropeConnectedToPlayer)
						{
							ConnectRopeToPlayer();
						}
					}
				}
				else
				{
					beingRetracted = true;
				}
			}
		}
	}
}
