using UnityEngine;
using System.Collections;

public class GrappleManager : MonoBehaviour
{
	private const float pullInDistanceMultiplier = 16;
	private const float outOfRopeKunaiDrag = 0.1f;
	private const float outOfRopeKunaiMass = 8f;
	private const float pullInDivisorBase = 1f;
	private const float pullInDivisorStep = 0.1f;
	private const float pullInKunaiDrag = 4f;
	private const float pullInKunaiMass = 0.2f;
	private const float pullInRopeDrag = 2f;

	public int ropeSegments = 50;
	public bool grappleIsOut = false;
	public bool beingRetracted = false;
	
	public float ropeSegSize;
	
	public Kunai kunai;
	public GameObject lastRopeSegment;
	
	public GameObject ropePrefab;
	
	public void InitializeGrapple(Kunai newKunai)
	{
		if(ropeSegments == 0)
		{
			kunai.GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
			//Destroy(kunai.gameObject);
			return;
		}
	
		kunai = newKunai;
		grappleIsOut = true;
		Debug.Log(ropePrefab);
		Debug.Log(kunai);
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
		
		ReduceGrappleMass();
		beingRetracted = true;
	}
	
	private void ReduceGrappleMass()
	{
		SpringJoint2D joint = GetComponent<SpringJoint2D>();
		Rigidbody2D next;
		
		float divisor = pullInDivisorBase;
		
		while(joint != null && joint.connectedBody != null)
		{
			next = joint.connectedBody;
			
			if(next.gameObject == kunai.gameObject)
			{
				next.mass = pullInKunaiMass;
				next.drag = pullInKunaiDrag;
				return;
			}
			next.mass /= divisor;
			next.drag = pullInRopeDrag;
			joint.frequency /= divisor;
			divisor += pullInDivisorStep;
			
			joint = joint.connectedBody.GetComponent<SpringJoint2D>();
		}
	}
	
	private void AddToRope()
	{
		//Debug.Log("Adding to rope");
		var dir = lastRopeSegment.transform.position - gameObject.transform.position;
		float distanceLeft = dir.magnitude;
		dir = dir.normalized;
		
		if(!kunai.isStuck)
		{
			while(ropeSegments > 0 && distanceLeft > ropeSegSize)
			{
				GameObject newRope = (GameObject)Network.Instantiate(ropePrefab, lastRopeSegment.transform.position - dir * ropeSegSize, Quaternion.identity, 0);
				newRope.GetComponent<RopeDestruction>().kunai = kunai;
				SpringJoint2D jointSpr = newRope.GetComponent<SpringJoint2D>();
				jointSpr.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
				jointSpr.distance = ropeSegSize;
				DistanceJoint2D jointDis = newRope.GetComponent<DistanceJoint2D>();
				jointDis.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
				jointDis.distance = ropeSegSize;
				lastRopeSegment = newRope;
				
				distanceLeft -= ropeSegSize;
				--ropeSegments;
			}
		}
		
		if(ropeSegments == 0 || kunai.isStuck)
		{
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
		}
		
		
	}
	
	private void PullInRope()
	{	
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
			--pulledIn;
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
		}
		
		ropeSegments += pulledIn;
	}
		
	// Update is called once per frame
	void Update ()
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
					AddToRope();
				}
				else
				{
					beingRetracted = true;
				}
			}
		}
	}
}
