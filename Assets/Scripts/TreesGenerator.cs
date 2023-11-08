using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TreesGenerator {

    public static TreesData GenerateTrees(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
        TreesData collectableData = new TreesData(TreesNoise.GenerateNoiseMap(mapWidth, mapHeight, scale, seed, octaves, persistance, lacunarity, offset));
        return collectableData;
    }

}