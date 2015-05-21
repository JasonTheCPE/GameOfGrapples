using UnityEngine;
using System.Collections;

public class EyeMover : MonoBehaviour {

	public Vector3 basePos;
	public float mult = 1;
	public Vector3 eyeVector;
	// Use this for initialization
	void Start () {
		basePos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		eyeVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		eyeVector = eyeVector - transform.position;
		
		lookAt(eyeVector);
	}

	void lookAt(Vector3 dir) {
		Vector3 newPos = basePos + dir*mult;
		transform.position = newPos;
	}
}
