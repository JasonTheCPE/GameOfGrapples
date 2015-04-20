using UnityEngine;
using System.Collections;

public class PlayGameManager : MonoBehaviour
{
	public string selectedLevel = "";
	public LevelIOManager levelIOManager;
	public LevelScaleManager levelScaleManager;
	public GameObject levelCanvas;
	public GameObject levelSelectMenu;
	public GameObject pauseMenu;
	
	public Level loadedLevel;
	public string levelName;
	public int levelWidth;
	public int levelHeight;
	public float levelGravity;
	public string levelBorder;
	
	public void SelectLevel(string levelName)
	{
		selectedLevel = levelName;
	}
	
	public void LoadSelectedLevel()
	{
		if(selectedLevel.Equals(""))
		{
			return;
		}
		
		levelIOManager.levelName = selectedLevel;
		loadedLevel = levelIOManager.LoadLevel();
		levelSelectMenu.SetActive(false);
		ConstructLevel();
	}
	
	private void ConstructLevel()
	{
		this.levelName = loadedLevel.levelName;
		this.levelWidth = loadedLevel.levelWidth;
		this.levelHeight = loadedLevel.levelHeight;
		this.levelGravity = loadedLevel.levelGravity;
		this.levelBorder = loadedLevel.levelBorder;
		
		foreach(Level.Tile tile in loadedLevel.tiles)
		{
			GameObject newTile = Instantiate(Resources.Load<GameObject>("Tiles/" + tile.prefab));
			newTile.transform.SetParent(levelCanvas.transform, true);
			newTile.transform.position = new Vector3(tile.posX, tile.posY, 0f);
			newTile.transform.localScale = new Vector3(tile.scaleX, tile.scaleY, 1f);
			newTile.transform.Rotate(new Vector3(0f, 0f, tile.rot));
		}
		levelCanvas.SetActive(true);
	}
}
