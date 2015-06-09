using UnityEngine;
using System.Collections;

public class ButtonSelection : MonoBehaviour
{
	public string optionSelected;
	
	public void SelectOption(string selected)
	{
		optionSelected = selected;
	}
}
