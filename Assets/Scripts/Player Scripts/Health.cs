using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public int MaxHealth = 1;
	public int health;
	
	public float invincibilityTime;
	public float damagedTime;
	public float healedTime;
	private const float startInvincibilitySeconds = 5.0f;
	private const float repeatDamageDelay = 1.0f;
	private const float healFlashPeriod = 1.0f;
	
	private SpriteRenderer[] bodyParts;
	private Color NormalColor = new Color(1f, 1f, 1f);
	private Color DamagedColor = new Color(1f, 0, 0);
	private Color InvincibilityColor = new Color(1f, 1f, 0);
	private Color HealedColor = new Color(0.3f, 1f, 0.3f);

	void Start ()
	{
		health = MaxHealth;
		invincibilityTime = startInvincibilitySeconds; //invincibility at the start of a match
		damagedTime = healedTime = 0;
		bodyParts = GetComponentsInChildren<SpriteRenderer>();
	}
	
	void Update()
	{
		if(invincibilityTime > 0)
		{
			invincibilityTime -= Time.deltaTime;
		}
		if(damagedTime > 0)
		{
			damagedTime -= Time.deltaTime;
		}
		if(healedTime > 0)
		{
			healedTime -= Time.deltaTime;
		}
	}

	public int GetHit(int damage)
	{
		health -= damage;
		if (health <= 0) {
			health = 0;
		}

		damagedTime = repeatDamageDelay;
		
		return health;
	}

	public int Heal (int restore)
	{
		health += restore;
		
		if (health > MaxHealth)
		{
			health = MaxHealth;
		}

		healedTime = healFlashPeriod;
		
		return health;
	}

	public void InitHealth(int Max)
	{
		MaxHealth = Max;
		health = MaxHealth;
	}

	[RPC]
	public void GetHurt()
	{
		if(invincibilityTime <= 0 && damagedTime <= 0)
		{
			int health = GetHit(1);
		}
		
		//GetComponent<Throwing>().pickupStar();
		if(GetComponent<NetworkView>().isMine == true)
		{
			Referee reff = GameObject.Find("Ingame Manager").GetComponent<Referee>();
			if (reff)
			{
				reff.GetComponent<NetworkView>().RPC("KillPlayer", RPCMode.All, Network.player);
			}
			
			if (health == 0) {
				GetComponent<NetworkView>().RPC("Die", RPCMode.All);
			}
		}
	}
	
	private void FlashColor(Color color)
	{
		
	}
}
