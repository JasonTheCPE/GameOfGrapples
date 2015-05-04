using UnityEngine;
using System.Collections;

public class GrappleManager : MonoBehaviour
{
	public uint ropeSegments = 30;
	public bool grappleIsOut = false;
	public bool beingRetracted = false;
	
	public float ropeSegSize;
	
	public Kunai kunai;
	public GameObject lastRopeSegment;
	
	public GameObject ropePrefab;
	
	public void InitializeGrapple(Kunai newKunai)
	{
		kunai = newKunai;
		grappleIsOut = true;
		lastRopeSegment = (GameObject)Network.Instantiate(ropePrefab, transform.position, Quaternion.identity, 0);
		lastRopeSegment.GetComponent<RopeDestruction>().kunai = kunai;
		SpringJoint2D joint = lastRopeSegment.GetComponent<SpringJoint2D>();
		joint.connectedBody = kunai.GetComponent<Rigidbody2D>();
		Transform ropeBindPoint = kunai.transform.FindChild("RopeBindPoint");
		joint.connectedAnchor = new Vector2(ropeBindPoint.localPosition.x, ropeBindPoint.localPosition.y);
		ropeSegSize = lastRopeSegment.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		joint.distance = ropeSegSize;
		
	}
	
	public void RetractGrapple()
	{
		if(kunai.ropeIntact)
		{
			kunai.isStuck = false;
			kunai.beingRetracted = beingRetracted = true;
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
				SpringJoint2D joint = newRope.GetComponent<SpringJoint2D>();
				joint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
				joint.distance = ropeSegSize;
				lastRopeSegment = newRope;
				
				distanceLeft -= ropeSegSize;
				--ropeSegments;
			}
		}
		
		if(ropeSegments == 0 || kunai.isStuck)
		{
			
			SpringJoint2D playerRopeJoint = GetComponent<SpringJoint2D>();
			playerRopeJoint.enabled = true;
			playerRopeJoint.connectedBody = lastRopeSegment.GetComponent<Rigidbody2D>();
			
			SpringJoint2D lastInRope = lastRopeSegment.GetComponent<SpringJoint2D>();
			playerRopeJoint.distance = 0;
			playerRopeJoint.dampingRatio = lastInRope.dampingRatio;
			playerRopeJoint.frequency = lastInRope.frequency;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(grappleIsOut)
		{
			if(beingRetracted)
			{
				
			}
			else
			{
				if(kunai.ropeIntact)
				{
					AddToRope();
				}
			}
		}
	}
}
