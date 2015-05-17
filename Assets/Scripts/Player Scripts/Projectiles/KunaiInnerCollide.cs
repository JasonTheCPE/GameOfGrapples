using UnityEngine;
using System.Collections;

public class KunaiInnerCollide : MonoBehaviour
{
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles" && GetComponentInParent<Kunai>().beingRetracted == false)
		{
			GetComponentInParent<Kunai>().GetComponent<NetworkView>().RPC("GetStuck", RPCMode.All);
		}
		//Debug.Log("Kunai inner collider triggered: " + other.tag);
	}
}
