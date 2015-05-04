using UnityEngine;
using System.Collections;

public class Kunai : MonoBehaviour
{
	public bool isStuck = false;
	public bool ropeIntact = true;
	public bool beingRetracted = false;
	public int playerNumber;
	
	private Collider2D stickInWallCollider;
	private Collider2D lethalCollider;
	
	// Use this for initialization
	void Start ()
	{
		//Debug.Log("Kunai Born!");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!beingRetracted && !isStuck)
		{
			Vector2 dir = GetComponent<Rigidbody2D>().velocity;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Tiles")
		{
			
		}
		else if (other.tag == "Player")
		{
			if (!isStuck)
			{
				
			}
			else
			{
				
			}
		}
	}
}
