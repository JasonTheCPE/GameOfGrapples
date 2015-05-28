using UnityEngine;
using System.Collections;

public class RopeDestruction : MonoBehaviour
{
	public Kunai kunai;
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (GetComponent<NetworkView>().isMine)
		{
			if(other.tag == "Weapon" && other.gameObject != kunai.gameObject)
			{
				Rigidbody2D rbInOther = other.GetComponent<Rigidbody2D>();
				if(rbInOther != null && !rbInOther.isKinematic)
				{
					rbInOther.velocity = rbInOther.velocity * 0.95f;
					kunai.ropeIntact = false;
					GetComponent<NetworkView>().RPC("SelfDestruct", RPCMode.AllBuffered);
				}
			}
		}
	}

	[RPC]
	void SelfDestruct() 
	{ 
		Destroy(gameObject);
	}
}
