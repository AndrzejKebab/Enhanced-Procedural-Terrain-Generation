using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode
{
	NoiseMap = 0,
	ColorMap = 1,
}

public class MapGenerator : MonoBehaviour
{
	public DrawMode drawMode;

	[Header("Noise Data")]
	public HeightMapData heightMapData;
	public bool useSecondNoiseMap;
	public TerrainType[] regions;

	public void GenerateMap()
	{
		float[,] noiseMap = HeightMap.GenerateNoiseMap(heightMapData, useSecondNoiseMap);

		Color[] colorMap = new Color[heightMapData.mapWidth * heightMapData.mapHeight];

		for(int y = 0; y < heightMapData.mapHeight; y++)
		{
			for(int x = 0; x < heightMapData.mapWidth; x++)
			{
				float currentHeight = noiseMap[x, y];

				for(int i = 0; i < regions.Length; i++)
				{
					if(currentHeight <= regions[i].height)
					{
						colorMap[y * heightMapData.mapWidth + x] = regions[i].color;
						break;
					}
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay>();
		
		if(drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if(drawMode == DrawMode.ColorMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, heightMapData.mapWidth, heightMapData.mapHeight));
		}

	}
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}
