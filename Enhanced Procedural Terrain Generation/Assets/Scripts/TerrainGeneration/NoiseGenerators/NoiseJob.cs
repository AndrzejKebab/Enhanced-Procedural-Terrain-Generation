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
	[ReadOnly] public int Width;
	[ReadOnly] public int Height;
	[ReadOnly] public float Scale;
	[ReadOnly] public float Lacunarity;
	[ReadOnly] public NoiseType NoiseType;
	[ReadOnly] public int Seed;

	[NativeDisableUnsafePtrRestriction]
	[ReadOnly] public IntPtr nodePtr;

	[WriteOnly]
	public NativeArray<float> NoiseMap;

	[BurstCompile]
	public void Execute()
	{
		GenerateNoise();
	}

	private void GenerateNoise()
	{
		NativeArray<float> _noiseMap = new(Width * Height, Allocator.Temp);

		var minmax = GenUniformGrid2D(nodePtr, _noiseMap, 0, 0, Width, Height, 1 / Scale, Seed);

		for (int y = 0; y < Height; y++)
		for (int x = 0; x < Width; x++)
		{
			switch (NoiseType)
			{
				case NoiseType.Continentalness:
					NoiseMap = _noiseMap;
					break;
				case NoiseType.Erosion:
					NoiseMap[x + y * Width] = math.unlerp(minmax.min, minmax.max, _noiseMap[x + y * Width]);
					break;
				case NoiseType.PeaksAndValleys:
					NoiseMap[x + y * Width] = math.abs(_noiseMap[x + y * Width]);
					break;
			}
		}
	}
}