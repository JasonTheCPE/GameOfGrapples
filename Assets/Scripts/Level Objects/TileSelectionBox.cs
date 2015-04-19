using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;

public class TileSelectionBox : MonoBehaviour
{
	public GameObject tileDisplayPrefab;
	public LevelEditorManager LEM;
	
	//Keep these at the current "Tile Display" prefab canvas size
	private const float tileDisplayWidth = 60f;
	private const float tileDisplayHeight = 60f;
	private const float halfTDW = 30f;
	private const float halfTDH = 30f;
	private const float vertSpaceRatio = 1.25f;
	
	public void SetupTileSelectionBox()
	{
		GameObject[] tileDisplays = Resources.LoadAll<GameObject>("Tiles");
		
		float height = tileDisplays.Length * tileDisplayHeight * vertSpaceRatio / 3f;
		
		transform.GetComponentInParent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		float width = transform.GetComponentInParent<RectTransform>().rect.width;
		
		// 1/6 of the free space in a row
		float tileXSpace = ((width / 3) - tileDisplayWidth) / 2;
		
		float topAnchor = tileDisplayHeight / vertSpaceRatio;
		float leftAnchor = -tileXSpace;
		
		int numTile = 0, colNum;
		float posX, posY;
		
		foreach(GameObject tile in tileDisplays)
		{
			GameObject tempTile = Instantiate(tile) as GameObject;
			GameObject newTileDisplay = Instantiate(tileDisplayPrefab, new Vector3(0f, 0f, -2f), Quaternion.identity) as GameObject;
			
			newTileDisplay.transform.SetParent(transform, true);
			newTileDisplay.SetActive(true);
			
			newTileDisplay.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().sprite
				= tempTile.GetComponentInChildren<SpriteRenderer>().sprite;
			//instance.GetComponentInChildren<Canvas>().transform.FindChild("Tile Image").GetComponent<Image>().material
			//    = newTile.GetComponentInChildren<SpriteRenderer>().material;
			newTileDisplay.GetComponentInChildren<Canvas>().GetComponentInChildren<InputField>().text
				= tile.name;
			
			AddSetCurrentTileListener(newTileDisplay.GetComponentInChildren<Button>(), tile.name);
			
			colNum = numTile % 3;
			posX = leftAnchor + (tileXSpace + tileDisplayWidth) * (colNum);
			posY = topAnchor - (tileDisplayHeight * vertSpaceRatio) * Mathf.Floor(numTile / 3);
			newTileDisplay.transform.position = new Vector3(posX, posY, -2f);
			
			++numTile;
			
			Destroy(tempTile);
		}
	}
	
	// For each loops don't work with assigning listeners properly so we need this
	private void AddSetCurrentTileListener(Button b, string name)
	{
		b.onClick.AddListener(() => LEM.SetCurrentTile(name));
	}
}
