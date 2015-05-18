using UnityEngine;
using System.Collections;

public class JumpRefresh : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Tiles" || other.tag == "Player")
		{
			parentPlayer.TouchedGround();
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
			parentPlayer.LeftGround();
		}
	}
	
}
