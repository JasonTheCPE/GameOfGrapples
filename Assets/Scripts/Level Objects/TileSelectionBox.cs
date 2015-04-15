using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class TileSelectionBox : MonoBehaviour
{
	public string tilePrefabDir = "Assets/Prefabs/Tiles";
	public List<GameObject> tileDisplays;
	public GameObject tileDisplayPrefab;
	
	private float width;
	
	//private float height;
	
	
	void Start()
	{
		tileDisplays = new List<GameObject>();
		width = transform.localScale.x;
		
		DirectoryInfo tilesDir = new DirectoryInfo(tilePrefabDir);
		FileInfo[] tilesInfo = tilesDir.GetFiles("*.*");
		
		foreach(FileInfo f in tilesInfo)
		{
			if(f.Name.EndsWith(".prefab"))
			{
				GameObject newTile = Instantiate(Resources.LoadAssetAtPath<GameObject>(tilePrefabDir + "/" + f.Name)) as GameObject;
				GameObject instance = (GameObject) Instantiate(tileDisplayPrefab, new Vector3(0f, 0f, -2f), Quaternion.identity);
				
				instance.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().sprite
					= newTile.GetComponentInChildren<SpriteRenderer>().sprite;
				instance.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().material
					= newTile.GetComponentInChildren<SpriteRenderer>().material;
				instance.GetComponentInChildren<Canvas>().GetComponentInChildren<InputField>().text
					= f.Name.Split('.')[0];
				
				//newTile.transform.parent = transform;
				Destroy(newTile);
				instance.transform.parent = transform;
				
				
				//instance.SetActive(true);
				//instance.transform.localPosition = new Vector3(100, 0, -2);
				//instance.transform.localScale = new Vector3(100, 100, 1);
				//tileDisplays.Add(instance);
				
				break;
			}
		}
	}
}
