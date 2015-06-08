using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillPanelWithSelections : MonoBehaviour
{
	public string resourcesDirectory;
	private Object[] options;
	
	public GameObject buttonPrefab;
	public float vertSpaceRatio;
	private float buttonHeight;
	private float spaceBtwnButtons;
	
	public RectTransform panel;
	public float minimumPanelSize;
	
	public ButtonSelection buttonSelection;
	
	// Use this for initialization
	void Start ()
	{
		options = Resources.LoadAll(resourcesDirectory);
		
		buttonHeight = buttonPrefab.GetComponent<RectTransform>().rect.height;
		spaceBtwnButtons = buttonHeight * vertSpaceRatio;

		float heightOfPanel = options.Length * spaceBtwnButtons;
		if(heightOfPanel < minimumPanelSize)
		{
			heightOfPanel = minimumPanelSize;
		}
		panel.transform.GetComponentInParent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightOfPanel);
		
		float yPos = panel.position.y - spaceBtwnButtons / 2;
		
		foreach(Object o in options)
		{
			CreateButton(o.name, yPos, () => buttonSelection.SelectOption(o.name));
			yPos -= spaceBtwnButtons;
		}
	}
	
	public void CreateButton(string buttonText, float yPos, UnityEngine.Events.UnityAction method)
	{
		GameObject button = Instantiate(buttonPrefab) as GameObject;
		button.GetComponent<Button>().onClick.AddListener(method);
		button.GetComponentInChildren<Text>().text = buttonText;
		button.transform.SetParent(panel.transform);
		button.transform.position = new Vector3(panel.position.x, yPos, 0);
	}
}
