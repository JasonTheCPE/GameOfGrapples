using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Level")]
public class Level
{
	[XmlAttribute("LevelName")]
	public string levelName;
	
	[XmlAttribute("LevelWidth")]
	public int levelWidth;
	[XmlAttribute("LevelHeight")]
	public int levelHeight;
	
	[XmlAttribute("LevelGravity")]
	public float levelGravity;
	
	[XmlAttribute("LevelBorder")]
	public string levelBorder;
	
	[XmlArray("Tiles"), XmlArrayItem("Tile")]
	public List<Tile> tiles;
	
	[Serializable]
	public class Tile
	{
		[XmlAttribute("prefab")]
		public string prefab;
		
		[XmlAttribute("posX")]
		public float posX;
		[XmlAttribute("posY")]
		public float posY;
		
		[XmlAttribute("rot")]
		public float rot;
		
		[XmlAttribute("scaleX")]
		public float scaleX;
		[XmlAttribute("scaleY")]
		public float scaleY;
	}
}