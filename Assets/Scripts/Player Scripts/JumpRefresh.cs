using UnityEngine;
using System.Collections;

public class JumpRefresh : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	private int thingsStandingOn;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
		thingsStandingOn = 0;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			parentPlayer.TouchedGround();
			++thingsStandingOn;
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			parentPlayer.TouchedGround();
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			if(--thingsStandingOn == 0)
			{
				parentPlayer.LeftGround();
			}
		}
	}
	
}
