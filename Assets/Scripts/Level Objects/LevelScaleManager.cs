using UnityEngine;
using System.Collections;

public class LevelScaleManager : MonoBehaviour
{
	public int levelWidth;
	public int levelHeight;
	public float scaledUnitSize;
	
	private float widthScaled;
	private float heightScaled;
	
	private const int gameResolutionWidth = 1440;
	private const int gameResolutionHeight = 900;
	private const int actualWidth = 480;
	private const int actualHeight = 300;
	
	public void CalculateScales()
	{
		widthScaled = (float) actualWidth / levelWidth;
		heightScaled = (float) actualHeight / levelHeight;
		scaledUnitSize = widthScaled < heightScaled ? widthScaled : heightScaled;
		//Debug.Log("ScreenWidth: " + Screen.width + " ScreenHeight: " + Screen.height);
		//Debug.Log("WidthScaled: " + widthScaled + " HeightScaled: " + heightScaled + " scaledUnitSize: " + scaledUnitSize);
	}
	
	public void ScaleTile(GameObject tile)
	{
		Transform trans = tile.GetComponent<Transform>();
		trans.localScale = new Vector3(trans.localScale.x * widthScaled, trans.localScale.y * heightScaled, trans.localScale.z);
	}
}
