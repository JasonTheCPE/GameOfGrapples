using UnityEngine;
using System.Collections;

public class PhysicsPlayerMovement : MonoBehaviour
{
	enum playerState
	{
		PlayerIdle = 0, PlayerRunning = 1, PlayerInAir = 2, PlayerMovingInAir = 3,
		PlayerWallCling = 4, PlayerHangingFromRope = 5, PlayerHangingFromRopeMoving = 6,
		PlayerThrowUpwards = 7, PlayerThrowDownwards = 8, PlayerDodge = 9,
		PlayerDefeat = 10, PlayerVictory = 11, PlayerDeath = 12
	};

	enum playerSounds
	{
		Step = 0, Jump = 1, Hit = 2, Die = 3,
		Victory = 4, Dodge = 5, Land = 6
	};
	
	public float moveForceMultiplier = 9000f;
	public Vector2 moveForceVector;
	
	public float jumpForceMultiplier = 8000f;
	public Vector2 jumpForceVector;
	private float upwardVelocityBound;
	public short maxJumps = 2;
	public short jumpsUsed = 0;
	
	public float dodgeForce = 12000f;
	public Vector2 dodgeForceVector;
	private const float dodgeAnimationTime = 0.38f;
	private float timeOfLastDodge = 0f;
	private float dodgeRechargeTime = 1.0f;
	
	private const float throwAnimationTime = 0.1f;
	
	private Rigidbody2D rb;
	private float playerWeight;
	
	private bool facingRight = true;
	private const float minSidewaysMoveAnimationSpeed = 0.6f;
	private Animator animator;
	private playerState lastState;
	private float animationTimeLeft = 0f;
	
	public bool canClingToWall = false;
	public bool isGrabbingWall = false;
	public bool movementAttempted = false;
	public bool isOnGround = false;
	public bool isMoving = false;
	public bool isDodging = false;
	
	public bool playerControllable = true;
	
	public GrappleManager grappleManager;
	
	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		playerWeight = rb.mass;
		jumpForceVector = new Vector2(0.0f, playerWeight * jumpForceMultiplier);
		moveForceVector = new Vector2(playerWeight * moveForceMultiplier, 0.0f);
		dodgeForceVector = new Vector2(playerWeight * dodgeForce, 0f);
		animator = GetComponent<Animator>();
		grappleManager = GetComponent<GrappleManager>();
		lastState = playerState.PlayerIdle;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GetComponent<NetworkView>().isMine && playerControllable)
		{
			float movement = Input.GetAxis("Horizontal");
			
			if(animationTimeLeft <= 0)
			{
				bool canDodge = Time.time > timeOfLastDodge + dodgeRechargeTime;
				if(Input.GetButtonDown("DodgeLeft") && isOnGround && canDodge)
				{
					facingRight = false;
					rb.AddForce(-dodgeForceVector);
					isDodging = true;
				}
				else if(Input.GetButtonDown("DodgeRight") && isOnGround && canDodge)
				{
					facingRight = true;
					rb.AddForce(dodgeForceVector);
					isDodging = true;
				}
				else
				{
					if(movement < 0)
					{
						facingRight = false;
						rb.AddForce(-moveForceVector * Time.deltaTime);
						movementAttempted = true;
					}
					else if(movement > 0)
					{
						facingRight = true;
						rb.AddForce(moveForceVector * Time.deltaTime);
						movementAttempted = true;
					}
					else
					{
						movementAttempted = false;
					}
					
					TryClingToWall();
					
					if (Input.GetKeyDown("space"))
					{
						if (jumpsUsed < maxJumps)
						{
							//Vector2 adjustedJumpForce;
							if(!isOnGround && !isGrabbingWall)
							{
								if(jumpsUsed == 0)
								{
									jumpsUsed = 1;
								}
							}
							
							//Debug.Log(adjustedJumpForce);
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

		gameObject.SetActive(false);
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
			jumpsUsed = 1;
		}
		else
		{
			isGrabbingWall = false;
		}
	}
	
	public void AnimateThrow(Vector3 throwVector)
	{
		if (GetComponent<NetworkView>().isMine && playerControllable)
		{
			if(throwVector.y > 0)
			{
				lastState = playerState.PlayerThrowUpwards;
			}
			else
			{
				lastState = playerState.PlayerThrowDownwards;
			}
			GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) lastState);
			animationTimeLeft = throwAnimationTime;
		}
	}
	
	private void TriggerAnimationTransition()
	{
		playerState thisState;
		
		if(lastState == playerState.PlayerDefeat ||
		   lastState == playerState.PlayerVictory ||
		   lastState == playerState.PlayerDeath)
		{
			return;
		}
		
		if(animationTimeLeft > 0)
		{
			animationTimeLeft -= Time.deltaTime;
			return;
		}
		
		if(isDodging)
		{
			lastState = playerState.PlayerDodge;
			GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) lastState);
			isDodging = false;
			animationTimeLeft = dodgeAnimationTime;
			timeOfLastDodge = Time.time;
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
				if(grappleManager.grappleIsOut)
				{
					thisState = playerState.PlayerHangingFromRopeMoving;
				}
				else
				{
					thisState = playerState.PlayerMovingInAir;
				}
			}
			else
			{
				if(grappleManager.grappleIsOut)
				{
					thisState = playerState.PlayerHangingFromRope;
				}
				else
				{
					thisState = playerState.PlayerInAir;
				}
				
				if(isGrabbingWall)
				{
					thisState = playerState.PlayerWallCling;
				}
			}
		}
		
		if(thisState != lastState)
		{
			//LaunchAnimation(thisState);
			GetComponent<NetworkView>().RPC("LaunchAnimation", RPCMode.All, (int) thisState);
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
