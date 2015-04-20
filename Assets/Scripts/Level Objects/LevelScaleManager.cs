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
	
	//Initializes this class, first set levelWidth and levelHeight
	public void CalculateScales()
	{
		widthScaled = (float) actualWidth / levelWidth;
		heightScaled = (float) actualHeight / levelHeight;
		scaledUnitSize = widthScaled < heightScaled ? widthScaled : heightScaled;
		//Debug.Log("ScreenWidth: " + Screen.width + " ScreenHeight: " + Screen.height);
		//Debug.Log("WidthScaled: " + widthScaled + " HeightScaled: " + heightScaled + " scaledUnitSize: " + scaledUnitSize);
	}
	
	public void ScaleObjectWithSprite(GameObject obj)
	{
		Transform trans = obj.GetComponent<Transform>();
		var spriteBounds = obj.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size;
		
		trans.localScale = new Vector3(scaledUnitSize / spriteBounds.x, scaledUnitSize / spriteBounds.y, 1f);
	}
	
	public Vector3 SnapVector(int snapsPerGridUnit, Vector3 vectToSnap)
	{
		float snapDiv = scaledUnitSize / snapsPerGridUnit;
		float nudge = snapDiv / 2;
		return new Vector3(vectToSnap.x - Mathf.Repeat(vectToSnap.x, snapDiv) + nudge, vectToSnap.y - Mathf.Repeat(vectToSnap.y, snapDiv), vectToSnap.z);
	}
	
	public void KeepObjectWithSpriteInBounds(GameObject obj)
	{
		Transform trans = obj.GetComponent<Transform>();
		var sprBounds = obj.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size;
		float maxX = (scaledUnitSize * levelWidth / 2) - trans.localScale.x * sprBounds.x / 2;
		float maxY = (scaledUnitSize * levelHeight / 2) -  trans.localScale.y * sprBounds.y / 2;
		//Debug.Log("MaxY, MaxX - " + maxY + " " + maxX);
		
		trans.position = new Vector3(Mathf.Clamp(trans.position.x, -maxX, maxX), Mathf.Clamp(trans.position.y, -maxY, maxY), trans.position.z);
	}
	
	public void InitializeVoidBorders(Canvas hasBorders)
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
