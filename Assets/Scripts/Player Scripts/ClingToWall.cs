using UnityEngine;
using System.Collections;

public class ClingToWall : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	private int thingsSlidingDown;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
		thingsSlidingDown = 0;
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			++thingsSlidingDown;
			parentPlayer.canClingToWall = true;
		}
	}
	
	void OnTriggerStay2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			parentPlayer.canClingToWall = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			if(--thingsSlidingDown == 0)
			{
				parentPlayer.canClingToWall = false;
			}
		}
	}
}
