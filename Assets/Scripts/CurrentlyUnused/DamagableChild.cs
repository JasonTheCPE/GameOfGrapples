using UnityEngine;
using System.Collections;

public class DamagableChild : MonoBehaviour
{
	Health health;
	// Use this for initialization
	void Start ()
	{
		health = GetComponentInParent<Health>();
	}
	
	public void TakeDamage()
	{
		health.GetHurt();
	}
}
