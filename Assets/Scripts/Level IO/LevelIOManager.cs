using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public static class LevelIOManager
{	
	public const string customLevelDir = "Assets/Resources/Levels/Custom";
	public const string builtInLevelDir = "Assets/Resources/Levels/BuiltIn";
	
	//Builds a Level XML out of the layout of the level set up inside levelCanvas 
	//and the given Level properties.
	public static void BuildLevelPrimitiveAndSaveLevel(GameObject levelCanvas, string levelName, int levelWidth, int levelHeight, float levelGravity, string levelBorder)
	{
		Level levelToSave = new Level();
		
		levelToSave.tiles = new List<Level.Tile>();
		
		foreach(Transform child in levelCanvas.transform)
		{
			if(child.tag == "Tiles")
			{
				Level.Tile tile = new Level.Tile();
				
				//Truncates the ending (clone) from tiles
				tile.prefab = child.name.Split('(')[0];
				tile.posX = child.transform.localPosition.x; //make function to turn into absolute position
				tile.posY = child.transform.localPosition.y;
				tile.rot = child.transform.rotation.eulerAngles.z;
				tile.scaleX = child.localScale.x; //make function to find absolute scale
				tile.scaleY = child.localScale.y;
				
				levelToSave.tiles.Add(tile);
			}
		}
		Debug.Log("Level contains " + levelToSave.tiles.Count + " tiles");
		SaveLevel(levelToSave);
	}
	
	//Given the name of a Level and whether or not it is a custom Level, returns its resource path.
	public static string GetLevelPath(string levelName, bool isCustom)
	{
		if(isCustom)
		{
			return Path.Combine(customLevelDir, levelName + ".xml");
		}
		return Path.Combine(builtInLevelDir, levelName + ".xml");
	}
	
	//Saves the given Level as a custom Level.
	public static void SaveLevel(Level levelToSave)
	{
		var serializer = new XmlSerializer(typeof(Level));
		using(var stream = new FileStream(GetLevelPath(levelToSave.levelName, true), FileMode.Create))
		{
			serializer.Serialize(stream, levelToSave);
		}
	}
	
	//Loads and returns the Level with the name levelName and status as a custom Level or not.
	public static Level LoadLevel(string levelName, bool isCustom)
	{
		var serializer = new XmlSerializer(typeof(Level));
		using(var stream = new FileStream(GetLevelPath(levelName, isCustom), FileMode.Open))
		{
			return serializer.Deserialize(stream) as Level;
		}
	}
	
	//Loads the XML directly from the given string. Useful in combination with www.text.
	public static Level LoadLevelFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(Level));
		return serializer.Deserialize(new StringReader(text)) as Level;
	}
	
	//Adds all the tiles from the given Level into the given Level Canvas and 
	//adds and initializes its LevelScaleManager.
	public static void ConstructLevelInCanvas(GameObject levelCanvas, Level level)
	{
		levelCanvas.AddComponent<LevelScaleManager>();
		levelCanvas.GetComponent<LevelScaleManager>().InitializeLevelScaleManager(level.levelWidth, level.levelHeight);
	
		levelCanvas.GetComponent<LevelScaleManager>().InitializeVoidBorders(levelCanvas);
	
		foreach(Level.Tile tile in level.tiles)
		{
			GameObject newTile = Object.Instantiate(Resources.Load<GameObject>("Tiles/" + tile.prefab));
			newTile.transform.SetParent(levelCanvas.transform, true);
			newTile.transform.position = new Vector3(tile.posX, tile.posY, 0f);
			newTile.transform.localScale = new Vector3(tile.scaleX, tile.scaleY, 1f);
			newTile.transform.Rotate(new Vector3(0f, 0f, tile.rot));
		}
		levelCanvas.SetActive(true);
	}
	
	public static void ContructLevelInCanvasByName(GameObject levelCanvas, string levelName, bool isCustom)
	{
		Level levelToLoad = LevelIOManager.LoadLevel(levelName, isCustom);
		ConstructLevelInCanvas(levelCanvas, levelToLoad);
	}
}