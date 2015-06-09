using UnityEngine;
using System.Collections;

public class JumpRefresh : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	private int thingsStandingOn;
	private bool wasJustOn = false;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
		thingsStandingOn = 0;
	}
	
	void Update()
	{
		if(wasJustOn)
		{
			wasJustOn = false;
		}
		else
		{
			if(thingsStandingOn > 0)
			{
				--thingsStandingOn;
			}
			else
			{
				parentPlayer.LeftGround();
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			wasJustOn = true;
			parentPlayer.TouchedGround();
			if(thingsStandingOn < 3)
			{
				++thingsStandingOn;
			}
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			wasJustOn = true;
			parentPlayer.TouchedGround();
			if(thingsStandingOn < 3)
			{
				++thingsStandingOn;
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
//		if(other.tag == "Tiles" || other.tag == "Player")
//		{
//			if(thingsStandingOn > 0)
//			{
//				--thingsStandingOn;
//			}
//			if(thingsStandingOn == 0)
//			{
//				parentPlayer.LeftGround();
//			}
//		}
	}
	
}
