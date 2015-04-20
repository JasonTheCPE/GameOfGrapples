using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LevelEditorManager : MonoBehaviour
{
	public string levelName = "";
	public int levelWidth;
	public int levelHeight;
	public float levelGravity;
	
	public Camera mainCam;
	public Canvas rootCanvas;
	public InputField levelNameInputText;
	public InputField levelWidthInputText;
	public InputField levelHeightInputText;
	public GameObject levelOptionsMenu;
	public GameObject sameNameWarningText;
	public GameObject needLevelNameWarningText;
	public GameObject invalidSizeWarningText;
	public GameObject buildingMenu;
	public GameObject tileSelectionBox;
	public LevelScaleManager levelScaleManager;
	
	public GameObject levelToEdit;
	public GameObject selectedTile;
	
	private bool placeTile = false;
	private bool delTile = false;
	private float lastPlacedX = Mathf.Infinity;
	private float lastPlacedY = Mathf.Infinity;
	
	void Start()
	{
		levelWidthInputText.text = "40";
		levelHeightInputText.text = "25";
	}
	
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
	
	public void SetLevelWidth()
	{
		invalidSizeWarningText.SetActive(false);
		int.TryParse(levelWidthInputText.text, out levelWidth);
		if(levelWidth < 10 || levelHeight < 10)
		{
			invalidSizeWarningText.SetActive(true);
		}
	}
	
	public void SetLevelHeight()
	{
		invalidSizeWarningText.SetActive(false);
		int.TryParse(levelHeightInputText.text, out levelHeight);
		if(levelWidth < 10 || levelHeight < 10)
		{
			invalidSizeWarningText.SetActive(true);
		}
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
		if(invalidSizeWarningText.activeSelf)
		{
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
		levelScaleManager.InitializeVoidBorders(rootCanvas);
		
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
		levelScaleManager.ScaleObjectWithSprite(selectedTile);
		selectedTile.SetActive(false);
	}
	
	private void DisplayTileAtCursor()
	{
		selectedTile.SetActive(true);
		Vector3 newPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8f));
		newPos = levelScaleManager.SnapVector(3, newPos);
		selectedTile.transform.position = newPos;
		levelScaleManager.KeepObjectWithSpriteInBounds(selectedTile);
	}
	
	private GameObject GetTileUnderCurrent()
	{
		selectedTile.SetActive(false);
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		selectedTile.SetActive(true);
		
		if(hit.collider != null && hit.collider.tag == "Tiles")
		{
			return hit.collider.transform.gameObject;
		}
		
		return (GameObject) null;
	}
	
	void Update()
	{
		if(Input.GetButtonDown("LevelEditor_ToggleMenu") || Input.GetButtonDown("Cancel"))
		{
			if(levelOptionsMenu.activeSelf == false)
			{
				buildingMenu.SetActive(!buildingMenu.activeSelf);
			}
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
		
		if(selectedTile != null)
		{
			if(tileSelectionBox.activeInHierarchy == false)
			{
				DisplayTileAtCursor();
				if(Input.GetButtonDown("LevelEditor_PlaceTile"))
				{
					placeTile = true;				
				}
				else if(Input.GetButtonDown("LevelEditor_DeleteTile"))
				{
					delTile = true;
				}
				
				if(placeTile)
				{
					var tileBounds = selectedTile.transform.localScale;
					var spriteBounds = selectedTile.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size;
					
					if(Mathf.Abs(lastPlacedX - selectedTile.transform.position.x) >= tileBounds.x * spriteBounds.x ||
					   Mathf.Abs(lastPlacedY - selectedTile.transform.position.y) >= tileBounds.y * spriteBounds.y)
					{
						GameObject tempTile = Instantiate(selectedTile);
						tempTile.transform.SetParent(levelToEdit.transform, true);
						lastPlacedX = selectedTile.transform.position.x;
						lastPlacedY = selectedTile.transform.position.y;
					}
					
				}
				else if(delTile)
				{
					GameObject tempTile = GetTileUnderCurrent();
					if(tempTile != null)
					{
						Destroy(tempTile);
					}
				}
			}
			else
			{
				selectedTile.SetActive(false);
			}
		}
		
		if(Input.GetButtonUp("LevelEditor_PlaceTile"))
		{
			placeTile = false;
			lastPlacedX = Mathf.Infinity;
			lastPlacedY = Mathf.Infinity;
		}
		if(Input.GetButtonUp("LevelEditor_DeleteTile"))
		{
			delTile = false;
		}
	}
}
