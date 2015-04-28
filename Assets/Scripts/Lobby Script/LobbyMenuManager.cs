using UnityEngine;
using System.Collections;

public class LobbyMenuManager : MonoBehaviour {

	public string currentMenu; 
	public string matchName;
	public string matchPassword;
	public int maxPlayers = 4;
	public LobbyMenuManager instance;

	private Object[] skins;
	private Vector2 scrollLobby = Vector2.zero;
	private string iptemp = "127.0.0.1";

	// Use this for initialization
	void Start () {
		currentMenu = "Main";
		matchName = "Temp Name: " + Random.Range(0, 5000);
		instance = this;
		skins = Resources.LoadAll("Skins");
		//DontDestroyOnLoad(transform.gameObject);
	}

	//keep the instance alive
	void FixedUpdate() {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		switch(currentMenu) {
		case "Main":
			Menu_Main();
			break;
		case "Lobby":
			Menu_Lobby();
			break;
		case "Host":
			Menu_HostGame();
			break;
		case "SelMap":
			Menu_ChooseMap();
			break;
		case "InGame":
			Menu_InGame();
			break;
		}
	}

	public void NavigateTo(string nextMenu) {
		currentMenu = nextMenu;
	}

	private void Menu_Main() {
		if(GUI.Button(new Rect(10, 10, 200, 50), "Host Game")) {
			NavigateTo("Host");
		}

		if(GUI.Button(new Rect(10, 70, 200, 50), "Refresh")) {
			MasterServer.RequestHostList("Deathmatch");
		}

		GUI.Label(new Rect(220, 10, 130, 30), "Player Name");
		MultiplayerManager.instance.playerName = GUI.TextField(new Rect(350, 10, 150, 30), MultiplayerManager.instance.playerName);

		if(GUI.Button(new Rect(510, 10, 100, 30), "Save Name")) {
			PlayerPrefs.SetString("Player Name", MultiplayerManager.instance.playerName);
		}

		GUI.Label(new Rect(220, 50, 130, 30), "Direct Connect");
		iptemp = GUI.TextField(new Rect(350, 50, 150, 30), iptemp);
		
		if(GUI.Button(new Rect(510, 50, 100, 30), "Connect")) {
			Network.Connect(iptemp, 2652);
		}

		GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height), "Server List", "Box");

		foreach(HostData match in MasterServer.PollHostList()) {
			GUILayout.BeginHorizontal("Box");

			GUILayout.Label(match.gameName);
			if(GUILayout.Button("Connect")) {
				Network.Connect(match);
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
	}

	private void Menu_HostGame() {
		//Buttons for Host Game
		if(GUI.Button(new Rect(10, 10, 200, 50), "Back")) {
			NavigateTo("Main");
		}

		if (GUI.Button(new Rect(10, 60, 200, 50), "Start Server")) {
			MultiplayerManager.instance.StartServer(matchName, matchPassword, maxPlayers);
		}

		if (GUI.Button(new Rect(10, 160, 200, 50), "Choose Map")) {
			NavigateTo("SelMap");
		}


		GUI.Label(new Rect(220, 10, 130, 30), "Match Name");
		matchName = GUI.TextField(new Rect(400, 10, 200, 30), matchName);

		GUI.Label(new Rect(220, 50, 130, 30), "Match Password");
		matchPassword = GUI.PasswordField(new Rect(400, 50, 200, 30), matchPassword, '*');

		/*GUI.Label(new Rect(220, 90, 130, 30), "Max Players");
		string maxUsers = GUI.TextField(new Rect(400, 90, 200, 30), maxPlayers.ToString());
		maxPlayers = int.Parse(maxUsers);
		maxPlayers = Mathf.Clamp(maxPlayers, 4, 8);*/

		if(GUI.Button(new Rect(220, 90, 100, 30), "4 Player Mode"))
			maxPlayers = 4;
		if(GUI.Button(new Rect(325, 90, 100, 30), "8 Player Mode"))
			maxPlayers = 8;

		GUI.Label(new Rect(650, 10, 130, 30), MultiplayerManager.instance.currentMap.mapName);
	}

	private void Menu_Lobby() {
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

		GUI.Label(new Rect(250, 55, 130, 30), "Choose Skin");
		GUILayout.BeginArea(new Rect(250, 86, 150, Screen.height - 90));
		
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

	private void Menu_ChooseMap() {
		if(GUI.Button(new Rect(10, 10, 200, 50), "Back")) {
			NavigateTo("Host");
		}

		GUI.Label(new Rect(220, 10, 130, 30), "Choose Map");
		GUILayout.BeginArea(new Rect(350, 10, 150, Screen.height));

		foreach(MapSettings map in MultiplayerManager.instance.MapList) {
			if(GUILayout.Button(map.mapName)) {
				NavigateTo("Host");
				MultiplayerManager.instance.currentMap = map;
			}
		}

		GUILayout.EndArea();
	}

	private void Menu_InGame() {

	}

	void OnConnectedToServer() {
		NavigateTo("Lobby");
	}

	void OnServerInitialized() {
		NavigateTo("Lobby");
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		NavigateTo("Main");
	}

	/*void OnLevelWasLoaded(int level) {
		if (level == 0) {
			Destroy(gameObject);
		} else if (level == 3) {
			Debug.Log("Entered Level 3");
			NavigateTo("Lobby");
		} else if (level == 4) {
			NavigateTo("InGame");
		}
		
	}*/
}
