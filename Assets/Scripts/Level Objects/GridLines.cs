using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridLines : MonoBehaviour
{
	public float scaledUnitSize;
	public int levelWidth;
	public int levelHeight;
	public float lineWidth;
	public Color color = Color.green;
	public bool gridActive;
	
	public List<Vector3> gridVertices;
	private float halfWidth;
	private float halfHeight;
	
	public void InitializeGrid()
	{
		halfWidth = levelWidth / 2.0f;
		halfHeight = levelHeight / 2.0f;
		InitGridVertices();
		gridActive = false;
		
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.SetColors(color, color);
		lineRenderer.SetWidth(lineWidth, lineWidth);
		lineRenderer.SetVertexCount(gridVertices.Count);
	}
	
	void Update()
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		
		if(!gridActive)
		{
			lineRenderer.enabled = false;
			return;
		}
		lineRenderer.enabled = true;
		
		int i = 0;
		foreach(Vector3 vertex in gridVertices)
		{
			lineRenderer.SetPosition(i++, vertex);
		}
	}
	
	private void InitGridVertices()
	{
		gridVertices = new List<Vector3>();
		float buffer = 8.0f;
		float top = scaledUnitSize * halfHeight;
		float bottom = -scaledUnitSize * halfHeight;
		float right = scaledUnitSize * halfWidth;
		float left = -scaledUnitSize * halfWidth;
				
		//Set vertical lines
		int i = 0;
		while(i <= levelWidth)
		{
			gridVertices.Add(new Vector3(left + i * scaledUnitSize, top + buffer, 0f));
			gridVertices.Add(new Vector3(left + i * scaledUnitSize, bottom - buffer, -1.0f));
			++i;
			gridVertices.Add(new Vector3(left + i * scaledUnitSize, bottom - buffer, 0f));
			gridVertices.Add(new Vector3(left + i * scaledUnitSize, top + buffer, -1.0f));
			++i;
		}
		
		//Get into top right position for horizontal lines
		gridVertices.Add(new Vector3(right + buffer, scaledUnitSize * halfHeight, 0f));
		
		//Set horizontal lines
		i = 0;
		while(i <= levelHeight)
		{
			gridVertices.Add(new Vector3(left - buffer, top - i * scaledUnitSize, 0f));
			++i;
			gridVertices.Add(new Vector3(left - buffer, top - i * scaledUnitSize, -1.0f));
			gridVertices.Add(new Vector3(right + buffer, top - i * scaledUnitSize, 0f));
			++i;
			gridVertices.Add(new Vector3(right + buffer, top - i * scaledUnitSize, -1.0f));
		}
	}
	
	
}
