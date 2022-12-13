using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[Header("Noise Data")]
	public HeightMapData heightMapData;

	public void GenerateMap()
	{
		float[,] noiseMap = HeightMap.GenerateNoiseMap(heightMapData);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		display.DrawNoiseMap(noiseMap);
	}
}
