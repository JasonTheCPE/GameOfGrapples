var speed: int = 5;
var gravity = 5;
private var cc: CharacterController;

function Start() {
	cc = GetComponent(CharacterController);
}

function Update(){
	if (GetComponent(NetworkView).isMine) {
		cc.Move(Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
	} else {
		enabled = false;
	}
}

/*public class playercontroller : MonoBehaviour {
	public float speed;
	private int count = 0;
	public GUIText countText;
	public GUIText winText;

	void Start () {
		count = 0;
		winText.text = "";
		countText.text = "Count: " + count.ToString();
	}

	void FixedUpdate() 
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		GetComponent<Rigidbody>().AddForce (movement*speed*Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
			count++;
			SetCountText ();
		}
	}

	void SetCountText (){
		countText.text = "Count: " + count.ToString();
		if (count >= 10) {
			winText.text = "You WIN!";
		}
	}
} */