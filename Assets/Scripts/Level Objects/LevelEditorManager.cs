using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LevelEditorManager : MonoBehaviour
{
	public string levelName = "";
	public int levelWidth = 40;
	public int levelHeight = 25;
	public float levelGravity;
	
	public InputField levelNameInputText;
	public GameObject levelOptionsMenu;
	public GameObject sameNameWarningText;
	public GameObject needLevelNameWarningText;
	public GameObject buildingMenu;
	
	public GameObject levelToEdit;
	
	public void SetLevelName()
	{
		sameNameWarningText.SetActive(false);
		needLevelNameWarningText.SetActive(false);
		
		//only allow 'nice' characters in level names
		levelNameInputText.text = Regex.Replace(levelNameInputText.text, "[^\\w\\_]", "");
		
		//check existing level names
		DirectoryInfo levelDir = new DirectoryInfo("Assets/Levels");
		FileInfo[] levelInfo = levelDir.GetFiles("*.*");
		
		foreach(FileInfo f in levelInfo)
		{
			if(f.Name.Equals(levelNameInputText.text + ".xml"))
			{
				sameNameWarningText.SetActive(true);
				levelName = "";
				return;
			}
		}
		
		levelName = levelNameInputText.text;
	}
	
	public void CloseEditOptions()
	{
		if(sameNameWarningText.activeSelf)
		{
			return;
		}
		needLevelNameWarningText.SetActive(false);
		if(levelName.Equals(""))
		{
			needLevelNameWarningText.SetActive(true);
			return;
		}
		levelOptionsMenu.SetActive(false);
		buildingMenu.SetActive(true);
		PrepareEditor();
	}
	
	private void PrepareEditor()
	{
		LevelIOManager levelIOManager = levelToEdit.GetComponent<LevelIOManager>();
		levelIOManager.levelName = this.levelName;
		levelIOManager.levelWidth = this.levelWidth;
		levelIOManager.levelHeight = this.levelHeight;
		levelIOManager.levelGravity = this.levelGravity;
		
		LevelScaleManager levelScaleManager = levelToEdit.GetComponent<LevelScaleManager>();
		levelScaleManager.levelWidth = this.levelWidth;
		levelScaleManager.levelHeight = this.levelHeight;
		levelScaleManager.CalculateScales();
		
		GridLines gridLines = levelToEdit.GetComponent<GridLines>();
		gridLines.scaledUnitSize = levelScaleManager.scaledUnitSize;
		gridLines.levelWidth = this.levelWidth;
		gridLines.levelHeight = this.levelHeight;
		gridLines.InitializeGrid();
		
		levelToEdit.SetActive(true);
	}
	
	void Update()
	{
		if(Input.GetButtonDown("LevelEditor_ToggleMenu") || Input.GetButtonDown("Cancel"))
		{
			buildingMenu.SetActive(!buildingMenu.activeSelf);
		}
		
		if(Input.GetButtonDown("LevelEditor_ToggleGridLines"))
		{
			GridLines gridLines = levelToEdit.GetComponent<GridLines>();
			gridLines.gridActive = !gridLines.gridActive; 
		}
		
		if(Input.GetButtonDown("LevelEditor_RotateTile"))
		{
			//TODO
		}
	}
}
