using UnityEngine;
using System.Collections;

public class BackgroundSetter : MonoBehaviour {
	public AudioClip bg;
	public string bgName;

	// Use this for initialization
	void Start () {
		//GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlayBackground(bg);
		GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlaySong(bgName);
	}
}
