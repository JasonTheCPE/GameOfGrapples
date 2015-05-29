using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillPanelWithSelections : MonoBehaviour
{
	public string resourcesDirectory;
	public Object[] options;
	
	public Color buttonColor;
	public Color selectedButtonColor;
	public Color buttonFontColor;
	public Font buttonFont;
	public Image buttonTexture;
	public float buttonHeight;
	public float buttonWidth;
	
	public RectTransform panel;
	public float minimumPanelSize;
	
	// Use this for initialization
	void Start ()
	{
		panel = GetComponent<RectTransform>();
		options = Resources.LoadAll(resourcesDirectory);
		//Button newButton = Button(new Rect(0, 0, buttonWidth, buttonHeight), "HELLO");
		CreateButton(panel.transform, new Vector3(0f, 0f, -1f),
		             new Vector2(buttonWidth, buttonHeight), () => EventFn(options[0].name));
		//Button(new Rect(0, 0, buttonWidth, buttonHeight), "HELLO")
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void CreateButton(Transform panel ,Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
	{
		GameObject button = new GameObject();
		button.transform.parent = panel;
		button.AddComponent<RectTransform>();
		button.AddComponent<Image>();
		button.AddComponent<Button>();
		button.transform.position = position;
		button.GetComponent<RectTransform>().sizeDelta = size;
		button.GetComponent<Button>().onClick.AddListener(method);
	}
	
	public void EventFn(string str)
	{
		Debug.Log(str);
	}
}
