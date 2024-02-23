using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public static class Noisegenerator
{
	public static float[,] GenerateNoiseMap(HeightMapData noiseMapData, int mapHeight, int mapWidth, int seed, int typeIndex)
	{
		System.Random prng = new System.Random(seed);
		NativeArray<float2> octaveOffsets = new NativeArray<float2>(noiseMapData.octaves, Allocator.TempJob);

		for (int i = 0; i < noiseMapData.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + noiseMapData.offset.x;
			float offsetY = prng.Next(-100000, 100000) + noiseMapData.offset.y;

			octaveOffsets[i] = new float2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];

		var noiseData = new NoiseJob()
		{
			NoiseMap = new NativeArray<float>(mapWidth * mapHeight, Allocator.TempJob),
		};

		JobHandle generateNoise = new NoiseJob
		{
			NoiseMapData = new NoiseJob.NoiseData
			{
				OctaveOffsets = octaveOffsets,
				MapWidth = mapWidth,
				MapHeight = mapHeight,
				Scale = noiseMapData.noiseScale,
				Octaves = noiseMapData.octaves,
				Persistence = noiseMapData.persistance,
				Lacunarity = noiseMapData.lacunarity,
				NoiseTypeIndex = typeIndex,
			},
			NoiseMap = noiseData.NoiseMap
		}.Schedule();

		generateNoise.Complete();

		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				noiseMap[x, y] = noiseData.NoiseMap[x + y * mapWidth];
			}
		}

		return noiseMap;
	}
}