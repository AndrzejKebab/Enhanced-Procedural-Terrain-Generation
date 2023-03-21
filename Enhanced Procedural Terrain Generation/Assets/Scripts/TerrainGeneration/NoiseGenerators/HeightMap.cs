using Unity.Mathematics;
using UnityEngine;

public static class HeightMap
{
	public static float[,] GenerateNoiseMap(HeightMapData heightMapData, int mapHeight, int mapWidth)
	{
		System.Random prng = new System.Random(heightMapData.seed);
		Vector2[] octaveOffsets = new Vector2[heightMapData.octaves];

		for(int i = 0; i < heightMapData.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + heightMapData.offset.x;
			float offsetY = prng.Next(-100000, 100000) + heightMapData.offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxNoiseHeigth = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2;
		float halfHeight = mapHeight / 2;
		
		for(int y = 0; y < mapHeight; y++)
		{
			for(int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for(int i = 0; i < heightMapData.octaves; i++)
				{
					float sampleX = (x - halfWidth) / heightMapData.noiseScale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / heightMapData.noiseScale * frequency + octaveOffsets[i].y;
					
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;

					noiseHeight += perlinValue * amplitude;

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

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeigth, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}

	public static float[,] GenerateContinentalnessMap(HeightMapData continentalnessData, int mapHeight, int mapWidth)
	{
		System.Random prng = new System.Random(continentalnessData.seed);
		Vector2[] octaveOffsets = new Vector2[continentalnessData.octaves];

		for (int i = 0; i < continentalnessData.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + continentalnessData.offset.x;
			float offsetY = prng.Next(-100000, 100000) + continentalnessData.offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxNoiseHeigth = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2;
		float halfHeight = mapHeight / 2;

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < continentalnessData.octaves; i++)
				{
					float sampleX = (x - halfWidth) / continentalnessData.noiseScale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / continentalnessData.noiseScale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

					noiseHeight += perlinValue * amplitude;

					amplitude *= continentalnessData.persistance;
					frequency *= continentalnessData.lacunarity;
				}

				if (noiseHeight > maxNoiseHeigth)
				{
					maxNoiseHeigth = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight + 0.35f;
			}
		}

		return noiseMap;
	}

	public static float[,] GenerateErosionMap(HeightMapData erosionData, int mapHeight, int mapWidth)
	{
		System.Random prng = new System.Random(erosionData.seed);
		Vector2[] octaveOffsets = new Vector2[erosionData.octaves];

		for (int i = 0; i < erosionData.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + erosionData.offset.x;
			float offsetY = prng.Next(-100000, 100000) + erosionData.offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxNoiseHeigth = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2;
		float halfHeight = mapHeight / 2;

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < erosionData.octaves; i++)
				{
					float sampleX = (x - halfWidth) / erosionData.noiseScale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / erosionData.noiseScale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					amplitude *= erosionData.persistance;
					frequency *= erosionData.lacunarity;
				}

				if (noiseHeight > maxNoiseHeigth)
				{
					maxNoiseHeigth = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeigth, noiseMap[x, y]);
				noiseMap[x, y] = -0.15f + noiseMap[x, y];
			}
		}

		return noiseMap;
	}

	public static float[,] GeneratePeaksAndValleysMap(HeightMapData peaksAndValleys, int mapHeight, int mapWidth)
	{
		System.Random prng = new System.Random(peaksAndValleys.seed);
		Vector2[] octaveOffsets = new Vector2[peaksAndValleys.octaves];

		for (int i = 0; i < peaksAndValleys.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + peaksAndValleys.offset.x;
			float offsetY = prng.Next(-100000, 100000) + peaksAndValleys.offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxNoiseHeigth = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2;
		float halfHeight = mapHeight / 2;

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < peaksAndValleys.octaves; i++)
				{
					float sampleX = (x - halfWidth) / peaksAndValleys.noiseScale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / peaksAndValleys.noiseScale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					amplitude *= peaksAndValleys.persistance;
					frequency *= peaksAndValleys.lacunarity;
				}

				if (noiseHeight > maxNoiseHeigth)
				{
					maxNoiseHeigth = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.Abs(noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}