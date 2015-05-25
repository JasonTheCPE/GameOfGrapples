using UnityEngine;
using System.Collections;

public class PostGame : MonoBehaviour {
	private MultiplayerManager mm;
	private string mode = "";
	private bool hasLocked = false;
	// Use this for initialization
	void Start () {
		mm = GameObject.Find("Multiplayer Manager").GetComponent<MultiplayerManager>();

		if(mm) {
			int i = 1;
			if (mm.PreviousWinners.Count == 0) {
				mode = "Tie-Game";
				foreach(MyPlayer mp in mm.PlayerList) {
					if (mp.playerNetwork == Network.player) {
						GameObject temp = (GameObject) Network.Instantiate(mm.accessSkin(mp.skinID), Vector3.zero, Quaternion.identity, 0); 
						temp.GetComponent<NetworkView>().observed = transform;
						temp.GetComponent<HoverName>().localName = mp.playerName;
						temp.transform.position = GameObject.Find("Tie " + i.ToString() + " Spawn").transform.position;
						temp.transform.localScale = GameObject.Find("Tie " + i.ToString() + " Spawn").transform.localScale;
						temp.GetComponent<Rigidbody2D>().isKinematic = true;
						//TODO temp.GetComponent<PhysicsPlayerMovement>().StartClapping();
						NetworkRigidbody2D _NetworkRigidbody = temp.GetComponent<NetworkRigidbody2D>();
						_NetworkRigidbody.enabled = false;
						Throwing th = temp.GetComponent<Throwing>();
						th.enabled = false;
					}
					++i;
				}
			} else if (mm.allowTeams) {
				mode = "Team-Game";
				foreach(MyPlayer mp in mm.PreviousWinners) {
					if (mp.playerNetwork == Network.player) {
						GameObject temp = (GameObject) Network.Instantiate(mm.accessSkin(mp.skinID), Vector3.zero, Quaternion.identity, 0);
						temp.GetComponent<NetworkView>().observed = transform;
						temp.GetComponent<HoverName>().localName = mp.playerName;
						temp.transform.position = GameObject.Find("Team " + i.ToString() + " Spawn").transform.position;
						temp.transform.localScale = GameObject.Find("Team " + i.ToString() + " Spawn").transform.localScale;
						//TODO temp.GetComponent<PhysicsPlayerMovement>().StartClapping();
						temp.GetComponent<Rigidbody2D>().isKinematic = true;
						NetworkRigidbody2D _NetworkRigidbody = temp.GetComponent<NetworkRigidbody2D>();
						_NetworkRigidbody.enabled = false;
						Throwing th = temp.GetComponent<Throwing>();
						th.enabled = false;
					}
					++i;
				}
			} else {
				mode = "Free-For-All";
				foreach(MyPlayer mp in mm.PlayerList) {
					if (mp.playerNetwork == Network.player) {
						GameObject temp = (GameObject) Network.Instantiate(mm.accessSkin(mp.skinID), Vector3.zero, Quaternion.identity, 0);
						temp.GetComponent<NetworkView>().observed = transform;
						//TODO temp.GetComponent<PhysicsPlayerMovement>().StartClapping();
						temp.GetComponent<HoverName>().localName = mp.playerName;
						temp.GetComponent<Rigidbody2D>().isKinematic = true;
						NetworkRigidbody2D _NetworkRigidbody = temp.GetComponent<NetworkRigidbody2D>();
						_NetworkRigidbody.enabled = false;
						Throwing th = temp.GetComponent<Throwing>();
						th.enabled = false;
						if (mp.playerNetwork != mm.PreviousWinners[0].playerNetwork) {
							temp.transform.position = GameObject.Find("Loser " + i.ToString() + " Spawn").transform.position;
							temp.transform.localScale = GameObject.Find("Loser " + i.ToString() + " Spawn").transform.localScale;
							++i;
						} else {
							temp.transform.position = GameObject.Find("Winner Spawn").transform.position;
							temp.transform.localScale = GameObject.Find("Winner Spawn").transform.localScale;
						}
					}
				}
			}
		} else {
			Debug.Log("ERROR! Could not find the multiplayer manager!");
		}

		Debug.Log("It was a " + mode);
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (!hasLocked) {
			if(GUI.Button(new Rect(Screen.width - 405, Screen.height - 40, 200, 40), "Ready")) {

			}
		}
	}
}
