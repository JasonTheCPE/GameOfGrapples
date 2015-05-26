using UnityEngine;
using System.Collections;

public class CharacterAudio : MonoBehaviour {
	public AudioClip step;
	public AudioClip hit1;
	public AudioClip hit2;
	public AudioClip die;
	public AudioClip victory;
	public AudioClip dodge1;
	public AudioClip dodge2;
	public AudioClip jump;
	public AudioClip land;
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
	
	public void PlayStepSFX() {
		if (foundSM) {
			sm.PlaySingle(step);
		}
	}

	public void PlayHitSFX() {
		if (foundSM) {
			sm.RandomizeSfx(hit1, hit2);
		}
	}

	public void PlayDieSFX() {
		if (foundSM) {
			sm.PlaySingle(die);
		}
	}

	public void PlayVictorySFX() {
		if (foundSM) {
			sm.PlaySingle(victory);
		}
	}

	public void PlayDodgeSFX() {
		if (foundSM) {
			sm.RandomizeSfx(dodge1, dodge2);
		}
	}

	public void PlayLandSFX() {
		if (foundSM) {
			sm.PlaySingle(land);
		}
	}

	public void PlayJumpSFX() {
		if (foundSM) {
			sm.PlaySingle(jump);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
