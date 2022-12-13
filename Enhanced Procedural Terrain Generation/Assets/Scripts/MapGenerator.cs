using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[Header("Noise Data")]
	public HeightMapData heightMapData;
	public bool useSecondNoiseMap;

	public void GenerateMap()
	{
		float[,] noiseMap = HeightMap.GenerateNoiseMap(heightMapData, useSecondNoiseMap);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		display.DrawNoiseMap(noiseMap);
	}
}
