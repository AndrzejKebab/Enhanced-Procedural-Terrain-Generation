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
	public int Height;
	public int Width;
	public int Seed;
	public HeightMapData continentalnessData;
	public AnimationCurve continentalnessCurve;
	public HeightMapData peakAndValleysData;
	public AnimationCurve peakAndValleysCurve;
	public HeightMapData erosionData;
	public AnimationCurve erosionCurve;

	public TerrainType[] regions;

	public void GenerateMap()
	{
		float[,] noiseMap = new float[Width, Height];
		float[,] continentalnessMap = Noisegenerator.GenerateNoiseMap(continentalnessData, Height, Width, Seed, 0);
		float[,] erosionMap = Noisegenerator.GenerateNoiseMap(erosionData, Height, Width, Seed, 1);
		float[,] peakAndValleysMap = Noisegenerator.GenerateNoiseMap(peakAndValleysData, Height, Width, Seed, 2);

		Color[] colorMap = new Color[Width * Height];

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		for (int y = 0; y < Height; y++)
		{
			for(int x = 0; x < Width; x++)
			{
				float currentHeight =  continentalnessCurve.Evaluate(continentalnessMap[x, y]);

				currentHeight *= 1 - erosionCurve.Evaluate(erosionMap[x, y]);
				
				//if (erosionMap[x, y] >= 0.5f)
				//{
				//	currentHeight = 100 * erosionCurve.Evaluate(erosionMap[x, y] + 50);
				//}
				
				currentHeight *= 0.6f + peakAndValleysCurve.Evaluate(peakAndValleysMap[x, y]);

				if (peakAndValleysMap[x, y] <= 0.10f)
				{
					currentHeight *= peakAndValleysCurve.Evaluate(peakAndValleysMap[x, y]);
				}

				for(int i = 0; i < regions.Length; i++)
				{
					if(currentHeight <= regions[i].height)
					{
						colorMap[y * Width + x] = regions[i].color;
						break;
					}
				}
				
				if(drawMode == DrawMode.NoiseMap)
				{
					if (currentHeight > maxNoiseHeight)
					{
						maxNoiseHeight = currentHeight;
					}
					else if (currentHeight < minNoiseHeight)
					{
						minNoiseHeight = currentHeight;
					}

					noiseMap[x, y] = currentHeight;
				}
			}
		}

		if(drawMode == DrawMode.NoiseMap)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
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
			display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, Width, Height));
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