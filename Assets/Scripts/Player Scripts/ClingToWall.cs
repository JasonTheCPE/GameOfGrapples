using UnityEngine;
using System.Collections;

public class ClingToWall : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	private short thingsColliding;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
		thingsColliding = 0;
	}
	
	void Update()
	{
		if(thingsColliding > 0)
		{
			parentPlayer.canClingToWall = true;
			--thingsColliding;
		}
		else
		{
			parentPlayer.canClingToWall = false;
		}
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			thingsColliding = 2;
		}
	}
	
	void OnTriggerStay2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			thingsColliding = 2;
		}
	}
}
