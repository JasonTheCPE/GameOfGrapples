using UnityEngine;
using System.Collections;

public class HoverName : MonoBehaviour {
	public string localName = "";
	public GUIStyle namePlate;
	private Vector3 namePlatePos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Place the name plate where the gameObject (player prefab) is
		namePlatePos = Camera.main.WorldToScreenPoint(gameObject.transform.position);    
		GUI.Label(new Rect((namePlatePos.x-50), (Screen.height - namePlatePos.y+10), 100, 50), localName, namePlate);    
	}
}
