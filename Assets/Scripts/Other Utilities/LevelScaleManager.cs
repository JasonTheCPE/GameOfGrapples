using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	
	//Initializes the LevelScaleManager to do scaling for a level with the given width and height.
	public void InitializeLevelScaleManager(int level_Width, int level_Height)
	{
		levelWidth = level_Width;
		levelHeight = level_Height;
		widthScaled = (float) actualWidth / levelWidth;
		heightScaled = (float) actualHeight / levelHeight;
		scaledUnitSize = widthScaled < heightScaled ? widthScaled : heightScaled;
		//Debug.Log("ScreenWidth: " + Screen.width + " ScreenHeight: " + Screen.height);
		//Debug.Log("WidthScaled: " + widthScaled + " HeightScaled: " + heightScaled + " scaledUnitSize: " + scaledUnitSize);
	}
	
	//Finds the SpriteRenderer in the children of this GameObject and scales its sprite to be 
	//the number of units wide and tall given.
	public void ScaleObjectWithSprite(GameObject obj, float unitsWide, float unitsTall)
	{
		Transform trans = obj.GetComponent<Transform>();
		var spriteBounds = obj.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size;
		
		trans.localScale = new Vector3(unitsWide * scaledUnitSize / spriteBounds.x, unitsTall * scaledUnitSize / spriteBounds.y, 1f);
	}
	
	//For a GameObject with a SpriteRenderer, adjusts the given GameObject's transform vector 
	//to keep it within the level bounds set for this LevelScaleManager based on the size of 
	//the GameObject's Sprite.
	public void KeepObjectWithSpriteInBounds(GameObject obj)
	{
		Transform trans = obj.GetComponent<Transform>();
		var sprBounds = obj.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size;
		float maxX = (scaledUnitSize * levelWidth / 2) - trans.localScale.x * sprBounds.x / 2;
		float maxY = (scaledUnitSize * levelHeight / 2) -  trans.localScale.y * sprBounds.y / 2;
		//Debug.Log("MaxY, MaxX - " + maxY + " " + maxX);
		
		trans.position = new Vector3(Mathf.Clamp(trans.position.x, -maxX, maxX), Mathf.Clamp(trans.position.y, -maxY, maxY), trans.position.z);
	}
	
	//Creates and returns a new Vector3 that is snapped into a grid that has snapsPerGridUnit 
	//number of possible x or y positions for an area that is scaledUnitSize by scaledUnitSize 
	//wide and tall.
	public Vector3 SnapVector(int snapsPerGridUnit, Vector3 vectToSnap)
	{
		float snapDiv = scaledUnitSize / snapsPerGridUnit;
		float nudgeX = levelWidth % 2 == 1 ? 0 : snapDiv / 2;
		float nudgeY = levelHeight % 2 == 1 ? 0 : snapDiv / 2;
		return new Vector3(vectToSnap.x - Mathf.Repeat(vectToSnap.x, snapDiv) + nudgeX, vectToSnap.y - Mathf.Repeat(vectToSnap.y, snapDiv) + nudgeY, vectToSnap.z);
	}
	
	//Given the Canvas that contains the Level border RawImages, this will adjust those RawImages 
	//to be in the perfect position and have the perfect size to frame the Level.
	public void InitializeVoidBorders(GameObject hasBorders)
	{
		RawImage topBorder = hasBorders.transform.Find("Top Border").GetComponent<RawImage>();
		RawImage bottomBorder = hasBorders.transform.Find("Bottom Border").GetComponent<RawImage>();
		RawImage leftBorder = hasBorders.transform.Find("Left Border").GetComponent<RawImage>();
		RawImage rightBorder = hasBorders.transform.Find("Right Border").GetComponent<RawImage>();
		
		float viewableMaxX = actualWidth / 2;
		float viewableMaxY = actualHeight / 2;
		float levelMaxX = (scaledUnitSize * levelWidth / 2);
		float levelMaxY = (scaledUnitSize * levelHeight / 2);

		float borderHeight = viewableMaxY - levelMaxY;
		float borderWidth = viewableMaxX - levelMaxX;
		
		topBorder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, actualWidth);
		topBorder.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, borderHeight);
		
		bottomBorder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, actualWidth);
		bottomBorder.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, borderHeight);
		
		leftBorder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, actualHeight);
		leftBorder.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, borderWidth);
		
		rightBorder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, actualHeight);
		rightBorder.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, borderWidth);
		
		topBorder.gameObject.SetActive(true);
		bottomBorder.gameObject.SetActive(true);
		leftBorder.gameObject.SetActive(true);
		rightBorder.gameObject.SetActive(true);
	}
}
