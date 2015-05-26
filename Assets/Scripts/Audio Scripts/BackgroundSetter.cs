using UnityEngine;
using System.Collections;

public class BackgroundSetter : MonoBehaviour {
	public AudioClip bg;

	// Use this for initialization
	void Start () {
		GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlayBackground(bg);
	}
}
