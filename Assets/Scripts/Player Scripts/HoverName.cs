using UnityEngine;
using System.Collections;

public class HoverName : MonoBehaviour {
	public string localName = "";
	public GUIStyle namePlate;
	private Vector3 namePlatePos;
	// Use this for initialization
	void Start () {
		if (GetComponent<NetworkView>().isMine) {
			GetComponent<NetworkView>().RPC("SetName", RPCMode.Others, localName);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {
		// Place the name plate where the gameObject (player prefab) is
		namePlatePos = Camera.main.WorldToScreenPoint(gameObject.transform.position);  
		GUI.Label(new Rect((namePlatePos.x - 25), (Screen.height - namePlatePos.y - 4*gameObject.transform.localScale.y), 100, 50), localName, namePlate);    
	}

	[RPC]
	void SetName(string name) {
		localName = name;
	}
}
