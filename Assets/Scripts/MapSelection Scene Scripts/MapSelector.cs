using UnityEngine;
using System.Collections;

public class MapSelector : MonoBehaviour {
	public MapSelector instance;

	private string currentMenu;
	private Object[] skins;
	private Vector2 scrollLobby = Vector2.zero;
	private int time = 60; 				// in seconds
	private int startHealth = 1;
	private int ammo = 4;
	private bool AllReady = false;

	public AudioClip MainMenuAudio;
	// Use this for initialization
	void Start () {
		instance = this;
		skins = Resources.LoadAll("Skins");
		currentMenu = "Lobby";
	}

	//keep the instance alive
	void FixedUpdate() {
		instance = this;

		time = MultiplayerManager.instance.matchTime;
		startHealth = MultiplayerManager.instance.matchHP;
		ammo = MultiplayerManager.instance.matchAmmo;

		int prevTeam = MultiplayerManager.instance.PlayerList[0].team;
		bool TeamVariety = false;
		bool AllLockedIn = false;
		foreach(MyPlayer pl in MultiplayerManager.instance.PlayerList) {
			if (pl.team != prevTeam || !MultiplayerManager.instance.allowTeams) {
				TeamVariety = true;
			}
		}

		//if (TeamVariety && AllLockedIn) {
			AllReady = true;
		//}
	}

	public void NavigateTo(string nextMenu) {
		currentMenu = nextMenu;
	}

	void OnGUI() {
		switch(currentMenu) {
		case "Lobby":
			Menu_Lobby();
			break;
		case "SelMap":
			Menu_SelectMap();
			break;
		}
	}

	private void Menu_Lobby() {
		//list all the players in the match and their wins
		scrollLobby = GUILayout.BeginScrollView(scrollLobby, GUILayout.MaxWidth(200));
		foreach(MyPlayer pl in MultiplayerManager.instance.PlayerList) {
			if (pl.playerNetwork == Network.player) {
				GUI.color = Color.cyan;
			}
			if (MultiplayerManager.instance.allowTeams) {
				GUILayout.Box(pl.playerName + ": " + pl.wins + " wins | Team " + pl.team.ToString());
			} else {
				GUILayout.Box(pl.playerName + ": " + pl.wins + " wins");
			}
			GUI.color = Color.white;
		}
		GUILayout.EndScrollView();

		GUI.Box(new Rect(250, 10, 200, 40), MultiplayerManager.instance.currentMap.mapName);

		GUI.Label(new Rect(250, 100, 200, 40), "Set Time");
		GUI.Label(new Rect(290, 123, 100, 50), time.ToString());

		GUI.Label(new Rect(250, 160, 200, 40), "Set Health");
		GUI.Label(new Rect(295, 183, 100, 50), startHealth.ToString());

		GUI.Label(new Rect(250, 220, 200, 40), "Set Ammo");
		if (ammo == 11) {
			GUI.Label(new Rect(295, 243, 100, 50), "Unl.");
		} else {
			GUI.Label(new Rect(295, 243, 100, 50), ammo.ToString());
		}

		if (Network.isServer) {
			if(GUI.Button(new Rect(250, 51, 200, 40), "Change Map")) {
				NavigateTo("SelMap");
			}

			if(GUI.Button(new Rect(250, 120, 30, 30), "-")) {
				if (time > 0) {
					if (time == 1) {
						time = 0;
					} else if (time == 15) {
						time = 1;
					} else {
						time -= 15;
					}
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetTime", RPCMode.All, time);
			}
			if(GUI.Button(new Rect(322, 120, 30, 30), "+")) {
				if (time < 300) {
					if (time == 0) {
						time = 1;
					} else if (time == 1) {
						time = 15;
					} else {
						time += 15;
					}
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetTime", RPCMode.All, time);
			}

			if(GUI.Button(new Rect(250, 180, 30, 30), "-")) {
				if (startHealth > 1) {
					--startHealth;
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetHealth", RPCMode.All, startHealth);
			}
			if(GUI.Button(new Rect(322, 180, 30, 30), "+")) {
				if (startHealth < 10) {
					++startHealth;
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetHealth", RPCMode.All, startHealth);
			}

			if(GUI.Button(new Rect(250, 243, 30, 30), "-")) {
				if (ammo > 1) {
					--ammo;
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetAmmo", RPCMode.All, ammo);
			}
			if(GUI.Button(new Rect(322, 243, 30, 30), "+")) {
				if (ammo < 11) {
					++ammo;
				}
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetAmmo", RPCMode.All, ammo);
			}
		}
		
		GUI.Label(new Rect(460, 10, 130, 30), "Choose Skin");
		GUILayout.BeginArea(new Rect(460, 41, 150, Screen.height - 90));
		
		for(int i = 0; i < skins.Length; ++i) {
			if(GUILayout.Button(skins[i].name)) {
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetSkin", RPCMode.All, i, Network.player);
			}
		}
		
		GUILayout.EndArea();
		
		if (Network.isServer) {
			if(GUI.Button(new Rect(621, 10, 130, 40), "Toggle Teams")) {
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("ToggleTeams", RPCMode.All);
			}

			if (AllReady) {
				if(GUI.Button(new Rect(Screen.width - 405, Screen.height - 40, 200, 40), "Start Match")) {
					MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetTime", RPCMode.All, time);
					MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetHealth", RPCMode.All, startHealth);
					MultiplayerManager.instance.GetComponent<NetworkView>().RPC("Client_LoadMultiplayerMap", 
						RPCMode.All, MultiplayerManager.instance.currentMap.mapLoadName, MultiplayerManager.instance.oldPrefix + 1);
					MultiplayerManager.instance.oldPrefix += 1;
					MultiplayerManager.instance.isMatchStarted = true;
				}
			}
		}

		if (MultiplayerManager.instance.allowTeams) {
			for (int i = 0; i < 7; ++i) {
				if(GUI.Button(new Rect(621, 50 + 40*i, 130, 40), "Team " + i.ToString())) {
					MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetTeam", RPCMode.All, Network.player, i);
				}
			}
		}
		
		if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Disconnect")) {
			Network.Disconnect();
		}
	}

	private void Menu_SelectMap() {
		if(GUI.Button(new Rect(10, 10, 200, 50), "Back")) {
			NavigateTo("Lobby");
		}
		
		GUI.Label(new Rect(220, 10, 130, 30), "Choose Map");
		GUILayout.BeginArea(new Rect(350, 10, 150, Screen.height));
		
		foreach(MapSettings map in MultiplayerManager.instance.MapList) {
			if(GUILayout.Button(map.mapName)) {
				NavigateTo("Lobby");
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetCurrentMap", RPCMode.All, map.mapName);
			}
		}
		
		GUILayout.EndArea();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlayBackground(MainMenuAudio);
		Application.LoadLevel("Lobby");
	}
}
