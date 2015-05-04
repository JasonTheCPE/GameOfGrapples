using UnityEngine;
using System.Collections;

public class GrappleManager : MonoBehaviour
{
	private const int pullInRate = 1;

	public int ropeSegments = 30;
	//public float pulledIn = 0f;
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
			Destroy(kunai.gameObject);
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
		if(kunai.ropeIntact)
		{
			kunai.isStuck = false;
			kunai.beingRetracted = beingRetracted = true;
			kunai.GetComponent<Rigidbody2D>().mass = 5;
			kunai.GetComponent<Rigidbody2D>().isKinematic = false;
		}
		else
		{
			beingRetracted = true;
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
		}
	}
	
	private void PullInRope()
	{
		SpringJoint2D playersJoint = GetComponent<SpringJoint2D>();
		Rigidbody2D grapplePiece = playersJoint.connectedBody;
		Rigidbody2D toDestroy;
		int pulledIn = 0;
		
		while(grapplePiece != null && pulledIn <= pullInRate)
		{
			toDestroy = grapplePiece;
			if(grapplePiece == kunai.GetComponent<Rigidbody2D>())
			{
				--pulledIn;
			}
			else
			{
				grapplePiece = grapplePiece.GetComponent<SpringJoint2D>().connectedBody;
			}
			Destroy(toDestroy.gameObject);
			++pulledIn;
		}
		
		if(grapplePiece == null)
		{
			grappleIsOut = false;
		}
		else
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
