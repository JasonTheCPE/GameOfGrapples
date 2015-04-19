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
	
	public Camera mainCam;
	public InputField levelNameInputText;
	public GameObject levelOptionsMenu;
	public GameObject sameNameWarningText;
	public GameObject needLevelNameWarningText;
	public GameObject buildingMenu;
	public GameObject tileSelectionBox;
	public LevelScaleManager levelScaleManager;
	
	public GameObject levelToEdit;
	public GameObject selectedTile;
	
	public void SetLevelName()
	{
		sameNameWarningText.SetActive(false);
		needLevelNameWarningText.SetActive(false);
		
		//only allow 'nice' characters in level names
		levelNameInputText.text = Regex.Replace(levelNameInputText.text, "[^\\w\\_]", "");
		
		//check existing level names
		DirectoryInfo levelDir = new DirectoryInfo("Assets/Resources/Levels");
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
		
		TileSelectionBox tileBox = tileSelectionBox.GetComponent<TileSelectionBox>();
		tileBox.SetupTileSelectionBox();
		
		levelToEdit.SetActive(true);
	}
	
	public void SetCurrentTile(string tileName)
	{
		if(selectedTile != null)
		{
			Destroy(selectedTile);
		}

		selectedTile = Instantiate(Resources.Load<GameObject>("Tiles/" + tileName));
		levelScaleManager.ScaleTile(selectedTile);
		selectedTile.SetActive(false);
	}
	
	public void DisplayTileAtCursor()
	{
		if(selectedTile != null)
		{
			selectedTile.SetActive(true);
			Vector3 newPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8f));
			newPos = levelScaleManager.SnapVector(3, newPos);
			selectedTile.transform.position = newPos;
		}
	}
	
	void Update()
	{
		if(tileSelectionBox.activeInHierarchy == false)
		{
			DisplayTileAtCursor();
		}
		else if(selectedTile != null)
		{
			selectedTile.SetActive(false);
		}
		
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
			if(selectedTile != null)
			{
				selectedTile.transform.Rotate(new Vector3(0f, 0f, 45.0f));
			}
		}
		
		if(Input.GetButtonDown("LevelEditor_PlaceTile"))
		{
		
		}
		else if(Input.GetButtonDown("LevelEditor_DeleteTile"))
		{
		
		}
	}
}
