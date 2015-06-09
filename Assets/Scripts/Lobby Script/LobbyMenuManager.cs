using UnityEngine;
using System.Collections;

public class LobbyMenuManager : MonoBehaviour {

	public string currentMenu; 
	public string matchName;
	public string matchPassword;
	public int maxPlayers = 4;
	public LobbyMenuManager instance;

	private Vector2 scrollLobby = Vector2.zero;
	private string iptemp = "127.0.0.1";

	// Use this for initialization
	void Start () {
		currentMenu = "Main";								//start off in the main menu upon level start
		matchName = "Temp Name: " + Random.Range(0, 5000);	//setup a temp name that most likely won't show up again
		instance = this;
	}

	//keep the instance alive
	void FixedUpdate() {
		instance = this;
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
			MasterServer.RequestHostList("Deathmatch");	//Look for all games deemed "Deathmatch"
		}

		GUI.Label(new Rect(220, 10, 130, 30), "Player Name");
		MultiplayerManager.instance.playerName = GUI.TextField(new Rect(350, 10, 150, 30), MultiplayerManager.instance.playerName);
		if (MultiplayerManager.instance.playerName.Length > 15) {
			MultiplayerManager.instance.playerName = MultiplayerManager.instance.playerName.Substring(0, 15);
		}

		if(GUI.Button(new Rect(510, 10, 100, 30), "Save Name")) {
			PlayerPrefs.SetString("Player Name", MultiplayerManager.instance.playerName);	//save the user name in the player prefs
		}

		GUI.Label(new Rect(220, 50, 130, 30), "Direct Connect");
		iptemp = GUI.TextField(new Rect(350, 50, 150, 30), iptemp);
		
		if(GUI.Button(new Rect(510, 50, 100, 30), "Connect")) {
			Network.Connect(iptemp, 2652);	//connect to the newtwork
		}

		GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height), "Server List", "Box");

		//display all the matches we found and allow the player to connect to them
		foreach(HostData match in MasterServer.PollHostList()) {
			GUILayout.BeginHorizontal("Box");

			if (match.connectedPlayers < match.playerLimit) {
				GUILayout.Label(match.gameName + ": " + match.connectedPlayers.ToString() + "/" + match.playerLimit.ToString());
				if(GUILayout.Button("Connect")) {
					Network.Connect(match);
				}
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();

		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), "Back")) {
			Application.LoadLevel("MainMenu");
		}
	}

	private void Menu_HostGame() {
		//Buttons for Host Game
		if(GUI.Button(new Rect(10, 10, 200, 50), "Back")) {
			MultiplayerManager.instance.PlayerList.Clear();
			NavigateTo("Main");
		}

		if (GUI.Button(new Rect(10, 60, 200, 50), "Start Server")) {
			MultiplayerManager.instance.StartServer(matchName, matchPassword, maxPlayers);
		}

		GUI.Label(new Rect(220, 10, 130, 30), "Match Name");
		matchName = GUI.TextField(new Rect(400, 10, 200, 30), matchName);
		if (matchName.Length > 20) {
			matchName = matchName.Substring(0, 20);
		}

		GUI.Label(new Rect(220, 50, 130, 30), "Match Password");
		matchPassword = GUI.PasswordField(new Rect(400, 50, 200, 30), matchPassword, '*');

		if(GUI.Button(new Rect(220, 90, 100, 30), "4 Player Mode"))
			maxPlayers = 4;
		if(GUI.Button(new Rect(325, 90, 100, 30), "8 Player Mode"))
			maxPlayers = 8;
	}

	private void Menu_Lobby() {
		scrollLobby = GUILayout.BeginScrollView(scrollLobby, GUILayout.MaxWidth(200));

		foreach(MyPlayer pl in MultiplayerManager.instance.PlayerList) {
			if (pl.playerNetwork == Network.player) {
				GUI.color = Color.cyan;
			}
			GUILayout.Box(pl.playerName);
			GUI.color = Color.white;
		}

		GUILayout.EndScrollView();

		if (Network.isServer) {
			if(GUI.Button(new Rect(Screen.width - 405, Screen.height - 40, 200, 40), "Enter Dojo")) {
				//Network.maxConnections = 0; // TODO Stops new players from joining once in preproom
				MultiplayerManager.instance.GetComponent<NetworkView>().RPC("ToPrepRoom", RPCMode.All);
			}
		}

		if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Disconnect")) {
			Network.Disconnect();
		}
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
}
