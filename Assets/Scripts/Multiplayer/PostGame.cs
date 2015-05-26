﻿using UnityEngine;
using System.Collections;

public class PostGame : MonoBehaviour {
	private MultiplayerManager mm;
	private string mode = "";

	public bool hasLocked = false;
	public bool[] playersLocked;
	public int playersUnlocked;

	private const int winner = 0;
	private const int loser = 1;
	private const int meh = 2;

	//KNOWN BUG! Players locked isn't working, so using players unlocked. Whenever LockIn is called, it keeps saying that it is the same player
	//this may be from the lack of network view....

	// Use this for initialization
	void Start () {
		mm = GameObject.Find("Multiplayer Manager").GetComponent<MultiplayerManager>();
		
		if(mm) {
			InitiateLocked();
			int i = 1;
			if (mm.PreviousWinners.Count == 0) {
				mode = "Tie-Game";
				foreach(MyPlayer mp in mm.PlayerList) {
					GameObject skin = CreateSkin(mp.skinID,  GameObject.Find("Tie " + i.ToString() + " Spawn"), mp.playerName, meh);
					++i;
				}
			} else if (mm.allowTeams) {
				mode = "Team-Game";
				foreach(MyPlayer mp in mm.PreviousWinners) {
					GameObject skin = CreateSkin(mp.skinID, GameObject.Find("Team " + i.ToString() + " Spawn"), mp.playerName, winner);
					++i;
				}
			} else {
				mode = "Free-For-All";
				foreach(MyPlayer mp in mm.PlayerList) {
					GameObject skin;
					if (mp.playerNetwork != mm.PreviousWinners[0].playerNetwork) {
						skin = CreateSkin(mp.skinID, GameObject.Find("Loser " + i.ToString() + " Spawn"), mp.playerName, loser);
						++i;
					} else {
						skin = CreateSkin(mp.skinID, GameObject.Find("Winner Spawn"), mp.playerName, winner);
					}
				}
			}
		} else {
			Debug.Log("ERROR! Could not find the multiplayer manager!");
		}
		
		Debug.Log("It was a " + mode);
	}
	
	GameObject CreateSkin(int id, GameObject baseObject, string name, int reaction) {
		GameObject skin = mm.accessSkin(id);
		GameObject ret = (GameObject) Instantiate(skin);
		ret.GetComponent<NetworkView>().observed = transform;				//don't view the network rigidbody
		if (reaction == winner) {
			ret.GetComponent<PhysicsPlayerMovement>().BeVictorious();		//play victorious animation and disable movement
		} else if (reaction == meh) {
			ret.GetComponent<PhysicsPlayerMovement>().BeDefeated();			//play meh (TODO) animation and disable movement
		} else {
			ret.GetComponent<PhysicsPlayerMovement>().BeDefeated();			//play defeated animation and disable movement
		}
		ret.GetComponent<Health>().enabled = false;							//don't start out invincible
		ret.GetComponent<HoverName>().localName = "";						//display your name
		ret.GetComponent<NetworkRigidbody2D>().enabled = false;				//turn off networkrigidbody2D
		ret.GetComponent<Throwing>().enabled = false;						//turn off throwing
		ret.transform.position = baseObject.transform.position;
		ret.transform.localScale = baseObject.transform.localScale;
		ret.GetComponent<Rigidbody2D>().isKinematic = true;					//stop the rigidbody from moving
		return ret;
	}
	
	void InitiateLocked() {
		playersLocked = new bool[mm.PlayerList.Count];
		
		for (int i = 0; i < playersLocked.Length; ++i) {
			playersLocked[i] = false;
		}

		playersUnlocked = playersLocked.Length;
	}

	public void LockIn(NetworkPlayer player) {
		int i = 0;
		foreach(MyPlayer mp in mm.PlayerList) {
			if (mp.playerNetwork == player) {
				if (playersLocked [i])
					Debug.Log("OH NO!");
				playersLocked [i] = true;
			}
		}

		playersUnlocked--;
		check();
	}
	
	void check() {
		bool EveryoneLocked = true;
		foreach(bool b in playersLocked) {
			if (!b) {
				EveryoneLocked = false;
			}
		}
		if(EveryoneLocked || playersUnlocked == 0) {
			Application.LoadLevel("Prep");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI() {
		if (!hasLocked) {
			if(GUI.Button(new Rect(Screen.width - 405, Screen.height - 40, 200, 40), "Ready")) {
				hasLocked = true;
				mm.GetComponent<NetworkView>().RPC("PostGameLockIn", RPCMode.All, Network.player);
			}
		}
	}
}
