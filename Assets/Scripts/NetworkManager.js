#pragma strict

var playerPrefab:GameObject;
var spawnObject:Transform;
var gameName:String = "HowDoMerps"; //a unique game name

private var refreshing: boolean;
private var hostdata: HostData[];

private var btnX: float;
private var btnY: float;
private var btnW: float;
private var btnH: float;

function Start () {
	btnX = Screen.width * 0.05;
	btnY = Screen.width * 0.05;
	btnW = Screen.width * 0.1;
	btnH = Screen.width * 0.1;
}

function startServer() {
	var useNat = !Network.HavePublicAddress();
	Network.InitializeServer(32, 24508, useNat); //initializes server on port 24508 with 32 players
	MasterServer.RegisterHost(gameName, "Tutorial game networks", "This is a tutorial game"); //register as a host on the server
}

function refreshHostList() {
	MasterServer.ClearHostList();			//clear the host list before requesting host list
	MasterServer.RequestHostList(gameName); //request a host from the server
	refreshing = true;
	//MasterServer.PollHostList();			//gets the list of all hosts on the server
}

function spawnPlayer() {
	Network.Instantiate(playerPrefab, spawnObject.position, Quaternion.identity, 0);
}

//Messages
function OnServerInitialized() {	//premade function that is called when the server is initialized
	Debug.Log("Server initialized!");
	spawnPlayer();
}

function OnConnectedToServer(){
	spawnPlayer();
}

function OnMasterServerEvent(msEvent: MasterServerEvent) {	//premade function that is called on an event received from the master server
	if (msEvent == MasterServerEvent.RegistrationSucceeded) {
		Debug.Log("Server registered");
	}
	
	//way to display when you receive a host list
	if(msEvent == MasterServerEvent.HostListReceived)
		Debug.Log("Games Found: " + MasterServer.PollHostList().Length);
}

//GUI
function OnGUI () {
	if (!Network.isClient && !Network.isServer) {
		if (GUI.Button(Rect(btnX, btnY, btnW, btnH), "Start Server")) {
			Debug.Log("starting server");
			startServer();
		}
		
		if (GUI.Button(Rect(btnX, btnY*1.2 + btnH, btnW, btnH), "Refresh Hosts")) {
			Debug.Log("refreshing");
			refreshHostList();
		}
		
		if (hostdata) {
			for(var i:int = 0; i < hostdata.length; i++) {
				if(GUI.Button(Rect(btnX*1.5 + btnW, btnY*1.2 + (btnH * i), btnW*3, btnH*0.5), hostdata[i].gameName)) {
					Network.Connect(hostdata[i]);
				}
			}
		}
	}
}

function Update() {
	if(refreshing) {
		if(MasterServer.PollHostList().Length > 0) {
			refreshing = false;
			Debug.Log(MasterServer.PollHostList().Length);
			hostdata = MasterServer.PollHostList();
		}
	}
}