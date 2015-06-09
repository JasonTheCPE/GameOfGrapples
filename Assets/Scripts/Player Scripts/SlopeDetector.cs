using UnityEngine;
using System.Collections;

public class SlopeDetector : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	private int thingsInFront;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
		thingsInFront = 0;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			parentPlayer.hasTileInFront = true;
			++thingsInFront;
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			parentPlayer.hasTileInFront = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			if(--thingsInFront == 0)
			{
				parentPlayer.hasTileInFront = false;
			}
		}
	}
}
