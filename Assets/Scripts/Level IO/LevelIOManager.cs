﻿using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LevelIOManager : MonoBehaviour
{	
	public string levelName;
	public int levelWidth;
	public int levelHeight;
	public float levelGravity;
	
	private string levelDir = "Assets/Levels";
	private Level levelToSave;
	
	public void BuildLevelPrimitiveAndSaveLevel()
	{
		levelToSave = new Level();
		
		levelToSave.levelName = this.levelName;
		levelToSave.levelWidth = this.levelWidth;
		levelToSave.levelHeight = this.levelHeight;
		levelToSave.levelGravity = this.levelGravity;
		
		levelToSave.tiles = new List<Level.Tile>();
		
		foreach(Transform child in transform)
		{
			if(child.tag == "Tiles")
			{
				Level.Tile tile = new Level.Tile();
				
				//All tile prefabs should be one word, this then truncates the " #" and returns the name
				tile.prefab = child.name.Split(' ')[0];
				tile.posX = (int) child.transform.localPosition.x; //make function to turn into absolute position
				tile.posY = (int) child.transform.localPosition.y;
				tile.rot = (int) child.transform.localRotation.z;
				tile.scaleX = (int) child.localScale.x; //make function to find absolute scale
				tile.scaleY = (int) child.localScale.y;
				
				levelToSave.tiles.Add(tile);
			}
		}
		Debug.Log("Level contains " + levelToSave.tiles.Count + " tiles");
		SaveLevel();
	}
	
	public string GetLevelPath()
	{
		return Path.Combine(levelDir, levelName + ".xml");
	}
	
	public void SaveLevel()
	{
		var serializer = new XmlSerializer(typeof(Level));
		using(var stream = new FileStream(GetLevelPath(), FileMode.Create))
		{
			serializer.Serialize(stream, levelToSave);
		}
	}
	
	public static Level LoadLevel(string path)
	{
		var serializer = new XmlSerializer(typeof(Level));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as Level;
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static Level LoadLevelFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(Level));
		return serializer.Deserialize(new StringReader(text)) as Level;
	}
}