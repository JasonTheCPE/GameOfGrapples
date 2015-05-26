using UnityEngine;
using System.Collections;

//THIS SCRIPT WILL CAUSE ERRORS IF YOU DON'T START IN MAINMENU AND YOU GO THERE, 
//BUT THAT WON'T EVER HAPPEN IN THE RELEASE! It is for testing purposes only.

public class Checker : MonoBehaviour {
	public GameObject sm;
	public AudioClip background;

	// Use this for initialization
	void Start () {
		Object check = GameObject.Find("Sound Manager");
		if (check) {
			enabled = false;
		} else {
			Debug.Log("No Sound Manager Detected! Creating new one");
			GameObject soundman = Instantiate(sm);
			soundman.GetComponent<SoundManager>().PlayBackground(background);
			soundman.name = "Sound Manager";
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
