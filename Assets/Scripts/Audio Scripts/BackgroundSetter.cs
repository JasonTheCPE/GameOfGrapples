using UnityEngine;
using System.Collections;

public class BackgroundSetter : MonoBehaviour {
	public string bgName;

	// Use this for initialization
	void Start () {
		GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlaySong(bgName);
	}
}
