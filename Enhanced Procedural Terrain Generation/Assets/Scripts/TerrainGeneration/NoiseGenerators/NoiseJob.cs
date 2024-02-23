using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

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
	}

	[ReadOnly]
	public NoiseData NoiseMapData;

	[WriteOnly]
	public NativeArray<float> NoiseMap;

	[BurstCompile]
	public void Execute()
	{
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = NoiseMapData.MapWidth / 2;
		float halfHeight = NoiseMapData.MapHeight / 2;

		NativeArray<float> _noiseMap = new(NoiseMapData.MapWidth * NoiseMapData.MapHeight ,Allocator.Temp); ;

		for (int y = 0; y < NoiseMapData.MapHeight; y++)
		{
			for (int x = 0; x < NoiseMapData.MapWidth; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < NoiseMapData.Octaves; i++)
				{
					float sampleX = (x - halfWidth) / NoiseMapData.Scale * frequency + NoiseMapData.OctaveOffsets[i].x;
					float sampleY = (y - halfHeight) / NoiseMapData.Scale * frequency + NoiseMapData.OctaveOffsets[i].y;
					float2 sampleXY = new float2(sampleX, sampleY);

					float perlinValue = noise.snoise(sampleXY);

					noiseHeight += perlinValue * amplitude;

					amplitude *= NoiseMapData.Persistence;
					frequency *= NoiseMapData.Lacunarity;
				}

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
	}
}