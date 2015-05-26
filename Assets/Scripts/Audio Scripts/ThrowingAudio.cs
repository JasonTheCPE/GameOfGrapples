using UnityEngine;
using System.Collections;

public class ThrowingAudio : MonoBehaviour {
	public AudioClip HitWall;
	public AudioClip HitPlayer;
	public AudioClip Throw;
	public AudioClip PickUp;
	private SoundManager sm;
	private bool foundSM;

	// Use this for initialization
	void Start () {
		sm = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
		if (sm) {
			foundSM = true;
		} else {
			Debug.Log("Could not find the sound manager!");
			foundSM = false;
		}
	}

	//If we add more SFX, we can use these functions to switch between the sfx. 
	
	public void PlayThrowSFX() {
		if (foundSM) {
			sm.PlaySingle(Throw);
		}
	}
	
	public void PlayHitWallSFX() {
		if (foundSM) {
			sm.PlaySingle(HitWall);
		}
	}
	
	public void PlayHitPlayerSFX() {
		if (foundSM) {
			sm.PlaySingle(HitPlayer);
		}
	}
	public void PlayPickupSFX() {
		if (foundSM) {
			sm.PlaySingle(PickUp);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
