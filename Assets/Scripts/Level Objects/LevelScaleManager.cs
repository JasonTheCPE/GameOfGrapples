using UnityEngine;
using System.Collections;

public class LevelScaleManager : MonoBehaviour
{
	public int levelWidth;
	public int levelHeight;
	public float scaledUnitSize;
	
	private float widthScaled;
	private float heightScaled;
	
	public const int gameResolutionWidth = 1440;
	public const int gameResolutionHeight = 900;
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
		var spriteBounds = tile.GetComponentInChildren<SpriteRenderer>().sprite.bounds;
		
		trans.localScale = new Vector3(scaledUnitSize / spriteBounds.size.x, scaledUnitSize / spriteBounds.size.y, 1f);
	}
	
	public Vector3 SnapVector(int snapsPerGridUnit, Vector3 vectToSnap)
	{
		float snapDiv = scaledUnitSize / snapsPerGridUnit;
		float nudge = snapDiv / 2;
		Vector3 newVector = new Vector3(vectToSnap.x - Mathf.Repeat(vectToSnap.x, snapDiv) + nudge, vectToSnap.y - Mathf.Repeat(vectToSnap.y, snapDiv), vectToSnap.z);
		return newVector;
	}
}
