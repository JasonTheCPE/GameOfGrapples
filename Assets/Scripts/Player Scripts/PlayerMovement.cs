﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{	
	public float speed = 20;
	public float maxSpeed = 50.0f;
	public float jump = 240;

	private Rigidbody2D rb;
	private short isJumping = 0;
	private bool facingRight = true;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<NetworkView>().isMine)
		{
			Vector2 curVel = rb.velocity;
			var movement = Input.GetAxis("Horizontal") * speed;
			
			if (movement < 0)
				facingRight = false;
			else
				facingRight = true;
			
			curVel.x = Mathf.Clamp(curVel.x + movement, -maxSpeed, maxSpeed);
			
			if (Input.GetKeyDown("space"))
			{
				if (isJumping < 2)
				{
					if (isJumping == 1)
						curVel.y += jump/2;	//ensure shitty double jump
					else
						curVel.y += jump;
					++isJumping;
				}
			}
			rb.velocity = curVel;
		}
		else
		{
			enabled = false;
		}
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		//Debug.Log("A trigger!");
		if((other.tag == "Tiles" || other.tag == "Player") && isJumping > 0)
		{
			Land();
		}
	}

	[RPC]
	void SelfDestruct() {
		Die();
	}

	[RPC]
	public void Die() {
		GetComponent<Throwing>().refill();
		rb.velocity = new Vector3(0,0,0);
		rb.position = new Vector3(0, 0, 0);
		//Destroy(gameObject);
	}
	
	void Land()
	{
		isJumping = 0;
	}
	
}