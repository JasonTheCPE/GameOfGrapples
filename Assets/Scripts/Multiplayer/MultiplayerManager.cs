using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class MultiplayerManager : MonoBehaviour {
	
	public static MultiplayerManager instance;
	private string matchName = "";				//name of the match
	private string matchPassword = "";			//password to enter the match
	private int matchMaxUsers = 4;				//max players in the match
	private string toLoad = "";					//stores the level to load in OnLevelWasLoaded
	private bool isCustom = false;
	private Object[] skins;
	private int usingSkin = 0;

	public int matchTime = 60, matchHP = 1;
	
	public string playerName = "Host Player";	//the player name the current player will have in the match
	public GameObject playerPrefab;
	
	public List<MyPlayer> PlayerList = new List<MyPlayer>();
	public List<MapSettings> MapList = new List<MapSettings>();
	
	public MapSettings currentMap = null;
	
	public int oldPrefix;
	public bool isMatchStarted = false;
	
	public bool allowTeams = false;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Use this for initialization
	void Start () {
		instance = this;
		playerName = PlayerPrefs.GetString("Player Name");	//get saved player settings for their name
		
		DirectoryInfo levelDir = new DirectoryInfo("Assets/Resources/Levels");
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
		
		Network.InitializeServer(matchMaxUsers - 1, 2652, false);			//set the max number of players, port number, and something else
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
		//the newly connected player gets a list of all the other players
		foreach(MyPlayer pl in PlayerList) {
			GetComponent<NetworkView>().RPC("Client_AddPlayerToList", player, pl.playerName, pl.playerNetwork);
		}
		//tell the newly connected player the settings and map to load
		GetComponent<NetworkView>().RPC("Client_GetMultiplayerMatchSettings", player, currentMap.mapName, "", "");
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		PlayerList.Clear();
	}
	
	[RPC]
	void Server_PlayerJoinRequest(string playerName, NetworkPlayer view) {
		//tell everyone on server to add player to their list
		GetComponent<NetworkView>().RPC("Client_AddPlayerToList", RPCMode.All, playerName, view);
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
	void AssignWin(NetworkPlayer view) {
		foreach(MyPlayer pl in PlayerList) {
			if(pl.playerNetwork == view) {
				++pl.wins;
			}
		}
		Application.LoadLevel("Prep");
	}
	
	[RPC]
	void AssignTeamWin(int team) {
		foreach(MyPlayer pl in PlayerList) {
			if(pl.team == team) {
				++pl.wins;
			}
		}
		Application.LoadLevel("Prep");
	}
	
	[RPC]
	void AssignDraw() {
		Application.LoadLevel("Prep");
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
		if (level == 0) {
			Destroy(gameObject);
		} else if (level == 3) {
			GameObject.Find("LobbyMenu").GetComponent<LobbyMenuManager>().NavigateTo("Lobby");
		} else if (level == 4) {
			Debug.Log("Loading Level " + toLoad);
			LevelIOManager.ContructLevelInCanvasByName(GameObject.Find("Level"), toLoad, isCustom);

			if (matchTime == 0) {
				GameObject.Find("Ingame Manager").GetComponent<Referee>().isTimed = false;
			} else {
				GameObject.Find("Ingame Manager").GetComponent<Referee>().isTimed = true;
			}
			GameObject.Find("Ingame Manager").GetComponent<Referee>().timer = matchTime;
			GameObject.Find("Ingame Manager").GetComponent<Referee>().startHealth = matchHP;
			spawnPlayer(usingSkin);
		} else if (level == 5) {
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
	}
	
	GameObject accessSkin(int skinID) {
		int arrayLength = skins.Length;
		
		for (int i = 0; i < arrayLength; i++) {
			GameObject caste = (GameObject)skins[i];
			if (skinID == caste.GetComponent<ID>().skinID) 
				return caste;
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