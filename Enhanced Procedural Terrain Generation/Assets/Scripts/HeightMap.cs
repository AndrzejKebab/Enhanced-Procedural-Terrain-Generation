using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMap
{
	public static float[,] GenerateNoiseMap(HeightMapData heightMapData)
	{
		System.Random prng = new System.Random(heightMapData.seed);
		Vector2[] octaveOffsets = new Vector2[heightMapData.octaves];
		for(int i = 0; i < heightMapData.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + heightMapData.offset.x;
			float offsetY = prng.Next(-100000, 100000) + heightMapData.offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[heightMapData.mapWidth, heightMapData.mapHeight];

		float maxNoiseHeigth = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = heightMapData.mapWidth / 2;
		float halfHeight = heightMapData.mapHeight / 2;
		
		for(int y = 0; y < heightMapData.mapHeight; y++)
		{
			for(int x = 0; x < heightMapData.mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for(int i = 0; i < heightMapData.octaves; i++)
				{
					float sampleX = (x - halfWidth) / heightMapData.noiseScale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / heightMapData.noiseScale * frequency + octaveOffsets[i].y;

					float perlingValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlingValue * amplitude;

					amplitude *= heightMapData.persistance;
					frequency *= heightMapData.lacunarity;
				}

				if(noiseHeight > maxNoiseHeigth)
				{
					maxNoiseHeigth = noiseHeight;
				}
				else if(noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < heightMapData.mapHeight; y++)
		{
			for (int x = 0; x < heightMapData.mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeigth, noiseMap[x, y]);

			}
		}

		return noiseMap;
	}

}
