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

	public CustomGradient customGradient;
	public TerrainType[] regions;

	public void GenerateMap()
	{
		float[,] noiseMap = new float[Width, Height];
		int[,] heightMap = new int[Width, Height];
		float[,] continentalnessMap = NoiseGenerator.GenerateNoiseMap(continentalnessData, Height, Width, Seed);
		float[,] erosionMap = NoiseGenerator.GenerateNoiseMap(erosionData, Height, Width, Seed);
		float[,] peakAndValleysMap = NoiseGenerator.GenerateNoiseMap(peakAndValleysData, Height, Width, Seed);

		Color[] colorMap = new Color[Width * Height];

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;
	
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{

				var continentalness = continentalnessCurve.Evaluate(continentalnessMap[x, y]);
				var erosion = erosionCurve.Evaluate(erosionMap[x, y]);
				var peakAndValleys = peakAndValleysCurve.Evaluate(peakAndValleysMap[x, y]);
				int currentHeight = Mathf.FloorToInt(continentalness * (1.01f - erosion) * (1 + peakAndValleys)) + 50 ;

				if (currentHeight > maxNoiseHeight)
				{
					maxNoiseHeight = currentHeight;
				}
				else if (currentHeight < minNoiseHeight)
				{
					minNoiseHeight = currentHeight;
				}

				heightMap[x, y] = currentHeight;
			}
		}

		if (drawMode == DrawMode.NoiseMap)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightMap[x, y]);
				}
			}
		}
		else
		{
			customGradient.Clear();
			for (int i = 0; i < regions.Length; i++)
			{
				customGradient.AddKey(regions[i].Color, Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, regions[i].Height));
			}

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					colorMap[x + y * Width] = customGradient.Evaluate(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightMap[x, y]));
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay>();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, Width, Height));
		}
	}
}

[System.Serializable]
public struct TerrainType
{
	public string Name;
	public float Height;
	public Color Color;
}