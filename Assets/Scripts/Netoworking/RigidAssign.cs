using UnityEngine;
using System.Collections;

public class RigidAssign : MonoBehaviour
{
	
	void OnNetworkInstantiate (NetworkMessageInfo msg) {
		if (GetComponent<NetworkView>().isMine)
		{
			NetworkRigidbody2D _NetworkRigidbody = GetComponent<NetworkRigidbody2D>();
			_NetworkRigidbody.enabled = false;
		}
		else
		{
			name += "Remote";
			NetworkRigidbody2D _NetworkRigidbody2 = GetComponent<NetworkRigidbody2D>();
			_NetworkRigidbody2.enabled = true;
		}
	}
}