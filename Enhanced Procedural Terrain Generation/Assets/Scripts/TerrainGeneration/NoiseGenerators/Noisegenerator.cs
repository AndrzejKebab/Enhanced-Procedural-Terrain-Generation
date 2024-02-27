using Unity.Collections;
using Unity.Jobs;

public static class NoiseGenerator
{
	public static float[,] GenerateNoiseMap(HeightMapData noiseMapData, int mapHeight, int mapWidth, int seed)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		var noiseData = new NoiseJob()
		{
			NoiseMap = new NativeArray<float>(mapWidth * mapHeight, Allocator.TempJob),
		};

		FastNoise fastNoise = new("FractalFBm");
		fastNoise.Set("Source", new FastNoise("OpenSimplex2"));
		fastNoise.Set("Octaves", noiseMapData.Octaves);
		fastNoise.Set("Lacunarity", noiseMapData.Lacunarity);
		fastNoise.Set("Gain", noiseMapData.Persistence);

		JobHandle generateNoise = new NoiseJob
		{

			Width = mapWidth,
			Height = mapHeight,
			Scale = noiseMapData.NoiseScale,
			Lacunarity = noiseMapData.Lacunarity,
			NoiseType = noiseMapData.NoiseType,
			Seed = seed,
			NoiseMap = noiseData.NoiseMap,
			nodePtr = fastNoise.NodeHandlePtr,
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