using UnityEngine;
using System.Collections;

public class MapSelector : MonoBehaviour {
	public MapSelector instance;

	private string currentMenu;
	private Object[] skins;
	private Vector2 scrollLobby = Vector2.zero;
	// Use this for initialization
	void Start () {
		instance = this;
		skins = Resources.LoadAll("Skins");
		currentMenu = "Lobby";
	}

	//keep the instance alive
	void FixedUpdate() {
		instance = this;
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
			GUILayout.Box(pl.playerName + ": " + pl.wins + " wins");
			GUI.color = Color.white;
		}
		GUILayout.EndScrollView();

		GUI.Box(new Rect(250, 10, 200, 40), MultiplayerManager.instance.currentMap.mapName);

		if (Network.isServer) {
			if(GUI.Button(new Rect(250, 51, 200, 40), "Change Map")) {
				NavigateTo("SelMap");
			}
		}
		
		GUI.Label(new Rect(460, 10, 130, 30), "Choose Skin");
		GUILayout.BeginArea(new Rect(460, 41, 150, Screen.height - 90));
		
		for(int i = 0; i < skins.Length; ++i) {
			if(GUILayout.Button("Skin " + i.ToString())) {
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("SetSkin", RPCMode.All, i, Network.player);
			}
		}
		
		GUILayout.EndArea();
		
		if (Network.isServer) {
			if(GUI.Button(new Rect(Screen.width - 405, Screen.height - 40, 200, 40), "Start Match")) {
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("Client_LoadMultiplayerMap", 
					RPCMode.All, MultiplayerManager.instance.currentMap.mapLoadName, MultiplayerManager.instance.oldPrefix + 1);
				MultiplayerManager.instance.oldPrefix += 1;
				MultiplayerManager.instance.isMatchStarted = true;
				//MultiplayerManager.instance.GetComponent<Network>().maxConnections = -1; // TODO
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
		Application.LoadLevel("Lobby");
	}
}
