﻿using UnityEngine;
using System.Collections;

public class KunaiInnerCollide : MonoBehaviour
{
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles" && GetComponentInParent<Kunai>().beingRetracted == false)
		{
			GetComponentInParent<Kunai>().isStuck = true;
			GetComponentInParent<Kunai>().turnTowardsVelocity = false;
			GetComponentInParent<Rigidbody2D>().isKinematic = true;
		}
		//Debug.Log("Kunai inner collider triggered: " + other.tag);
	}
}
