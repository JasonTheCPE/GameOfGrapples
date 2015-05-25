using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	private GameObject[] players;
	// Use this for initialization
	private 
	void Start () {
		players = GameObject.FindGameObjectsWithTag("Player");
		TurnOffPlayers();
		AnimateCountdown();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		/*To Test*/
		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 100, 100), "start")) {
			StartMatchTimer();
			TurnOnPlayers();
		}
	}

	void TurnOffPlayers() {
		foreach (GameObject pl in players) {
			pl.GetComponent<Throwing>().enabled = false;							//turn off throwing
			pl.GetComponent<PhysicsPlayerMovement>().enabled = false;				//turn off movement
		}
	}

	void TurnOnPlayers() {
		foreach (GameObject pl in players) {
			pl.GetComponent<Throwing>().enabled = true;								//turn on throwing
			pl.GetComponent<PhysicsPlayerMovement>().enabled = true;				//turn on movement
		}
	}

	void StartMatchTimer() {
		GetComponent<Referee>().StartTimer();
	}

	//note that the timer in referee starts out off, so you only need to do this... never?
	void StopMatchTimer() {
		GetComponent<Referee>().StopTimer();
	}

	//when done, use TurnOnPlayers and StartMatchTimer to activate them again
	void AnimateCountdown() {

	}
}
