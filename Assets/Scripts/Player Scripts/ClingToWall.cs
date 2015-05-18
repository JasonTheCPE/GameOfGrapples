using UnityEngine;
using System.Collections;

public class ClingToWall : MonoBehaviour
{
	PhysicsPlayerMovement parentPlayer;
	
	void Start()
	{
		parentPlayer = GetComponentInParent<PhysicsPlayerMovement>();
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
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
			parentPlayer.canClingToWall = false;
		}
	}
}
