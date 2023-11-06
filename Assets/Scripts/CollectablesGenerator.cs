using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectablesGenerator {

    public static CollectableData GenerateCollectables(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
        CollectableData collectableData = new CollectableData(CollectableNoise.GenerateNoiseMap(mapWidth, mapHeight, scale, seed, octaves, persistance, lacunarity, offset));
        return collectableData;
    }

}