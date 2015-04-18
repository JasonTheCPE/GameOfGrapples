using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class TileSelectionBox : MonoBehaviour
{
	public string tilePrefabDir = "Assets/Resources/Tiles";
	public GameObject[] tileDisplays;
	public GameObject tileDisplayPrefab;
	
	private float width;
	
	//private float height;
	
	
	void Start()
	{
		//tileDisplays = new List<GameObject>();
		width = transform.localScale.x;
		
		//DirectoryInfo tilesDir = new DirectoryInfo(tilePrefabDir);
		//FileInfo[] tilesInfo = tilesDir.GetFiles("*.*");
		
		tileDisplays = Resources.LoadAll<GameObject>(tilePrefabDir);
		
		foreach(GameObject tile in tileDisplays)
		{
			GameObject newTile = Instantiate(tile) as GameObject;
			GameObject instance = (GameObject) Instantiate(tileDisplayPrefab, new Vector3(0f, 0f, -2f), Quaternion.identity);
			
			instance.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().sprite
			    = newTile.GetComponentInChildren<SpriteRenderer>().sprite;
			//instance.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().material
			//    = newTile.GetComponentInChildren<SpriteRenderer>().material;
			instance.GetComponentInChildren<Canvas>().GetComponentInChildren<InputField>().text
			    = newTile.name.TrimEnd("(Clone)".ToCharArray());
			
			Destroy(newTile);
			instance.transform.parent = transform;
			break;
		}
	}
}
