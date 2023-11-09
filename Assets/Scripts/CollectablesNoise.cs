using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectablesNoise {

    public static float[,] GenerateNoise(string seed, int width, int height) {
        float[,] collectablesNoise = new float[width, height];

        System.Random pseudoRandomNumberGen = new System.Random(seed.GetHashCode());

        int offSetX = pseudoRandomNumberGen.Next(-1000, 1000);
        int offSetY = pseudoRandomNumberGen.Next(-1000, 1000);

        for (int y = 0; y < height; y+=pseudoRandomNumberGen.Next(7,9)) {
            for (int x = 0; x < width; x+=pseudoRandomNumberGen.Next(10,12)) {
                float sampleX = x  / (float)offSetX - y;
                float sampleY = y  / (float)offSetY - x;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                collectablesNoise[x, y] = perlinValue;
            }  
        }    

        return collectablesNoise;
    }
}
