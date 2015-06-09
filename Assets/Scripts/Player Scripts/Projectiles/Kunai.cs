using UnityEngine;
using System.Collections;

public class Kunai : MonoBehaviour
{
	public bool isStuck = false;
	public bool ropeIntact = true;
	public bool beingRetracted = false;
	public bool turnTowardsVelocity = true;
	public int playerNumber;
	
	private Collider2D stickInWallCollider;
	private Collider2D lethalCollider;
	private Transform lastFrame;
	
	// Use this for initialization
	void Start ()
	{
		//Debug.Log("Kunai Born!");
		GetComponent<ThrowingAudio>().PlayThrowSFX();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(turnTowardsVelocity)
		{
			TurnWithMotion();
		}
	}
	
	private void TurnWithMotion()
	{
		Vector2 dir = GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
		lastFrame = transform;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	[RPC]
	void SelfDestruct()
	{
		Destroy(gameObject);
	}

	[RPC]
	public void GetStuck()
	{
		GetComponent<ThrowingAudio>().PlayHitWallSFX();
		GetComponentInParent<Kunai>().isStuck = true;
		if(lastFrame != null)
		{
			transform.rotation = lastFrame.rotation;
		}
		else
		{
			TurnWithMotion();
		}
		GetComponentInParent<Kunai>().turnTowardsVelocity = false;
		GetComponentInParent<Rigidbody2D>().isKinematic = true;
	}
}
