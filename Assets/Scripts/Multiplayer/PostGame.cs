using UnityEngine;
using System.Collections;

public class PostGame : MonoBehaviour {
	private MultiplayerManager mm;
	private string mode = "";

	public bool hasLocked = false;
	public bool[] playersLocked;
	public int playersUnlocked;
	public int MaxTimeWaiting = 30;

	private const int winner = 0;
	private const int loser = 1;
	private const int meh = 2;
	private double timer = 0;

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
				/*foreach(MyPlayer mp in mm.PlayerWinOrder) {
					GameObject skin;
					skin = CreateSkin(mp.skinID, GameObject.Find("Tie " + i.ToString() + " Spawn"), mp.playerName, mp.wins, meh);
					++i;
				}*/
				foreach(MyPlayer mp in mm.PlayerList) {
					GameObject skin = CreateSkin(mp.skinID,  GameObject.Find("Tie " + i.ToString() + " Spawn"), mp.playerName, mp.wins, meh);
					++i;
				}
			} else if (mm.allowTeams) {
				mode = "Team-Game";
				foreach(MyPlayer mp in mm.PreviousWinners) {
					GameObject skin = CreateSkin(mp.skinID, GameObject.Find("Team " + i.ToString() + " Spawn"), mp.playerName, mp.wins, winner);
					++i;
				}
			} else {
				mode = "Free-For-All";
				for (i = 1; i < 9; ++i) {
					MyPlayer mp = mm.PlayerWinOrder[i - 1];
					GameObject skin;
					if (mp != null){
						if (i == 1)
							skin = CreateSkin(mp.skinID, GameObject.Find("Free " + i.ToString() + " Spawn"), mp.playerName, mp.wins, winner);
						else
							skin = CreateSkin(mp.skinID, GameObject.Find("Free " + i.ToString() + " Spawn"), mp.playerName, mp.wins, loser);
					}
				}
				/*foreach(MyPlayer mp in mm.PlayerList) {
					GameObject skin;
					if (mp.playerNetwork != mm.PreviousWinners[0].playerNetwork) {
						skin = CreateSkin(mp.skinID, GameObject.Find("Loser " + i.ToString() + " Spawn"), mp.playerName, mp.wins, loser);
						++i;
					} else {
						skin = CreateSkin(mp.skinID, GameObject.Find("Winner Spawn"), mp.playerName, mp.wins, winner);
					}
				}*/
			}
		} else {
			Debug.Log("ERROR! Could not find the multiplayer manager!");
		}
		
		Debug.Log("It was a " + mode);
	}
	
	GameObject CreateSkin(int id, GameObject baseObject, string name, int numWins, int reaction) {
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
		ret.GetComponent<HoverName>().healthSring = "";						//display your health
		ret.GetComponent<NetworkRigidbody2D>().enabled = false;				//turn off networkrigidbody2D
		ret.GetComponent<Throwing>().enabled = false;						//turn off throwing
		ret.transform.position = baseObject.transform.position;
		ret.transform.localScale = baseObject.transform.localScale*10;
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
		timer += Time.deltaTime;

		//check if everyone had been at the victory screen too long
		if (timer  > MaxTimeWaiting) {
			//Application.LoadLevel("Prep");
			mm.GetComponent<NetworkView>().RPC("ToPrepRoom", RPCMode.All);
		}
	}

	public void ButtonClicked() {
		if (!hasLocked) {
			hasLocked = true;
			mm.GetComponent<NetworkView>().RPC("PostGameLockIn", RPCMode.All, Network.player);
			GameObject.Find("Ready Button").SetActive(false);
		}
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Destroy(GameObject.Find("Multiplayer Manager"));
		Application.LoadLevel("Lobby");
	}
}
