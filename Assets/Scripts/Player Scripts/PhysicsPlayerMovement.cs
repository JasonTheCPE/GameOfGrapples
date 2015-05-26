using UnityEngine;
using System.Collections;

public class PhysicsPlayerMovement : MonoBehaviour
{
	enum playerState
	{
		PlayerIdle = 0, PlayerRunning = 1, PlayerInAir = 2, PlayerMovingInAir = 3,
		PlayerWallCling = 4, PlayerHangingFromRope = 5, PlayerHangingFromRopeMoving = 6,
		PlayerThrowUpwards = 7, PlayerThrowDownwards = 8, PlayerDodgeRoll = 9,
		PlayerDefeat = 10, PlayerVictory = 11, PlayerDeath = 12
	};

	enum playerSounds
	{
		Step = 0, Jump = 1, Hit = 2, Die = 3,
		Victory = 4, Dodge = 5, Land = 6
	};
	
	public float moveForceMultiplier = 350;
	public Vector2 moveForceVector;
	
	public float jumpForceMultiplier = 8000;
	public Vector2 jumpForceVector;
	public short maxJumps = 2;
	public short jumpsUsed = 0;
	
	private Rigidbody2D rb;
	private float playerWeight;
	
	private bool facingRight = true;
	private float minSidewaysMoveAnimationSpeed = 0.5f;
	private Animator animator;
	private playerState lastState;
	
	public bool canClingToWall = false;
	public bool isGrabbingWall = false;
	public bool movementAttempted = false;
	public bool isOnGround = false;
	public bool isMoving = false;
	public bool playerControllable = true;
	
	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		playerWeight = rb.mass;
		jumpForceVector = new Vector2(0.0f, playerWeight * jumpForceMultiplier);
		moveForceVector = new Vector2(playerWeight * moveForceMultiplier, 0.0f);
		animator = GetComponent<Animator>();
		lastState = playerState.PlayerIdle;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GetComponent<NetworkView>().isMine && playerControllable)
		{
			float movement = Input.GetAxis("Horizontal");
			
			if(movement < 0)
			{
				facingRight = false;
				rb.AddForce(-moveForceVector);
				movementAttempted = true;
			}
			else if(movement > 0)
			{
				facingRight = true;
				rb.AddForce(moveForceVector);
				movementAttempted = true;
			}
			else
			{
				movementAttempted = false;
			}
			
			if (Input.GetKeyDown("space"))
			{
				if (jumpsUsed < maxJumps)
				{
					GetComponent<NetworkView>().RPC("PlaySFX", RPCMode.All, (int)playerSounds.Jump);
					if (jumpsUsed > 0)
					{
						if(rb.velocity.y < 0) //double jump will cancel downward momentum
						{
							rb.velocity = new Vector2(rb.velocity.x, 0);
						}
						rb.AddForce(jumpForceVector * 0.5f);
					}
					else
					{
						rb.AddForce(jumpForceVector);
					}
					++jumpsUsed;
				}
			}
			
			if((!facingRight && transform.localScale.x > 0) || (facingRight && transform.localScale.x < 0))
			{
				GetComponent<NetworkView>().RPC("SwitchDirection", RPCMode.All);
			}
			
			//Animator state checks
			if(Mathf.Abs(rb.velocity.x) > minSidewaysMoveAnimationSpeed)
			{
				isMoving = true;
			}
			else
			{
				isMoving = false;
			}
			
			TryClingToWall();
			TriggerAnimationTransition();
		}
		else
		{
			enabled = false;
		}
	}
	
	[RPC]
	void SelfDestruct()
	{
		Die();
	}
	
	[RPC]
	public void Die()
	{
		GetComponent<Throwing>().refill();
		rb.velocity = new Vector3(0, 0, 0);
		rb.position = new Vector3(0, 0, 0);
		PlaySFX((int)playerSounds.Die);
		if(GetComponent<NetworkView>().isMine == true)
		{
			Referee reff = GameObject.Find("Ingame Manager").GetComponent<Referee>();
			if(reff)
			{
				reff.GetComponent<NetworkView>().RPC("KillPlayer", RPCMode.All, Network.player);
			}
		}
		//Destroy(gameObject);
	}
	
	public void TouchedGround()
	{
		jumpsUsed = 0;
		isOnGround = true;
	}
	
	public void LandedHook() {
		if (jumpsUsed > 0) {
			jumpsUsed = 1;
		}
	}
	
	public void LeftGround()
	{
		isOnGround = false;
	}
	
	public void TryClingToWall()
	{
		if(canClingToWall && movementAttempted && !isOnGround)
		{
			isGrabbingWall = true;
			jumpsUsed = 0;
		}
		else
		{
			isGrabbingWall = false;
		}
	}
	
	private void TriggerAnimationTransition()
	{
		playerState thisState;
		
		if(lastState == playerState.PlayerDefeat || lastState == playerState.PlayerVictory)
		{
			return;
		}
		
		if(isOnGround)
		{
			if(isMoving)
			{
				thisState = playerState.PlayerRunning;
			}
			else
			{
				thisState = playerState.PlayerIdle;
			}
		}
		else
		{
			if(isMoving)
			{
				thisState = playerState.PlayerMovingInAir;
			}
			else
			{
				thisState = playerState.PlayerInAir;
				
				if(isGrabbingWall)
				{
					thisState = playerState.PlayerWallCling;
				}
			}
		}
		
		if(thisState != lastState)
		{
			//LaunchAnimation(thisState);
			GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int)thisState);
			lastState = thisState;
		}
	}
	
	public void BeVictorious()
	{
		GetComponent<NetworkView>().RPC("PlaySFX", RPCMode.All, (int)playerSounds.Victory);
		playerControllable = false;
		lastState = playerState.PlayerVictory;
		GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) playerState.PlayerVictory);
	}
	
	public void BeDefeated()
	{
		playerControllable = false;
		lastState = playerState.PlayerDefeat;
		GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) playerState.PlayerDefeat);
	}
	
	public void BeDead()
	{
		playerControllable = false;
		lastState = playerState.PlayerDeath;
		GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) playerState.PlayerDeath);
	}
	
	[RPC]
	public void SwitchDirection ()
	{
		transform.localScale += new Vector3(-transform.localScale.x * 2, 0, 0);
	}
	
	[RPC]
	public void LaunchAnimation(int thisState)
	{
		animator.SetInteger("nextStateNum", thisState);
		animator.SetTrigger("nextStateTrigger");
	}
	
	[RPC]
	public void PlaySFX(int sfx) {
		switch (sfx) {
		case 0: 
			GetComponent<CharacterAudio>().PlayStepSFX();
			break;

		case 1: 
			GetComponent<CharacterAudio>().PlayJumpSFX();
			break;

		case 2: 
			GetComponent<CharacterAudio>().PlayHitSFX();
			break;

		case 3: 
			GetComponent<CharacterAudio>().PlayDieSFX();
			break;

		case 4: 
			GetComponent<CharacterAudio>().PlayVictorySFX();
			break;

		case 5: 
			GetComponent<CharacterAudio>().PlayDodgeSFX();
			break;

		case 6: 
			GetComponent<CharacterAudio>().PlayLandSFX();
			break;
		
		}
	}
}
