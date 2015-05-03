using UnityEngine;
using System.Collections;

public class GrappleManager : MonoBehaviour
{
	public uint ropeSegments = 50;
	public bool grappleIsOut = false;
	public bool beingRetracted = false;
	public Kunai kunai;
	public GameObject lastRopeSegment;
	
	public GameObject ropePrefab;
	
	public void InitializeGrapple(Kunai newKunai)
	{
		kunai = newKunai;
		grappleIsOut = true;
	}
	
	public void RetractGrapple()
	{
		if(kunai.isPullable)
		{
			kunai.isStuck = kunai.beingRetracted = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!kunai.isStuck)
		{
			
		}
		else
		{
		
		}
	}
}
