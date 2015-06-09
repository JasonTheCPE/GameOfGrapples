using UnityEngine;
using System.Collections;

public class HoverName : MonoBehaviour {
	public string localName = "";
	public string healthSring = "";
	public GUIStyle namePlate;
	private Vector3 namePlatePos;
	// Use this for initialization
	void Start () {
		ResetName();
	}

	public void ResetName () {
		if (GetComponent<NetworkView>().isMine) {
			SetName(localName, GetComponent<Health>().health.ToString());
			GetComponent<NetworkView>().RPC("SetName", RPCMode.Others, localName, healthSring);
			namePlate.normal.textColor = Color.cyan;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {
		// Place the name plate where the gameObject (player prefab) is
		namePlatePos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		GUI.Label(new Rect((namePlatePos.x - 25), (Screen.height - namePlatePos.y - 4*gameObject.transform.localScale.y), 100, 50), localName + " " + healthSring, namePlate);    
	}

	[RPC]
	void SetName(string name, string hs) {
		localName = name;
		healthSring = hs;
	}
}
