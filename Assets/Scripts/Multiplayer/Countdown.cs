using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	private GameObject[] players;
	public int countMax;
	public bool showCountDown = false;
	private float countDown;

	// Use this for initialization
	private 
	void Start () {
		players = GameObject.FindGameObjectsWithTag("Player");
		AnimateCountdown();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (showCountDown) {
			if (countDown > -1) {
				countDown -= Time.deltaTime/2;
			} else {
				countDown = -1;
				TurnOnPlayers();
				StartMatchTimer();
				showCountDown = false;
			}

			GUI.color = Color.red;    
			GUI.Box (new Rect (Screen.width / 2 - 100, 50, 200, 175), "GET READY");
			
			// display countdown    
			GUI.color = Color.white;
			if (countDown < .5) {
				GUI.TextField(new Rect (Screen.width / 2 - 90, 75, 180, 140), "GO!");
			} else {
				GUI.TextField(new Rect (Screen.width / 2 - 90, 75, 180, 140), countDown.ToString("0"));
			}
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
		TurnOffPlayers();
		countDown = countMax;
		showCountDown = true;
	}
}
