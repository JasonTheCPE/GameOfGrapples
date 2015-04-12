var speed: int = 5;
var gravity = 5;
private var cc: CharacterController;

var bulletPrefab:GameObject;

function Start() {
	cc = GetComponent(CharacterController);
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		cc.Move(Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
		if(Input.GetMouseButtonDown(0)){
            GetComponent(NetworkView).RPC("shoot", RPCMode.All);
    	}
	} else {
		enabled = false;
	}
}

@RPC
function shoot () {
	Network.Instantiate(bulletPrefab, transform.position, Quaternion.identity, 0);
    /*if (shotSound) audio.PlayOneShot(shotSound); // play the shot sound
    var hit: RaycastHit;
    if (Physics.Raycast(transform.position, transform.forward, hit))
	{
        // prepare rotation to instantiate blood or sparks
        var rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        if (hit.transform.tag == "Player"){ // if enemy hit...
            if (bloodPrefab) Instantiate(bloodPrefab, hit.point, rot); // make it bleed...
            //hit.transform.SendMessage("ApplyDamage", 5, SendMessageOptions.DontRequireReceiver); // and consume its health
            hit.transform.networkView.RPC("ApplyDamage", RPCMode.All, 5);  
        } else { // otherwise emit sparks at the hit point
            if (sparksPrefab) Instantiate(sparksPrefab, hit.point, rot);
        }
    }*/
}