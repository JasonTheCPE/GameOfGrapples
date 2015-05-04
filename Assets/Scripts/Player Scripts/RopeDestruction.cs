using UnityEngine;
using System.Collections;

public class RopeDestruction : MonoBehaviour
{
	public Kunai kunai;
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Weapon" && other.gameObject != kunai.gameObject)
		{
			kunai.ropeIntact = false;
			Destroy(gameObject);
		}
	}
}
