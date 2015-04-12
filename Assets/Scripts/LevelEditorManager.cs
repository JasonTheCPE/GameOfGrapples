using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour {

	public string levelName = "";
	public int levelWidth = 40;
	public int levelHeight = 25;
	
	public InputField levelNameInputText;
	public GameObject levelOptionsMenu;
	public GameObject sameNameWarningText;
	public GameObject needLevelNameWarningText;
	
	public void SetLevelName()
	{
		sameNameWarningText.SetActive(false);
		needLevelNameWarningText.SetActive(false);
		//check existing level names
		DirectoryInfo levelDir = new DirectoryInfo("Assets/Levels");
		FileInfo[] levelInfo = levelDir.GetFiles("*.*");
		
		foreach(FileInfo f in levelInfo)
		{
			if(f.Name.Equals(levelNameInputText.text))
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
	}
}
