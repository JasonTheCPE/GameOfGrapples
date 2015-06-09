using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class MultiplayerManager : MonoBehaviour {
	
	public static MultiplayerManager instance;	//holds a reference to itself
	private string matchName = "";				//name of the match
	private string matchPassword = "";			//password to enter the match
	private int matchMaxUsers = 4;				//max players in the match
	private string toLoad = "";					//stores the level to load in OnLevelWasLoaded
	private Object[] skins;						//an array to hold all the skins
	private int usingSkin = 0;					//an id for the skin you are currently using

	//variables related to the match
	public int matchTime = 60, matchHP = 1, matchAmmo = 4;	//match info
	public bool isMatchStarted = false;						//boolean for if the match has started
	public bool allowTeams = false;							//Whether theere will be teams in the game or not
	
	public string playerName = "Host Player";	//the player name the current player will have in the match
	public GameObject playerPrefab;				//a reference to the prefab (skin) the player wants to use
	
	public List<MyPlayer> PlayerList = new List<MyPlayer>();		//An array of players in the lobby

	//variables relating to maps
	public List<MapSettings> MapList = new List<MapSettings>();		//An array of all the maps
	private bool isCustom = false;									//a boolean for determining if the level 
																	//being loaded is a custom level or not
	public MapSettings currentMap = null;							//a reference to the current map
	
	public int oldPrefix;

	public List<MyPlayer> PreviousWinners = new List<MyPlayer>();

	public int inroom = 2;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Use this for initialization
	void Start () {
		instance = this;
		playerName = PlayerPrefs.GetString("Player Name");	//get saved player settings for their name
		
		DirectoryInfo levelDir = new DirectoryInfo("Assets/Resources/Levels");	//load all the levels
		FileInfo[] levelInfo = levelDir.GetFiles("*.*");
		
		foreach(FileInfo level in levelInfo)
		{
			if(level.Name.EndsWith(".xml"))
			{
				MapSettings newLevelDisplay = new MapSettings();
				newLevelDisplay.mapName = level.Name.Remove(level.Name.Length - 4);
				newLevelDisplay.mapLoadName = level.Name.Remove(level.Name.Length - 4);
				MapList.Add(newLevelDisplay);
			}
		}
		
		if(MapList.Count > 0)
			currentMap = MapList[0];
		
		skins = Resources.LoadAll("Skins");
	}
	
	//keep the instance alive
	void FixedUpdate() {
		instance = this;
	}
	
	// Start the server with the server name, max users, and a password (optional)
	public void StartServer (string serverName, string serverPassword, int maxUsers) {
		matchName = serverName;
		matchPassword = serverPassword;
		matchMaxUsers = maxUsers;
		
		Network.InitializeServer(matchMaxUsers - 1, 2386, false);			//set the max number of players, port number, and something else
		MasterServer.RegisterHost("Deathmatch", matchName, "No Comment");	//gametype, game name, and comment
		
		//Network.InitializeSecurity();	//breaks everything till further notice
	}
	
	void OnServerInitialized() {
		Server_PlayerJoinRequest(playerName, Network.player);	//add the host to the list
	}
	
	void OnConnectedToServer() {
		GetComponent<NetworkView>().RPC("Server_PlayerJoinRequest", RPCMode.Server, playerName, Network.player); //add new player to server
	}
	
	void OnPlayerDisconnected(NetworkPlayer id) {
		GetComponent<NetworkView>().RPC("Client_RemovePlayer", RPCMode.All, id);	//remove disconnected player from server
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		if (inroom > 3) {

		}else if (inroom == 3) {
			//if you want to be able to join a game in the prep step, uncomment out the code and start debugging
			/*
			//the newly connected player gets a list of all the other players
			foreach(MyPlayer pl in PlayerList) {
				GetComponent<NetworkView>().RPC("Client_AddPlayerToList", player, pl.playerName, pl.playerNetwork);
			}
			//tell the newly connected player the settings and map to load
			GetComponent<NetworkView>().RPC("Client_GetMultiplayerMatchSettings", player, currentMap.mapName, "", "");
			*/
		} else {
			//the newly connected player gets a list of all the other players
			foreach(MyPlayer pl in PlayerList) {
				GetComponent<NetworkView>().RPC("Client_AddPlayerToList", player, pl.playerName, pl.playerNetwork);
			}
			//tell the newly connected player the settings and map to load
			GetComponent<NetworkView>().RPC("Client_GetMultiplayerMatchSettings", player, currentMap.mapName, "", "");
		}
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		PlayerList.Clear();
	}
	
	[RPC]
	void Server_PlayerJoinRequest(string playerName, NetworkPlayer view) {
		//if you want players to be able to join in the prep room, remove the equal sign so that it is just >
		if (inroom >= 3) {
				Network.CloseConnection(view, true);
		} else if (inroom == 3) {
			//tell the person to go to the preproom when connected
			GetComponent<NetworkView>().RPC("ToPrepRoom", view);
			//tell everyone on server to add player to their list
			GetComponent<NetworkView>().RPC("Client_AddPlayerToList", RPCMode.All, playerName, view);
			//tell everyone on server to add player to their list
			GetComponent<NetworkView>().RPC("ResetLocks", RPCMode.All);
		} else {
			//tell everyone on server to add player to their list
			GetComponent<NetworkView>().RPC("Client_AddPlayerToList", RPCMode.All, playerName, view);
		}
	}

	[RPC]
	void ResetLocks() {
		GameObject.Find("MapSelection").GetComponent<MapSelector>().InitiateLocked();
	}
	
	[RPC]
	//adds player to the player list with the input playername and networkplayer
	void Client_AddPlayerToList(string playerName, NetworkPlayer view) {
		MyPlayer tempPlayer = new MyPlayer();
		tempPlayer.playerName = playerName;
		tempPlayer.playerNetwork = view;
		tempPlayer.skinID = 0;
		tempPlayer.wins = 0;
		PlayerList.Add(tempPlayer);
	}
	
	[RPC]
	//remove the plaer with the input networkplayer from the player list
	void Client_RemovePlayer(NetworkPlayer view) {
		MyPlayer tempPlayer = null;
		foreach(MyPlayer pl in PlayerList) {
			if(pl.playerNetwork == view) {
				tempPlayer = pl;
			}
		}
		
		if (tempPlayer != null) {
			PlayerList.Remove(tempPlayer);
		}
	}
	
	[RPC]
	void Client_GetMultiplayerMatchSettings(string map, string mode, string others) {
		currentMap = GetMap(map); //can add in mode later if I want to
	}
	
	[RPC]
	void SetSkin(int skinNum, NetworkPlayer view) {
		foreach(MyPlayer pl in PlayerList) {
			if(pl.playerNetwork == view) {
				pl.skinID = skinNum;
			}
		}
		
		if (view == Network.player)
			usingSkin = skinNum;
	}
	
	[RPC]
	public void AssignWin(NetworkPlayer view) {
		//if (GetComponent<NetworkView>().isMine) {
		Debug.Log("Assigning win");
		PreviousWinners.Clear();
		foreach(MyPlayer pl in PlayerList) {
			if(pl.playerNetwork == view) {
				++pl.wins;
				Debug.Log("Winner has " + pl.wins + " wins");
				PreviousWinners.Add(pl);
			}
		}
		//}
		Application.LoadLevel("BattleVictoryScene");
	}
	
	[RPC]
	void AssignTeamWin(int team) {
		//if (GetComponent<NetworkView>().isMine) {
		PreviousWinners.Clear();
		foreach(MyPlayer pl in PlayerList) {
			if(pl.team == team) {
				++pl.wins;
				PreviousWinners.Add(pl);
			}
		}
		//}
		Application.LoadLevel("BattleVictoryScene");
	}
	
	[RPC]
	void AssignDraw() {
		PreviousWinners.Clear();
		Application.LoadLevel("BattleVictoryScene");
	}
	
	[RPC]
	void ToPrepRoom() {
		Application.LoadLevel("Prep");
	}
	
	[RPC]
	void SetCurrentMap(string newMap) {
		foreach(MapSettings map in MapList) {
			if(map.mapName == newMap) {
				currentMap = map;
			}
		}
	}
	
	//can possibly make better by getting rid of get variable. we will see if i need it later
	public MapSettings GetMap(string name) {
		MapSettings get = null;
		
		foreach(MapSettings st in MapList) {
			if(st.mapName == name) {
				get = st;
				return get;
			}
			
		}
		
		return get;
	}
	
	[RPC]
	void Client_LoadMultiplayerMap(string map, int prefix) {
		Network.SetLevelPrefix(prefix); //make sure you only recieve new RPCs
		toLoad = map;
		isCustom = false;
		Application.LoadLevel("inmultiplayergame");
	}
	
	void OnLevelWasLoaded(int level) {
		inroom = level;
		if (level == 0) {
			Destroy(gameObject);
		} else if (level == 2) {
			GameObject.Find("LobbyMenu").GetComponent<LobbyMenuManager>().NavigateTo("Lobby");
		} else if (level == 3) {
			Debug.Log("Loading Level " + toLoad);
			LevelIOManager.ContructLevelInCanvasByName(GameObject.Find("Level"), toLoad, isCustom);

			if (matchTime == 0) {
				GameObject.Find("Ingame Manager").GetComponent<Referee>().isTimed = false;
			} else {
				GameObject.Find("Ingame Manager").GetComponent<Referee>().isTimed = true;
			}
			GameObject.Find("Ingame Manager").GetComponent<Referee>().timer = matchTime;
			spawnPlayer(usingSkin);
		} else if (level == 4) {
			if (Network.isServer) {
				Network.maxConnections = matchMaxUsers - 1;
			}
		}
		
	}
	
	int FindPlayerNumber() {
		for (int i = 0; i < PlayerList.Count; ++i) {
			if(PlayerList[i].playerNetwork == Network.player) {
				return i;
			}
		}
		
		return -1;
	}
	
	void spawnPlayer(int skinType) {
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		Vector3 spawnlocation;
		
		int playerNumber = FindPlayerNumber();
		
		if (playerNumber == -1) {
			Debug.Log("ERROR! Didn't find player number!");
			playerNumber = 0;
		}
		
		GameObject spawnPrefab = accessSkin(skinType);
		if (spawnPoints.Length == 0) {
			spawnlocation = new Vector3(0,0,0);
		} else {
			spawnlocation = spawnPoints[playerNumber%spawnPoints.Length].GetComponent<Transform>().position;
		}
		
		GameObject myPlayerGO = (GameObject)Network.Instantiate(spawnPrefab, spawnlocation, Quaternion.identity, 0);
		myPlayerGO.GetComponent<ID>().playerNumber = playerNumber;
		if (allowTeams) {
			myPlayerGO.GetComponent<ID>().teamID = PlayerList[playerNumber].team;
		} else {
			myPlayerGO.GetComponent<ID>().teamID = -1;
		}
		myPlayerGO.GetComponent<HoverName>().name = PlayerList[playerNumber].playerName;			//sets the name of the person?
		myPlayerGO.GetComponent<HoverName>().localName = PlayerList[playerNumber].playerName;

		myPlayerGO.GetComponent<Health>().InitHealth(matchHP);
		myPlayerGO.GetComponent<Throwing>().InitShurikens(matchAmmo);
	}
	
	public GameObject accessSkin(int skinID) {
		int arrayLength = skins.Length;
		
		for (int i = 0; i < arrayLength; i++) {
			GameObject caste = (GameObject)skins[i];
			if (skinID == caste.GetComponent<ID>().skinID)
			{
				Debug.Log(caste);
				return caste;
			}
		}
		
		return playerPrefab;
	}
	
	[RPC]
	void ToggleTeams() {
		allowTeams = !allowTeams;
	}
	
	[RPC]
	void SetTeam(NetworkPlayer view, int team) {
		foreach(MyPlayer pl in PlayerList) {
			if(pl.playerNetwork == view) {
				pl.team = team;
			}
		}
	}

	[RPC]
	void SetTime(int time) {
		matchTime = time;
	}

	[RPC]
	void SetHealth(int hp) {
		matchHP = hp;
	}

	[RPC]
	void SetAmmo(int am) {
		matchAmmo = am;
	}

	[RPC]
	void PostGameLockIn(NetworkPlayer player) {
		GameObject.Find("NinjaPlacer").GetComponent<PostGame>().LockIn(player);
	}

	[RPC]
	void PrepLockIn(NetworkPlayer player) {
		GameObject.Find("MapSelection").GetComponent<MapSelector>().LockIn(player);
	}

	public NetworkPlayer GetView() {
		return Network.player;
	}

}

[System.Serializable]
public class MyPlayer {
	public string playerName = "";
	public NetworkPlayer playerNetwork;
	public int skinID = 0;
	public int wins = 0;
	public int team = 0;
}

[System.Serializable]
public class MapSettings {
	public string mapName;
	public string mapLoadName;
	public Texture mapLoadTexture;
}