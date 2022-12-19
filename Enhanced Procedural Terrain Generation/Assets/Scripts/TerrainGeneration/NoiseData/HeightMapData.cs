using UnityEngine;

[System.Serializable]
public class HeightMapData : MonoBehaviour
{
	[Header("Perlin Noise Data")]
	public int mapWidth;
	public int mapHeight;
	public float noiseScale;
	[Range(1f, 8f)]
	public int octaves;
	[Range(0f, 1f)]
	public float persistance;
	[Range(1f, 18f)]
	public float lacunarity;
	public Vector2 offset; 
	public int seed;

	public void HeightMap()
	{ 
	}

	private void OnValidate()
	{
		if (mapWidth < 1) mapWidth = 1;
		if (mapHeight < 1) mapHeight = 1;
		if (noiseScale < 2) noiseScale = 2;
	}
}
