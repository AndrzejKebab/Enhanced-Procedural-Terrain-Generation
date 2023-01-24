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
	public HeightMapData continentalnessData;
	public AnimationCurve continentalnessCurve;
	public HeightMapData peakAndValleysData;
	public AnimationCurve peakAndValleysCurve;
	public HeightMapData erosionData;
	public AnimationCurve erosionCurve;

	public TerrainType[] regions;

	public void GenerateMap()
	{
		float[,] noiseMap = HeightMap.GenerateNoiseMap(heightMapData);
		float[,] continentalnessMap = HeightMap.GenerateContinentalnessMap(continentalnessData);
		float[,] erosionMap = HeightMap.GenerateErosionMap(erosionData);
		float[,] peakAndValleysMap = HeightMap.GeneratePeaksAndValleysMap(peakAndValleysData);

		Color[] colorMap = new Color[heightMapData.mapWidth * heightMapData.mapHeight];

		for(int y = 0; y < heightMapData.mapHeight; y++)
		{
			for(int x = 0; x < heightMapData.mapWidth; x++)
			{
				float currentHeight = (100 * continentalnessCurve.Evaluate(continentalnessMap[x, y]));

				//if (continentalnessMap[x, y] >= 0.3f)
				//{
					currentHeight -= 50 * erosionCurve.Evaluate(erosionMap[x, y]);
				//}

				if (erosionMap[x, y] >= 0.5f)
				{
					currentHeight = 100 * erosionCurve.Evaluate(erosionMap[x, y]) + 50;
				}

				//if (erosionMap[x, y] >= 0.4f)
				//{
					currentHeight += 100 * peakAndValleysCurve.Evaluate(peakAndValleysMap[x, y]);
				//}

				if (peakAndValleysMap[x, y] <= 0.10f)
				{
					currentHeight = peakAndValleysCurve.Evaluate(peakAndValleysMap[x, y]);
				}

				//currentHeight = Mathf.Clamp(currentHeight, 0, 200);

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
	public float continentalness;
	public float peaksAndValleys;
	public float erosion;
	public Color color;
}
