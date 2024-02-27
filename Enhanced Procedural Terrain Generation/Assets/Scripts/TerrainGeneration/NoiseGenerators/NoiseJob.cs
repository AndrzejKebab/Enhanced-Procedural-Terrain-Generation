using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static FastNoise;

[BurstCompile(OptimizeFor = OptimizeFor.Performance, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
public struct NoiseJob : IJob
{
	public struct NoiseData
	{
		public NativeArray<float2> OctaveOffsets;
		public int MapWidth;
		public int MapHeight;
		public float Scale;
		public int Octaves;
		public float Persistence;
		public float Lacunarity;
		public int NoiseTypeIndex;
		public int Seed;
	}

	[NativeDisableUnsafePtrRestriction] public IntPtr nodePtr;

	[ReadOnly]
	public NoiseData NoiseMapData;

	[WriteOnly]
	public NativeArray<float> NoiseMap;

	[BurstCompile]
	public void Execute()
	{
		/*
		FastNoiseLite fastNoiseLite = new();
		fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
		fastNoiseLite.SetFractalType(FastNoiseLite.FractalType.FBm);
		fastNoiseLite.SetFractalOctaves(NoiseMapData.Octaves);
		fastNoiseLite.SetFractalLacunarity(NoiseMapData.Lacunarity);
		fastNoiseLite.SetFractalGain(NoiseMapData.Persistence);
		fastNoiseLite.SetFrequency(0.01f);
		fastNoiseLite.SetSeed(NoiseMapData.Seed);

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = NoiseMapData.MapWidth / 2;
		float halfHeight = NoiseMapData.MapHeight / 2;
		*/
		NativeArray<float> _noiseMap = new(NoiseMapData.MapWidth * NoiseMapData.MapHeight ,Allocator.Temp); ;
		/*
		for (int y = 0; y < NoiseMapData.MapHeight; y++)
		{
			for (int x = 0; x < NoiseMapData.MapWidth; x++)
			{
				float sampleX = (x - halfWidth) / NoiseMapData.Scale;
				float sampleY = (y - halfHeight) / NoiseMapData.Scale;

				//var noiseHeight = fastNoiseLite.GetNoise(sampleX, sampleY);

				_noiseMap[x + y * NoiseMapData.MapWidth] = noiseHeight;

				switch (NoiseMapData.NoiseTypeIndex)
				{
					case 0:
						NoiseMap[x + y * NoiseMapData.MapWidth] = _noiseMap[x + y * NoiseMapData.MapWidth];
						break;
					case 1:
						if (noiseHeight > maxNoiseHeight)
						{
							maxNoiseHeight = noiseHeight;
						}
						else if (noiseHeight < minNoiseHeight)
						{
							minNoiseHeight = noiseHeight;
						}
						NoiseMap[x + y * NoiseMapData.MapWidth] = math.unlerp(minNoiseHeight, maxNoiseHeight, _noiseMap[x + y * NoiseMapData.MapWidth]);
						break;
					case 2:
						NoiseMap[x + y * NoiseMapData.MapWidth] = math.abs(_noiseMap[x + y * NoiseMapData.MapWidth]);
						break;
				}
			}
		}
		*/

		GenUniformGrid2D(nodePtr, _noiseMap, 0, 0, NoiseMapData.MapWidth, NoiseMapData.MapHeight, 0.001f, NoiseMapData.Seed);
		for (int y = 0; y < NoiseMapData.MapHeight; y++)		
		for (int x = 0; x < NoiseMapData.MapWidth; x++)
			
		switch (NoiseMapData.NoiseTypeIndex)
		{
			case 0:
				return;
			case 1:
				NoiseMap[x + y * NoiseMapData.MapWidth] = math.unlerp(-1, 1, _noiseMap[x + y * NoiseMapData.MapWidth]);
				break;
			case 2:
				NoiseMap[x + y * NoiseMapData.MapWidth] = math.abs(_noiseMap[x + y * NoiseMapData.MapWidth]);
				break;
		}
	}
}