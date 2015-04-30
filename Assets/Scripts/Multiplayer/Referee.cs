﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Referee : MonoBehaviour {

	private MultiplayerManager mm;
	public List<ActivePlayer> ingamePlayers;
	private int[] teams = new int[8] {0,0,0,0,0,0,0,0};

	// Use this for initialization
	void Start () {
		mm = GameObject.Find("Multiplayer Manager").GetComponent<MultiplayerManager>();
		if(mm) {
			foreach(MyPlayer mp in mm.PlayerList) {
				AddPlayer(mp.playerName, mp.playerNetwork, mp.team);
			}
		} else {
			Debug.Log("ERROR! Could not find the multiplayer manager!");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 100, 100), "Win")) {
			MultiplayerManager.instance.GetComponent<NetworkView>().RPC("AssignWin", RPCMode.All, Network.player);
		}
	}

	public void KillPlayer(NetworkPlayer view) {
		foreach(ActivePlayer ap in ingamePlayers) {
			if (ap.playerNetwork == view) {
				ap.isAlive = false;
				teams[ap.onTeam + 1] -= 1;
			}
		}

		if (mm.allowTeams) {
			int teamsAlive = 0, winningTeam = -1;
			for (int i = 1; i < 8; ++i) {
				if(teams[i] > 0) {
					++teamsAlive;
					winningTeam = i - 1;
				}
			}

			if (teamsAlive == 1) {
				Debug.Log("Game over! Team " + winningTeam.ToString() + " wins!");
				mm.GetComponent<NetworkView>().RPC("AssignTeamWin", RPCMode.All, winningTeam);
			} else if (teamsAlive == 0) {
				Debug.Log("Game over! It's a DRAW! All teams DIED!");
				mm.GetComponent<NetworkView>().RPC("AssignDraw", RPCMode.All);
			}

		} else {
			if(teams[0] == 1) {
				Debug.Log("Game over! One person left alive");
				foreach(ActivePlayer ap in ingamePlayers) {
					if (ap.isAlive) {
						Debug.Log("The winner is " + ap.playerName);
						mm.GetComponent<NetworkView>().RPC("AssignWin", RPCMode.All, ap.playerNetwork);
						return;
					}
				}
			} else if (teams[0] == 0) {
				Debug.Log("Game over! It's a DRAW!");
				mm.GetComponent<NetworkView>().RPC("AssignDraw", RPCMode.All);
			}
		}
	}

	void AddPlayer(string name, NetworkPlayer view, int teamNumber) {
		ActivePlayer tempPlayer = new ActivePlayer();
		tempPlayer.playerName = name;
		tempPlayer.playerNetwork = view;
		tempPlayer.isAlive = true;
		tempPlayer.onTeam = teamNumber;
		ingamePlayers.Add(tempPlayer);
		teams[teamNumber + 1] += 1;
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		MultiplayerManager.instance.PlayerList.Clear();
		Application.LoadLevel("Lobby");
	}
}

[System.Serializable]
public class ActivePlayer {
	public string playerName = "";
	public NetworkPlayer playerNetwork;
	public bool isAlive;
	public int onTeam;
}
