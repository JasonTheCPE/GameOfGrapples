using UnityEngine;
using System.Collections;

public class PlayGameManager : MonoBehaviour
{
	public string selectedLevel = "";
	public LevelScaleManager levelScaleManager;
	public GameObject levelCanvas;
	public GameObject levelSelectMenu;
	public GameObject pauseMenu;
	
	public Level levelToLoad;
	
	//Called by buttons on the Level Select menu.
	public void SelectLevel(string levelName)
	{
		selectedLevel = levelName;
	}
	
	//Loads the last clicked Level from the Level Select menu.
	public void LoadSelectedLevel()
	{
		if(selectedLevel.Equals(""))
		{
			return;
		}
		
		levelToLoad = LevelIOManager.LoadLevel(selectedLevel, false);
		levelSelectMenu.SetActive(false);
		LevelIOManager.ConstructLevelInCanvas(levelCanvas, levelToLoad);
	}
}
