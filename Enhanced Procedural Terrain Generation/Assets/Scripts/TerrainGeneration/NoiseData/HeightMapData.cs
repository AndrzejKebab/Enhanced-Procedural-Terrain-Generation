using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NoiseData", menuName = "Noise", order = 1)]
public class HeightMapData : ScriptableObject
{
	[Header("Perlin Noise Data")]
	public float NoiseScale;
	[Range(1f, 8f)]
	public int Octaves;
	[Range(0f, 1f)]
	public float Persistence;
	[Range(1f, 4f)]
	public float Lacunarity;
	public Vector2 Offset;
}