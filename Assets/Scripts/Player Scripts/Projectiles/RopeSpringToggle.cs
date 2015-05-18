using UnityEngine;
using System.Collections;

public class RopeSpringToggle : MonoBehaviour {

	void OnNetworkInstantiate (NetworkMessageInfo msg) {
		if (GetComponent<NetworkView>().isMine)
		{

		}
		else
		{
			name += "Remote";
			SpringJoint2D _SpringJoint2D = GetComponent<SpringJoint2D>();
			_SpringJoint2D.enabled = false;

			DistanceJoint2D _DistanceJoint2D = GetComponent<DistanceJoint2D>();
			_DistanceJoint2D.enabled = false;
		}
	}

}
