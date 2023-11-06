using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

public static class CollectableNoise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];


        System.Random pseudoNumberGen = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 0.7f;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = pseudoNumberGen.Next(-100000, 100000) + offset.x;
            float offsetY = pseudoNumberGen.Next(-100000, 100000) - offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        if(scale <= 0) {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 0.7f;
                frequency = 1f;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octavesOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }

         for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight,maxLocalNoiseHeight,noiseMap[x,y]);
            }
         }

        return noiseMap;
    }
}

