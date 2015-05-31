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
	private const float invincibilityFlashPeriod = 1.0f;
	
	private SpriteRenderer[] bodyParts;
	public Color normalColor = new Color(1f, 1f, 1f);
	private Color damagedColor = new Color(1f, 0, 0);
	private Color invincibilityColor = new Color(1f, 1f, 0);
	private Color healedColor = new Color(0.3f, 1f, 0.3f);
	public Color currentColor;

	void Start ()
	{
		health = MaxHealth;
		invincibilityTime = startInvincibilitySeconds; //invincibility at the start of a match
		damagedTime = healedTime = 0;
		bodyParts = GetComponentsInChildren<SpriteRenderer>();
	}
	
	void Update()
	{
		//GetComponent<NetworkView>().RPC("FadeInvincibilityAndColors", RPCMode.All);
		FadeInvincibilityAndColors();
	}
	
	public void BeInvincible(float seconds)
	{
		invincibilityTime = seconds;
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
			GetComponent<CharacterAudio>().PlayHitSFX();
			int health = GetHit(1);
		}
		
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
	
	public void SetAllChildColors(Color colorToSet)
	{
		foreach(SpriteRenderer part in bodyParts)
		{
			part.material.color = colorToSet;
		}
	}
	
	private void FadeInvincibilityAndColors()
	{
		currentColor = normalColor;
		
		if(invincibilityTime > 0)
		{
			invincibilityTime -= Time.deltaTime;
			
			float lerp = Mathf.PingPong(Time.time * 3, invincibilityFlashPeriod) / invincibilityFlashPeriod;
			currentColor = Color.Lerp(currentColor, invincibilityColor, lerp);
		}
		if(damagedTime > 0)
		{
			damagedTime -= Time.deltaTime;
			
			float lerp = damagedTime / repeatDamageDelay;
			currentColor = Color.Lerp(currentColor, damagedColor, lerp);
		}
		if(healedTime > 0)
		{
			healedTime -= Time.deltaTime;
			
			float lerp = healedTime / healFlashPeriod;
			currentColor = Color.Lerp(currentColor, healedColor, lerp);
		}
		
		SetAllChildColors(currentColor);
	}
}
