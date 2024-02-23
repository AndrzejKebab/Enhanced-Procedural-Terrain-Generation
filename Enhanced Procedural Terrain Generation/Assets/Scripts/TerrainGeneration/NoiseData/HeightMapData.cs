using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NoiseData", menuName = "Noise", order = 1)]
public class HeightMapData : ScriptableObject
{
	[Header("Perlin Noise Data")]
	public float noiseScale;
	[Range(1f, 8f)]
	public int octaves;
	[Range(0f, 1f)]
	public float persistance;
	[Range(1f, 4f)]
	public float lacunarity;
	public Vector2 offset;
}