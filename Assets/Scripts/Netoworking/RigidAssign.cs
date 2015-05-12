using UnityEngine;
using System.Collections;

public class RigidAssign : MonoBehaviour
{
	
	void OnNetworkInstantiate (NetworkMessageInfo msg) {
		if (GetComponent<NetworkView>().isMine)
		{
			NetworkRigidbody _NetworkRigidbody = GetComponent<NetworkRigidbody>();
			_NetworkRigidbody.enabled = false;
		}
		else
		{
			name += "Remote";
			NetworkRigidbody _NetworkRigidbody2 = GetComponent<NetworkRigidbody>();
			_NetworkRigidbody2.enabled = true;
		}
	}
}