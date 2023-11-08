using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CavernsGenerator {
    public static CavernsData GenerateCaverns (int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
        CavernsData cavernsData = new CavernsData(CavernEntranceNoise.GenerateNoiseMap(mapWidth, mapHeight, scale, seed, octaves, persistance, lacunarity, offset));
        return cavernsData;
    }
}
